using System.ComponentModel.DataAnnotations.Schema; // Для атрибута [ForeignKey]


namespace FoxBerry.API.Models
{
    public class Follow
    {
        public int Id { get; set; } // Первичный ключ
        public int FollowerId { get; set; } // Внешний ключ к User (кто подписался)
        public int FollowingId { get; set; } // Внешний ключ к User (на кого подписались)
        public DateTime Date { get; set; } = DateTime.UtcNow; // Дата подписки

        // Навигационные свойства:
        // Follower - тот, кто подписался
        [ForeignKey("FollowerId")]
        public User? Follower { get; set; } = null!;

        // Following - тот, на кого подписались
        [ForeignKey("FollowingId")]
        public User? Following { get; set; } = null!;
    }
}
