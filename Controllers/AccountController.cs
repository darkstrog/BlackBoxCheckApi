using BlackBoxCheckApi.ApiModels.RequestModels;
using BlackBoxCheckApi.Models.Profiles;
using BlackBoxCheckApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlackBoxCheckApi.Controllers
{
    [Route("api/auth/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(AuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }
        /// <summary>
        /// Регистрация нового пользователя в системе
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterUser(request.Username, request.Password);

            if (result.Success)
                return Ok(new { message = "Регистрация успешно завершена" });

            return Conflict(new { message = result.Error });
        }
        /// <summary>
        /// Логин пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Возвращает токен JWT</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var result = await _authService.VerifyUser(model.Username, model.Password);

            if (!result.IsVerified)
            {
                return Unauthorized(new { message = result.ErrorMessage ?? "Неверный логин или пароль" });
            }

            var token = GenerateJwtToken(result.UserProfile);
            return Ok(new { token });
        }

        private string GenerateJwtToken(UserProfile user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Login),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("UserId", user.Id.ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
