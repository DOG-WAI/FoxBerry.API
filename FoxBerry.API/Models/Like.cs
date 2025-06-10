using System.ComponentModel.DataAnnotations.Schema; // Для атрибута [ForeignKey]

namespace FoxBerry.API.Models
{
    public class Like
    {
        public int Id { get; set; } // Первичный ключ
        public int UserId { get; set; } // Внешний ключ к таблице User (кто лайкнул)
        public int PostId { get; set; } // Внешний ключ к таблице Post (какой пост лайкнули)
        public DateTime Date { get; set; } = DateTime.UtcNow; // Дата и время лайка

        // Навигационные свойства:
        [ForeignKey("UserId")]
        public User? User { get; set; } = null!; // Пользователь, поставивший лайк

        [ForeignKey("PostId")]
        public Post? Post { get; set; } = null!; // Пост, который лайкнули
    }
}
