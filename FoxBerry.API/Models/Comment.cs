using System.ComponentModel.DataAnnotations.Schema; // Для атрибута [ForeignKey]

namespace FoxBerry.API.Models
{
    public class Comment
    {
        public int Id { get; set; } // Первичный ключ
        public int UserId { get; set; } // Внешний ключ к таблице User (кто оставил комментарий)
        public int PostId { get; set; } // Внешний ключ к таблице Post (к какому посту)
        public string Content { get; set; } = string.Empty; // Текст комментария
        public DateTime Date { get; set; } = DateTime.UtcNow; // Дата и время комментария

        // Навигационные свойства:
        [ForeignKey("UserId")]
        public User? User { get; set; } // Пользователь, оставивший комментарий

        [ForeignKey("PostId")]
        public Post? Post { get; set; } // Пост, к которому относится комментарий
    }
}
