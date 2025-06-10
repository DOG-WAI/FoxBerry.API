using Microsoft.EntityFrameworkCore;
using FoxBerry.API.Models;

namespace FoxBerry.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Здесь будут определены свойства DbSet для каждой вашей модели.
         public DbSet<User> Users { get; set; } = null!;
         public DbSet<Post> Posts { get; set; } = null!;
         public DbSet<Comment> Comments { get; set; } = null!;
         public DbSet<Like> Likes { get; set; } = null!;
         public DbSet<Follow> Follows { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Здесь можно настроить отношения между моделями, индексы и другие детали схемы базы данных.
            // Настройка уникальности для Username и Email в таблице Users
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Гарантируем уникальность пары UserId и PostId для Like
            // Один пользователь может лайкнуть один пост только один раз
            modelBuilder.Entity<Like>()
                .HasIndex(l => new { l.UserId, l.PostId })
                .IsUnique();

            // Гарантируем уникальность пары FollowerId и FollowingId для Follow
            // Один пользователь может подписаться на другого только один раз
            modelBuilder.Entity<Follow>()
                .HasIndex(f => new { f.FollowerId, f.FollowingId })
                .IsUnique();

            // Настройка отношений для Follows, чтобы избежать циклических ссылок
            // и явно указать, какие навигационные свойства к каким foreign key относятся.
            // Это важно, когда у вас две связи между одними и теми же таблицами (User и User).
            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.Following) // Я подписан на кого-то
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict); // Не удалять пользователя при удалении подписки

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Following)
                .WithMany(u => u.Followers) // На меня подписан кто-то
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.Restrict); // Не удалять пользователя при удалении подписки
        }
    }
}
