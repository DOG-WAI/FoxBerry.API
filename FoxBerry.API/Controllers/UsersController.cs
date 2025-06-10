using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoxBerry.API.Data;
using FoxBerry.API.Models;
using FoxBerry.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FoxBerry.API.Controllers
{
    [Route("api/[controller]")] // Базовый маршрут: /api/Users
    [ApiController]
    [Authorize] // Все методы в этом контроллере требуют аутентификации
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UsersController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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

        // GET: api/Users/{id}
        // Получение информации о профиле пользователя по ID
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<UserProfileDto>>> GetUserProfile(int id)
        {
            var currentUserId = GetCurrentUserId();

            var user = await _context.Users
                .Include(u => u.Posts) // Для подсчета постов
                .Include(u => u.Followers) // Для подсчета подписчиков
                .Include(u => u.Following) // Для подсчета подписок
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }

            // Проверяем, подписан ли текущий пользователь на этот профиль
            bool isFollowing = await _context.Follows
                .AnyAsync(f => f.FollowerId == currentUserId && f.FollowingId == id);

            // Маппинг User в UserProfileDto
            var userProfileDto = new UserProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = (currentUserId == user.Id) ? user.Email : "Приватный", // Показываем email только владельцу профиля
                ProfilePictureUrl = user.ProfilePictureUrl,
                Bio = user.Bio,
                RegistrationDate = user.RegistrationDate,
                PostsCount = user.Posts.Count,
                FollowersCount = user.Followers.Count,
                FollowingCount = user.Following.Count,
                IsFollowing = isFollowing,
                IsMyProfile = (currentUserId == user.Id)
            };

            return Ok(userProfileDto);
        }

        // PUT: api/Users/{id}
        // Обновление собственного профиля (только для владельца)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserProfile(int id, [FromForm] UpdateUserProfileDto updateUserProfileDto)
        {
            var currentUserId = GetCurrentUserId();

            // Проверяем, что пользователь пытается обновить свой собственный профиль
            if (id != currentUserId)
            {
                return StatusCode(403, new { message = "Вы можете редактировать только свой профиль." }); // 403 Forbidden
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }

            // Обновляем биографию, если она предоставлена
            if (updateUserProfileDto.Bio != null)
            {
                user.Bio = updateUserProfileDto.Bio;
            }

            // Обработка загрузки нового изображения профиля
            if (updateUserProfileDto.ProfilePictureFile != null && updateUserProfileDto.ProfilePictureFile.Length > 0)
            {
                // Удаляем старое изображение, если оно есть
                if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                {
                    var oldImagePath = Path.Combine(_env.WebRootPath, user.ProfilePictureUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Сохраняем новое изображение
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "profiles"); // Можно создать отдельную папку
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(updateUserProfileDto.ProfilePictureFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await updateUserProfileDto.ProfilePictureFile.CopyToAsync(fileStream);
                }

                user.ProfilePictureUrl = $"/images/profiles/{uniqueFileName}";
            }
            // Если ProfilePictureFile равен null, это может означать, что пользователь хочет удалить аватарку
            // Добавим логику для удаления аватарки, если передана специальная метка (например, пустая строка или null)
            // Пока оставим без такой логики, если файл не предоставлен, то просто не обновляем

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Users.AnyAsync(e => e.Id == id))
                {
                    return NotFound("Профиль не найден после попытки обновления.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // 204 No Content
        }
    }
}
