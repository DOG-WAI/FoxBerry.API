namespace FoxBerry.API.DTOs
{
    public class PostDto
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string AuthorUsername { get; set; } = string.Empty; // Имя автора
        public string ImageUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public string? Location { get; set; }
        public DateTime UploadDate { get; set; }
        public int LikesCount { get; set; } // Количество лайков
        public int CommentsCount { get; set; } // Количество комментариев
        public bool IsLikedByCurrentUser { get; set; } // Лайкнул ли текущий пользователь
    }
}
