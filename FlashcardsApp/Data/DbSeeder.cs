using FlashcardsApp.Models;
using FlashcardsAppContracts.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<User> userManager)
    {
        // 1. Создать тестового пользователя
        if (!await context.Users.AnyAsync())
        {
            var testUser = new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), // фиксированный ID
                UserName = "test@test.com",
                Email = "test@test.com",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(testUser, "Test123!");

            // Создать статистику для тестового пользователя
            var statistics = new UserStatistics
            {
                UserId = testUser.Id,
                TotalXP = 650,
                CurrentStreak = 7,
                BestStreak = 10,
                LastStudyDate = DateTime.UtcNow,
                TotalStudyTime = TimeSpan.FromHours(2.5)
            };
            context.UserStatistics.Add(statistics);
        }

        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // 2. Создать достижения
        if (!await context.Achievements.AnyAsync())
        {
            var achievements = new List<Achievement>
            {
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Первые шаги",
                    Description = "Завершите свой первый урок",
                    IconUrl = "🔥"
                },
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "7 дней подряд",
                    Description = "Занимайтесь 7 дней подряд",
                    IconUrl = "⭐"
                },
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Высший балл",
                    Description = "Получите 100% выигрыша в викторине",
                    IconUrl = "🎯"
                },
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Быстро обучающийся",
                    Description = "Пройдите 10 уроков за один день",
                    IconUrl = "🏆"
                },
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Король знаний",
                    Description = "Достигните 10-го уровня",
                    IconUrl = "👑"
                },
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Восходящая звезда",
                    Description = "Заработайте 1000 очков опыта",
                    IconUrl = "🚀"
                }
            };

            context.Achievements.AddRange(achievements);

            // Разблокировать первые 3 достижения для тестового пользователя
            var firstAchievement = achievements[0];
            var secondAchievement = achievements[1];
            var thirdAchievement = achievements[2];

            context.UserAchievements.AddRange(
                new UserAchievement
                {
                    UserId = userId,
                    AchievementId = firstAchievement.Id,
                    UnlockedAt = DateTime.UtcNow.AddDays(-10)
                },
                new UserAchievement
                {
                    UserId = userId,
                    AchievementId = secondAchievement.Id,
                    UnlockedAt = DateTime.UtcNow.AddDays(-3)
                },
                new UserAchievement
                {
                    UserId = userId,
                    AchievementId = thirdAchievement.Id,
                    UnlockedAt = DateTime.UtcNow.AddDays(-1)
                }
            );
        }

        // 3. Создать группы
        if (!await context.Groups.AnyAsync())
        {
            var colors = new[] { GroupColor.Red, GroupColor.Green, GroupColor.Yellow, GroupColor.Orange, GroupColor.Purple };
            var groups = new List<Group>();

            for (int i = 1; i <= 5; i++)
            {
                var group = new Group
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    GroupName = $"Group {i}",
                    GroupColor = colors[i - 1],
                    CreatedAt = DateTime.UtcNow.AddDays(-i),
                    Order = i
                };
                groups.Add(group);
            }

            context.Groups.AddRange(groups);
            await context.SaveChangesAsync(); // Сохранить группы перед созданием карточек

            // 4. Создать карточки для каждой группы
            foreach (var group in groups)
            {
                var cards = new List<Card>();
                for (int j = 1; j <= 15; j++)
                {
                    var card = new Card
                    {
                        CardId = Guid.NewGuid(),
                        UserId = userId,
                        GroupId = group.Id,
                        Question = $"Question {j} for {group.GroupName}",
                        Answer = $"Answer {j} for {group.GroupName}",
                        CreatedAt = DateTime.UtcNow.AddDays(-j),
                        UpdatedAt = DateTime.UtcNow.AddDays(-j)
                    };
                    cards.Add(card);
                }
                context.Cards.AddRange(cards);
            }
        }

        await context.SaveChangesAsync();
    }
}