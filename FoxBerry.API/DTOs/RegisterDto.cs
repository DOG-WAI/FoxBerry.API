using System.ComponentModel.DataAnnotations; // Для атрибутов валидации

namespace FoxBerry.API.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Имя пользователя обязательно.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Имя пользователя должно быть от 3 до 50 символов.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email обязателен.")]
        [EmailAddress(ErrorMessage = "Некорректный формат email.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен быть минимум 6 символов.")]
        public string Password { get; set; } = string.Empty;
    }
}
