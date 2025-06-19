using BlackBoxCheckApi.ApiModels.ResponseModels;
using BlackBoxCheckApi.Models.Profiles;
using Microsoft.EntityFrameworkCore;

namespace BlackBoxCheckApi.Services
{
    public class AuthService
    {
        private readonly UsersContext _usersContext;
        private readonly ILogger<AuthService> _logger;

        public AuthService(UsersContext db, ILogger<AuthService> logger)
        {
            _usersContext = db;
            _logger = logger;
        }

        public async Task<RegistrationResult> RegisterUser(string username, string password)
        {
            try
            {
                if (await _usersContext.Users.AnyAsync(u => u.Login == username))
                {
                    return new RegistrationResult
                    {
                        Success = false,
                        Error = "Пользователь с таким логином уже существует"
                    };
                }

                var userProfile = new UserProfile()
                {
                    Login = username,
                    Password = BCrypt.Net.BCrypt.HashPassword(password)
                };

                _usersContext.Users.Add(userProfile);
                await _usersContext.SaveChangesAsync();

                return new RegistrationResult { Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при регистрации пользователя");
                return new RegistrationResult
                {
                    Success = false,
                    Error = "Произошла ошибка при регистрации"
                };
            }
        }
        public class RegistrationResult
        {
            public bool Success { get; set; }
            public string Error { get; set; }
        }
        public async Task<VerifyUserResponse> VerifyUser(string username, string password)
        {
            try
            {
                //Ищем пользователя по имени в базе
                var userProfile = await _usersContext.Users.FirstOrDefaultAsync(x => x.Login == username);
                //Если не нашли возвращаем false
                if (userProfile == null)
                {
                    return new VerifyUserResponse { IsVerified = false, UserProfile = userProfile, ErrorMessage = $"Пользователь {username} не найден." };
                }
                //Проверяем пароль
                bool IsPasswordValid = BCrypt.Net.BCrypt.Verify(password, userProfile.Password);
                return new VerifyUserResponse { IsVerified = IsPasswordValid, UserProfile = userProfile };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new VerifyUserResponse { ErrorMessage = ex.Message };
            }
        }
    }

}
