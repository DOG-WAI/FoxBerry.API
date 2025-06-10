using System.ComponentModel.DataAnnotations;

namespace FoxBerry.API.DTOs
{
    public class CreateCommentDto
    {
        [Required(ErrorMessage = "Содержание комментария обязательно.")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Комментарий должен быть от 1 до 500 символов.")]
        public string Content { get; set; } = string.Empty;
    }
}
