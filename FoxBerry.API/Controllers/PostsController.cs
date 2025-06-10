using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoxBerry.API.Data;
using FoxBerry.API.Models;
using FoxBerry.API.DTOs;
using Microsoft.AspNetCore.Authorization; // Для атрибута [Authorize]
using System.Security.Claims; // Для получения ID пользователя из токена

namespace FoxBerry.API.Controllers
{
    [Route("api/[controller]")] // Базовый маршрут: /api/Posts
    [ApiController]
    [Authorize] // Все методы в этом контроллере требуют аутентификации
    public class PostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env; // Для работы с файловой системой

        public PostsController(ApplicationDbContext context, IWebHostEnvironment env)
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

        // POST: api/Posts
        // Создание нового поста
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostDto createPostDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId(); // Получаем ID текущего пользователя из токена

            // 1. Сохранение изображения
            string imageUrl = string.Empty;
            if (createPostDto.ImageFile != null && createPostDto.ImageFile.Length > 0)
            {
                // Убедимся, что папка для изображений существует
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "posts");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Генерируем уникальное имя файла
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + createPostDto.ImageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Сохраняем файл на сервере
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await createPostDto.ImageFile.CopyToAsync(fileStream);
                }

                // Формируем URL для доступа к изображению
                imageUrl = $"/images/{uniqueFileName}";
            }
            else
            {
                return BadRequest("Файл изображения отсутствует или поврежден.");
            }

            // 2. Создание объекта Post
            var post = new Post
            {
                AuthorId = userId,
                ImageUrl = imageUrl,
                Caption = createPostDto.Caption,
                Location = createPostDto.Location,
                UploadDate = DateTime.UtcNow
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, new { Message = "Пост успешно создан!" });
        }

        // GET: api/Posts/{id}
        // Получение поста по ID
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPostById(int id)
        {
            var currentUserId = GetCurrentUserId(); // Получаем ID текущего пользователя

            var post = await _context.Posts
                .Include(p => p.Author) // Загружаем данные автора
                .Include(p => p.Likes) // Загружаем данные о лайках
                .Include(p => p.Comments) // Загружаем данные о комментариях
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound("Пост не найден.");
            }

            // Маппинг Post в PostDto
            var postDto = new PostDto
            {
                Id = post.Id,
                AuthorId = post.AuthorId,
                AuthorUsername = post.Author?.Username ?? "Неизвестно", // Используем null-conditional operator
                ImageUrl = post.ImageUrl,
                Caption = post.Caption,
                Location = post.Location,
                UploadDate = post.UploadDate,
                LikesCount = post.Likes.Count,
                CommentsCount = post.Comments.Count,
                IsLikedByCurrentUser = post.Likes.Any(l => l.UserId == currentUserId) // Проверяем, лайкнул ли текущий пользователь
            };

            return Ok(postDto);
        }

        // GET: api/Posts/feed
        // Получение ленты постов (посты всех пользователей, на которых подписан текущий пользователь)
        [HttpGet("feed")]
        public async Task<ActionResult<PostDto>> GetUserFeed()
        {
            var currentUserId = GetCurrentUserId();

            // Получаем список пользователей, на которых подписан текущий пользователь
            var followedUserIds = await _context.Follows
                .Where(f => f.FollowerId == currentUserId)
                .Select(f => f.FollowingId)
                .ToListAsync();

            // Включаем посты самого пользователя в ленту
            followedUserIds.Add(currentUserId);

            // Получаем посты от этих пользователей, отсортированные по дате загрузки
            var posts = await _context.Posts
                .Where(p => followedUserIds.Contains(p.AuthorId))
                .OrderByDescending(p => p.UploadDate) // Сортируем по убыванию даты
                .Include(p => p.Author)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Select(p => new PostDto // Выбираем только необходимые данные для DTO
                {
                    Id = p.Id,
                    AuthorId = p.AuthorId,
                    AuthorUsername = p.Author!.Username, // Здесь мы уверены, что Author не будет null благодаря Include
                    ImageUrl = p.ImageUrl,
                    Caption = p.Caption,
                    Location = p.Location,
                    UploadDate = p.UploadDate,
                    LikesCount = p.Likes.Count,
                    CommentsCount = p.Comments.Count,
                    IsLikedByCurrentUser = p.Likes.Any(l => l.UserId == currentUserId)
                })
                .ToListAsync();

            return Ok(posts);
        }

        // PUT: api/Posts/{id}
        // Обновление поста (только автором поста)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostDto updatePostDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();

            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound("Пост не найден.");
            }

            // Проверяем, является ли текущий пользователь автором поста
            if (post.AuthorId != userId)
            {
                return StatusCode(403, new { message = "У вас нет прав для редактирования этого поста." }); // 403 Forbidden
            }

            post.Caption = updatePostDto.Caption ?? post.Caption; // Обновляем подпись, если она предоставлена
            post.Location = updatePostDto.Location ?? post.Location; // Обновляем местоположение, если оно предоставлено
            
            _context.Entry(post).State = EntityState.Modified; // Указываем EF Core, что объект был изменен

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) // Обработка конфликтов параллельного доступа
            {
                if (!await _context.Posts.AnyAsync(e => e.Id == id))
                {
                    return NotFound("Пост не найден после попытки обновления.");
                }
                else
                {
                    throw; // Другой тип ошибки
                }
            }

            return NoContent(); // 204 No Content - успешное выполнение без возврата тела
        }

        // DELETE: api/Posts/{id}
        // Удаление поста (только автором поста)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = GetCurrentUserId();

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound("Пост не найден.");
            }

            // Проверяем, является ли текущий пользователь автором поста
            if (post.AuthorId != userId)
            {
                return StatusCode(403, new { message = "У вас нет прав для удаления этого поста." }); // 403 Forbidden
            }

            // Удаляем файл изображения с сервера
            if (!string.IsNullOrEmpty(post.ImageUrl))
            {
                var imagePath = Path.Combine(_env.WebRootPath, post.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }
    }
}
