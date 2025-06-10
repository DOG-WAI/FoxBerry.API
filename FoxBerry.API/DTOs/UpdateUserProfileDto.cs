using Microsoft.AspNetCore.Http; // Для IFormFile
using System.ComponentModel.DataAnnotations;

namespace FoxBerry.API.DTOs
{
    public class UpdateUserProfileDto
    {
        [StringLength(500, ErrorMessage = "Биография не должна превышать 500 символов.")]
        public string? Bio { get; set; }

        public IFormFile? ProfilePictureFile { get; set; } // Новый файл изображения профиля (опционально)
    }
}
