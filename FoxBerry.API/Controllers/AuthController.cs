using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FoxBerry.API.DTOs;
using FoxBerry.API.Data;
using FoxBerry.API.Models;
using BCrypt.Net; // Для хеширования паролей
using Microsoft.EntityFrameworkCore; // Для работы с БД
using System.Security.Claims; // Для работы с утверждениями (Claims)
using System.IdentityModel.Tokens.Jwt; // Для создания JWT-токенов
using Microsoft.IdentityModel.Tokens; // Для SymmetricSecurityKey
using System.Text; // Для Encoding
using Microsoft.Extensions.Configuration; // Для доступа к конфигурации (JWT Secret)

namespace FoxBerry.API.Controllers
{
    [Route("api/[controller]")] // Базовый маршрут: /api/Auth
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration; // Для получения JWT Secret

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/Auth/register
        [HttpPost("register")] // Полный маршрут: /api/Auth/register
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // 1. Валидация входных данных (уже сделана атрибутами в RegisterDto)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Возвращаем ошибки валидации, если есть
            }

            // 2. Проверка на существующего пользователя по Username
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
            {
                return Conflict("Имя пользователя уже занято."); // 409 Conflict
            }

            // 3. Проверка на существующего пользователя по Email
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return Conflict("Email уже зарегистрирован."); // 409 Conflict
            }

            // 4. Хеширование пароля
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // 5. Создание нового объекта User
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                RegistrationDate = DateTime.UtcNow,
                Bio = "Привет! Я новый пользователь FoxBerry." // Дефолтная биография
            };

            // 6. Добавление пользователя в базу данных
            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // Сохраняем изменения асинхронно

            // 7. Возвращаем успешный ответ
            return Ok(new { Message = "Регистрация успешна!" }); // 200 OK
        }

        // POST: api/Auth/login
        [HttpPost("login")] // Полный маршрут: /api/Auth/login
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Поиск пользователя по имени пользователя или email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.UsernameOrEmail ||
                                        u.Email == loginDto.UsernameOrEmail);

            if (user == null)
            {
                return Unauthorized("Неверное имя пользователя/email или пароль."); // 401 Unauthorized
            }

            // 2. Проверка пароля (сравнение хеша)
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized("Неверное имя пользователя/email или пароль."); // 401 Unauthorized
            }

            // 3. Если пароль верен, генерируем JWT-токен
            var token = GenerateJwtToken(user);

            // 4. Возвращаем токен клиенту
            return Ok(new { Token = token, UserId = user.Id, Username = user.Username,  }); // 200 OK
        }

        // Метод для генерации JWT-токена
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured.");
            var expiryMinutes = Convert.ToDouble(jwtSettings["ExpiryMinutes"]);
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            // Утверждения (Claims) - информация о пользователе, которая будет закодирована в токене
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // ID пользователя
                new Claim(ClaimTypes.Name, user.Username), // Имя пользователя
                // Добавьте другие клеймы, если нужны роли, email и т.д.
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes), // Срок действия токена
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
