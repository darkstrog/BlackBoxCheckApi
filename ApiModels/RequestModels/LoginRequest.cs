using System.ComponentModel.DataAnnotations;

namespace BlackBoxCheckApi.ApiModels.RequestModels
{
    /// <summary>
    /// Данные для авторизации
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Имя пользователя "Login"
        /// </summary>
        [Required]
        public string Username { get; set; } = default!;
        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [Required]
        public string Password { get; set; } = default!;

    }
}
