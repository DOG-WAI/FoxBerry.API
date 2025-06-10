using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoxBerry.API.Data;
using FoxBerry.API.Models;
using Microsoft.AspNetCore.Authorization; // Для атрибута [Authorize]
using System.Security.Claims; // Для получения ID пользователя из токена

namespace FoxBerry.API.Controllers
{
    [Route("api/[controller]")] // Базовый маршрут: /api/Likes
    [ApiController]
    [Authorize] // Все методы в этом контроллере требуют аутентификации
    public class LikesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LikesController(ApplicationDbContext context)
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

        // POST: api/Likes/post/{postId}
        // Добавление лайка к посту
        [HttpPost("post/{postId}")]
        public async Task<IActionResult> LikePost(int postId)
        {
            var userId = GetCurrentUserId();

            // Проверяем, существует ли пост
            var postExists = await _context.Posts.AnyAsync(p => p.Id == postId);
            if (!postExists)
            {
                return NotFound("Пост не найден.");
            }

            // Проверяем, не поставил ли пользователь уже лайк на этот пост
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);

            if (existingLike != null)
            {
                return Conflict("Вы уже лайкнули этот пост."); // 409 Conflict
            }

            var like = new Like
            {
                UserId = userId,
                PostId = postId,
                Date = DateTime.UtcNow
            };

            _context.Likes.Add(like);
            await _context.SaveChangesAsync();

            // Возвращаем обновленное количество лайков или просто успешный статус
            var likesCount = await _context.Likes.CountAsync(l => l.PostId == postId);
            return Ok(new { Message = "Пост успешно лайкнут!", LikesCount = likesCount });
        }

        // DELETE: api/Likes/post/{postId}
        // Удаление лайка с поста
        [HttpDelete("post/{postId}")]
        public async Task<IActionResult> UnlikePost(int postId)
        {
            var userId = GetCurrentUserId();

            // Проверяем, не поставил ли пользователь уже лайк на этот пост
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);

            if (existingLike == null)
            {
                return NotFound("Вы не лайкнули этот пост или пост не существует.");
            }

            _context.Likes.Remove(existingLike);
            await _context.SaveChangesAsync();

            var likesCount = _context.Likes.CountAsync(l => l.PostId == postId);
            return Ok(new { Message = "Лайк успешно удален!", LikesCount = likesCount });
        }
    }
}
