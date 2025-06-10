namespace FoxBerry.API.Models
{
    public class User
    {
        public int Id { get; set; } // Первичный ключ (Primary Key), EF Core распознает его по имени Id или UserId
        public string Username { get; set; } = string.Empty; // Имя пользователя, уникальное
        public string Email { get; set; } = string.Empty; // Email, уникальный
        public string PasswordHash { get; set; } = string.Empty; // Хеш пароля (никогда не храним пароли в открытом виде!)
        public string Bio { get; set; } = string.Empty; // Биография пользователя
        public string? ProfilePictureUrl { get; set; } // URL к фото профиля (может быть null)
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow; // Дата регистрации в UTC

        // Навигационные свойства: они связывают сущности друг с другом.
        // EF Core использует их для загрузки связанных данных.

        // Список постов, созданных этим пользователем
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        // Список комментариев, оставленных этим пользователем
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        // Список лайков, поставленных этим пользователем
        public ICollection<Like> Likes { get; set; } = new List<Like>();

        // Связи для подписок:
        // Пользователи, которые подписаны на меня (я - FollowingId в их Follow-записях)
        public ICollection<Follow> Followers { get; set; } = new List<Follow>();
        // Пользователи, на которых я подписан (я - FollowerId в моих Follow-записях)
        public ICollection<Follow> Following { get; set; } = new List<Follow>();
    }
}
