using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoxBerry.API.Data;
using FoxBerry.API.Models;
using FoxBerry.API.DTOs; // Возможно, понадобится для будущих DTOs с информацией о подписчиках/подписках
using Microsoft.AspNetCore.Authorization; // Для атрибута [Authorize]
using System.Security.Claims;
using System.Net.NetworkInformation; // Для получения ID пользователя из токена

namespace FoxBerry.API.Controllers
{
    [Route("api/[controller]")] // Базовый маршрут: /api/Follows
    [ApiController]
    [Authorize] // Все методы в этом контроллере требуют аутентификации
    public class FollowsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FollowsController(ApplicationDbContext context)
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

        // POST: api/Follows/user/{followingId}
        // Подписаться на пользователя
        [HttpPost("user/{followingId}")]
        public async Task<IActionResult> FollowUser(int followingId)
        {
            var followerId = GetCurrentUserId(); // Текущий пользователь - это тот, кто подписывается

            if (followerId == followingId)
            {
                return BadRequest("Вы не можете подписаться на самого себя.");
            }

            // Проверяем, существует ли пользователь, на которого подписываемся
            var existingFollow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

            if (existingFollow != null)
            {
                return Conflict("Вы уже подписаны на этого пользователя."); // 409 Conflict
            }

            var follow = new Follow
            {
                FollowerId = followerId,
                FollowingId = followingId,
                Date = DateTime.UtcNow
            };

            _context.Follows.Add(follow);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Вы успешно подписались на пользователя ID: {followingId}" });
        }

        // DELETE: api/Follows/user/{followingId}
        // Отписаться от пользователя
        [HttpDelete("user/{followingId}")]
        public async Task<IActionResult> UnfollowUser(int followingId)
        {
            var followerId = GetCurrentUserId(); // Текущий пользователь - это тот, кто отписывается

            var existingFollow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

            if (existingFollow == null)
            {
                return NotFound("Вы не подписаны на этого пользователя.");
            }

            _context.Follows.Remove(existingFollow);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Вы успешно отписались от пользователя ID: {followingId}" });
        }

        // GET: api/Follows/followers/{userId}
        // Получить список подписчиков для конкретного пользователя
        [HttpGet("followers/{userId}")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetFollowers(int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                return NotFound("Пользователь не найден.");
            }

            var followers = await _context.Follows
                .Where(f => f.FollowingId == userId) // Те, кто подписан на данного пользователя
                .Select(f => f.Follower!) // Выбираем объект Follower (User)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    Bio = u.Bio
                })
                .ToListAsync();

            return Ok(followers);
        }

        // GET: api/Follows/following/{userId}
        // Получить список пользователей, на которых подписан конкретный пользователь
        [HttpGet("following/{userId}")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetFollowing(int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                return NotFound("Пользователь не найден.");
            }

            var following = await _context.Follows
                .Where(f => f.FollowerId == userId) // Те, на кого подписан данный пользователь
                .Select(f => f.Follower!) // Выбираем объект Following (User)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    Bio = u.Bio
                })
                .ToListAsync();

            return Ok(following);
        }
    }
}
