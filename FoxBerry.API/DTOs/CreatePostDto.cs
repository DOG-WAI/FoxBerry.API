using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // Для IFormFile

namespace FoxBerry.API.DTOs
{
    public class CreatePostDto
    {
        [Required(ErrorMessage = "Изображение обязательно.")]
        public IFormFile ImageFile { get; set; } = null!; // Файл изображения

        [StringLength(2200, ErrorMessage = "Подпись не должна превышать 2200 символов.")]
        public string? Caption { get; set; } // Подпись (опционально)

        [StringLength(100, ErrorMessage = "Местоположение не должно превышать 100 символов.")]
        public string? Location { get; set; } // Местоположение (опционально)
    }
}
