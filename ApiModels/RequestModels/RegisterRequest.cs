using System.ComponentModel.DataAnnotations;

namespace BlackBoxCheckApi.ApiModels.RequestModels
{
    /// <summary>
    /// Данные для регистрации пользователя
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Имя пользователя "Login"
        /// </summary>
        [Required]
        public string Username { get; set; } = default!;

        /// <summary>
        /// Пароль
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
    }
}
