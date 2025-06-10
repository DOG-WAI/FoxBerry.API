namespace FoxBerry.API.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty; // Имя автора комментария
        public int PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
