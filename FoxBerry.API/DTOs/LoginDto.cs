using System.ComponentModel.DataAnnotations; // Для атрибутов валидации

namespace FoxBerry.API.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Имя пользователя или Email обязателен.")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен.")]
        public string Password { get; set; } = string.Empty;
    }
}
