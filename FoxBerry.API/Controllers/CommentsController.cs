using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoxBerry.API.Data;
using FoxBerry.API.Models;
using FoxBerry.API.DTOs;
using Microsoft.AspNetCore.Authorization; // Для атрибута [Authorize]
using System.Security.Claims; // Для получения ID пользователя из токена

namespace FoxBerry.API.Controllers
{
    [Route("api/[controller]")] // Базовый маршрут: /api/Comments
    [ApiController]
    [Authorize] // Все методы в этом контроллере требуют аутентификации
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Вспомогательный метод для получения ID текущего пользователя из JWT токена
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new InvalidOperationException("User ID claim not found.");
            }
            return int.Parse(userIdClaim.Value);
        }

        // POST: api/Comments/post/{postId}
        // Добавление нового комментария к посту
        [HttpPost("post/{postId}")]
        public async Task<IActionResult> AddComment(int postId, [FromBody] CreateCommentDto createCommentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();

            // Проверяем, существует ли пост
            var postExists = await _context.Posts.AnyAsync(p => p.Id == postId);
            if (!postExists)
            {
                return NotFound("Пост не найден.");
            }

            var comment = new Comment
            {
                UserId = userId,
                PostId = postId,
                Content = createCommentDto.Content,
                Date = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Возвращаем CommentDto
            var commentDto = new CommentDto
            {
                Id = comment.Id,
                UserId = comment.UserId,
                Username = (await _context.Users.FindAsync(comment.UserId))?.Username ?? "Неизвестно",
                PostId = comment.PostId,
                Content = comment.Content,
                Date = comment.Date
            };

            return CreatedAtAction(nameof(GetCommentsForPost), new { postId = comment.PostId }, commentDto);
        }

        // GET: api/Comments/post/{postId}
        // Получение всех комментариев для конкретного поста
        [HttpGet("post/{postId}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsForPost(int postId)
        {
            var comments = await _context.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.User) // Загружаем данные пользователя, чтобы получить Username
                .OrderByDescending(c => c.Date) // Сортируем по дате (новые сверху)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    Username = c.User!.Username,
                    PostId = c.PostId,
                    Content = c.Content,
                    Date = c.Date
                })
                .ToListAsync();

            if (!comments.Any())
            {
                return NotFound("Комментарии для этого поста не найдены."); // Или Ok(new List<CommentDto>())
            }

            return Ok(comments);
        }

        // DELETE: api/Comments/{id}
        // Удаление комментария (только автором комментария)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var userId = GetCurrentUserId();

            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound("Комментарий не найден.");
            }

            // Проверяем, является ли текущий пользователь автором комментария
            if (comment.UserId != userId)
            {
                return StatusCode(403, new { message = "У вас нет прав для удаления этого комментария." }); // 403 Forbidden
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }
    }
}
