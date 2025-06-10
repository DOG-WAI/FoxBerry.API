using System.ComponentModel.DataAnnotations;

namespace FoxBerry.API.DTOs
{
    public class UpdatePostDto
    {
        [StringLength(2200, ErrorMessage = "Подпись не должна превышать 2200 символов.")]
        public string? Caption { get; set; }

        [StringLength(100, ErrorMessage = "Местоположение не должно превышать 100 символов.")]
        public string? Location { get; set; }
    }
}
