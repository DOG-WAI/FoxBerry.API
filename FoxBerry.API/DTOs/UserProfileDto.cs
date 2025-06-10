using System;
using System.Collections.Generic; // Добавьте для ICollection

namespace FoxBerry.API.DTOs
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty; // Можно включить, если профиль для себя
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int PostsCount { get; set; } // Количество постов пользователя
        public int FollowersCount { get; set; } // Количество подписчиков
        public int FollowingCount { get; set; } // Количество подписок
        public bool IsFollowing { get; set; } // Подписан ли текущий пользователь на этот профиль
        public bool IsMyProfile { get; set; } // Это мой профиль?
    }
}
