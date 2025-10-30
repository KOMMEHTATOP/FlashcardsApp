using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FlashcardsApp.DAL.Seeds;

public static class SeedManager
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<User> userManager)
    {
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // 1. Создать тестового пользователя
        var testUser = await userManager.FindByEmailAsync("test@test.com");

        if (testUser == null)
        {
            testUser = new User
            {
                Id = userId,
                Login = "testuser",
                UserName = "test@test.com",
                Email = "test@test.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(testUser, "Test123!");

            if (!result.Succeeded)
            {
                Console.WriteLine("Failed to create test user:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"  - {error.Description}");
                }
                return;
            }

            Console.WriteLine("✅ Test user created successfully!");

            // Создать статистику для тестового пользователя
            var statistics = new UserStatistics
            {
                UserId = testUser.Id,
                TotalXP = 650,
                Level = 5,
                CurrentStreak = 7,
                BestStreak = 10,
                LastStudyDate = DateTime.UtcNow,
                TotalStudyTime = TimeSpan.FromHours(12.5),
                TotalCardsStudied = 45,
                TotalCardsCreated = 25,
                PerfectRatingsStreak = 3
            };
            context.UserStatistics.Add(statistics);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ User statistics created!");
        }

        // 2. Загрузить достижения из JSON файла
        if (!await context.Achievements.AnyAsync())
        {
            await SeedAchievementsFromJsonAsync(context);
            
            // Разблокировать достижения на основе статистики
            await UnlockAchievementsForTestUser(context, userId);
        }

        // 3. Создать группы
        if (!await context.Groups.AnyAsync(g => g.UserId == userId))
        {
            await SeedGroupsAndCards(context, userId);
        }

        Console.WriteLine("=== SEED COMPLETED SUCCESSFULLY ===");
    }

    /// <summary>
    /// Загрузить достижения из JSON файла
    /// </summary>
private static async Task SeedAchievementsFromJsonAsync(ApplicationDbContext context)
{
    try
    {
        // Читаем из Embedded Resource
        var assembly = typeof(SeedManager).Assembly;
        var resourceName = "FlashcardsApp.DAL.Seeds.achievements.json";

        await using var stream = assembly.GetManifestResourceStream(resourceName);
        
        if (stream == null)
        {
            Console.WriteLine($"⚠️ Embedded resource not found: {resourceName}");
            Console.WriteLine("Available resources:");
            foreach (var name in assembly.GetManifestResourceNames())
            {
                Console.WriteLine($"  - {name}");
            }
            return;
        }

        using var reader = new StreamReader(stream);
        var jsonContent = await reader.ReadToEndAsync();
        
        var achievementsData = JsonSerializer.Deserialize<AchievementsJsonData>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (achievementsData?.Achievements == null || !achievementsData.Achievements.Any())
        {
            Console.WriteLine("⚠️ No achievements found in JSON file");
            return;
        }

        var achievements = achievementsData.Achievements.Select(a => new Achievement
        {
            Id = Guid.Parse(a.Id),
            Name = a.Name,
            Description = a.Description,
            IconUrl = a.IconUrl,
            Gradient = a.Gradient,
            ConditionType = (AchievementConditionType)a.ConditionType,
            ConditionValue = a.ConditionValue,
            Rarity = (AchievementRarity)a.Rarity,
            DisplayOrder = a.DisplayOrder,
            IsActive = a.IsActive
        }).ToList();

        context.Achievements.AddRange(achievements);
        await context.SaveChangesAsync();
        
        Console.WriteLine($"✅ {achievements.Count} achievements loaded from embedded resource!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error loading achievements: {ex.Message}");
        throw;
    }
}
    
    /// <summary>
    /// Разблокировать достижения для тестового пользователя
    /// </summary>
    private static async Task UnlockAchievementsForTestUser(ApplicationDbContext context, Guid userId)
    {
        var stats = await context.UserStatistics.FirstAsync(s => s.UserId == userId);
        var achievements = await context.Achievements.Where(a => a.IsActive).ToListAsync();
        var unlockedAchievements = new List<UserAchievement>();

        foreach (var achievement in achievements)
        {
            bool shouldUnlock = achievement.ConditionType switch
            {
                AchievementConditionType.TotalCardsStudied => stats.TotalCardsStudied >= achievement.ConditionValue,
                AchievementConditionType.TotalCardsCreated => stats.TotalCardsCreated >= achievement.ConditionValue,
                AchievementConditionType.CurrentStreak => stats.CurrentStreak >= achievement.ConditionValue,
                AchievementConditionType.BestStreak => stats.BestStreak >= achievement.ConditionValue,
                AchievementConditionType.Level => stats.Level >= achievement.ConditionValue,
                AchievementConditionType.TotalXP => stats.TotalXP >= achievement.ConditionValue,
                AchievementConditionType.PerfectRatingsStreak => stats.PerfectRatingsStreak >= achievement.ConditionValue,
                _ => false
            };

            if (shouldUnlock)
            {
                unlockedAchievements.Add(new UserAchievement
                {
                    UserId = userId,
                    AchievementId = achievement.Id,
                    UnlockedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 10))
                });
            }
        }

        if (unlockedAchievements.Any())
        {
            context.UserAchievements.AddRange(unlockedAchievements);
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ {unlockedAchievements.Count} achievements unlocked for test user!");
        }
    }

    /// <summary>
    /// Создать группы и карточки для тестирования
    /// </summary>
    private static async Task SeedGroupsAndCards(ApplicationDbContext context, Guid userId)
    {
        var groupsData = new[]
        {
            new { Name = "Английский язык", Color = "from-green-500 to-emerald-500" },
            new { Name = "Программирование", Color = "from-orange-500 to-yellow-500" },
            new { Name = "История", Color = "from-purple-500 to-pink-500" }
        };

        var groups = new List<Group>();

        for (int i = 0; i < groupsData.Length; i++)
        {
            var group = new Group
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                GroupName = groupsData[i].Name,
                GroupIcon = "",
                GroupColor = groupsData[i].Color,
                CreatedAt = DateTime.UtcNow.AddDays(-(i + 1)),
                Order = i + 1
            };
            groups.Add(group);
        }

        context.Groups.AddRange(groups);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Groups created!");

        // Создать карточки для каждой группы
        var cardsData = new Dictionary<string, List<(string Question, string Answer)>>
        {
            ["Английский язык"] = new()
            {
                ("Как сказать 'привет' по-английски?", "Hello"),
                ("Перевод слова 'book'", "Книга"),
                ("Как сказать 'спасибо'?", "Thank you")
            },
            ["Программирование"] = new()
            {
                ("Что такое переменная?", "Контейнер для хранения данных"),
                ("Что означает ООП?", "Объектно-ориентированное программирование"),
                ("Что такое функция?", "Блок кода, который можно переиспользовать")
            },
            ["История"] = new()
            {
                ("В каком году открыли Америку?", "1492"),
                ("Кто был первым президентом США?", "Джордж Вашингтон"),
                ("Когда началась Вторая мировая война?", "1 сентября 1939")
            }
        };

        foreach (var group in groups)
        {
            if (cardsData.TryGetValue(group.GroupName, out var cards))
            {
                var cardEntities = cards.Select((card, index) => new Card
                {
                    CardId = Guid.NewGuid(),
                    UserId = userId,
                    GroupId = group.Id,
                    Question = card.Question,
                    Answer = card.Answer,
                    CreatedAt = DateTime.UtcNow.AddDays(-(index + 1)),
                    UpdatedAt = DateTime.UtcNow.AddDays(-(index + 1))
                }).ToList();

                context.Cards.AddRange(cardEntities);
            }
        }

        await context.SaveChangesAsync();
        Console.WriteLine("✅ Cards created!");
    }

    #region DTO для десериализации JSON

    /// <summary>
    /// DTO для десериализации JSON файла с достижениями
    /// </summary>
    private class AchievementsJsonData
    {
        public List<AchievementJsonDto> Achievements { get; set; } = new();
    }

    private class AchievementJsonDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public string Gradient { get; set; } = string.Empty;
        public int ConditionType { get; set; }
        public int ConditionValue { get; set; }
        public int Rarity { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    #endregion
}