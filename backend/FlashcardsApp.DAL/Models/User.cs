using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FlashcardsApp.DAL.Models;

public class User : IdentityUser<Guid>
{
    public required string Role { get; set; }
    
    [Required(ErrorMessage = "Логин обязателен")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Логин должен быть от 3 до 50 символов")]
    public required string Login { get; set; } 
    
    [Range(0, int.MaxValue, ErrorMessage = "Рейтинг не может быть отрицательным")]
    public int TotalRating { get; set; }
    public DateTime CreatedAt { get; set; } 
    public DateTime LastLogin { get; set; }
    
    // Навигационные свойства
    public List<StudyHistory>? StudyHistory { get; set; }
    public List<Group>? Groups { get; set; }
    public UserStatistics? Statistics { get; set; }
    public List<UserAchievement>? UserAchievements { get; set; }
    public List<UserGroupSubscription>? Subscriptions { get; set; }
}
