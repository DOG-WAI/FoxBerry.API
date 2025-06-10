using System.ComponentModel.DataAnnotations.Schema; // Для атрибута [ForeignKey]

namespace FoxBerry.API.Models
{
    public class Post
    {
        public int Id { get; set; } // Первичный ключ
        public int AuthorId { get; set; } // Внешний ключ к таблице User

        public string ImageUrl { get; set; } = string.Empty; // URL изображения/видео
        public string? Caption { get; set; } // Подпись к публикации (может быть null)
        public string? Location { get; set; } // Местоположение (может быть null)
        public DateTime UploadDate { get; set; } = DateTime.UtcNow; // Дата и время загрузки

        // Навигационное свойство для связи "один-ко-многим": один пользователь может иметь много постов.
        // EF Core автоматически установит связь между Post.AuthorId и User.Id
        [ForeignKey("AuthorId")] // Указывает, что AuthorId является внешним ключом
        public User? Author { get; set; } // Сам объект User, к которому относится Post

        // Навигационные свойства для коллекций "многие-ко-одному":
        // Список комментариев к этому посту
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        // Список лайков к этому посту
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
