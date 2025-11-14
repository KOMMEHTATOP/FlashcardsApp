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
                Role = "Admin",
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

        // 3. Создать группы и карточки из JSON
        if (!await context.Groups.AnyAsync(g => g.UserId == userId))
        {
            await SeedGroupsAndCardsFromJsonAsync(context, userId);
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
            var assembly = typeof(SeedManager).Assembly;
            var resourceName = "FlashcardsApp.DAL.Seeds.achievements.json";

            await using var stream = assembly.GetManifestResourceStream(resourceName);
            
            if (stream == null)
            {
                Console.WriteLine($"⚠️ Embedded resource not found: {resourceName}");
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
            
            Console.WriteLine($"✅ {achievements.Count} achievements loaded!");
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
            Console.WriteLine($"✅ {unlockedAchievements.Count} achievements unlocked!");
        }
    }

    /// <summary>
    /// Загрузить группы и карточки из JSON файлов
    /// </summary>
    private static async Task SeedGroupsAndCardsFromJsonAsync(ApplicationDbContext context, Guid userId)
    {
        try
        {
            var assembly = typeof(SeedManager).Assembly;
            
            // Загрузить группы
            var groupsResourceName = "FlashcardsApp.DAL.Seeds.groups.json";
            await using var groupsStream = assembly.GetManifestResourceStream(groupsResourceName);
            
            if (groupsStream == null)
            {
                Console.WriteLine($"⚠️ Groups resource not found: {groupsResourceName}");
                return;
            }

            using var groupsReader = new StreamReader(groupsStream);
            var groupsJson = await groupsReader.ReadToEndAsync();
            
            var groupsData = JsonSerializer.Deserialize<GroupsJsonData>(groupsJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (groupsData?.Groups == null || !groupsData.Groups.Any())
            {
                Console.WriteLine("⚠️ No groups found in JSON file");
                return;
            }

            // Создать группы
            var groups = groupsData.Groups.Select((g, index) => new Group
            {
                Id = Guid.Parse(g.Id),
                UserId = userId,
                GroupName = g.Name,
                GroupColor = g.Color,
                GroupIcon = g.Icon,
                IsPublished = g.IsPublished,
                CreatedAt = DateTime.UtcNow.AddDays(-(index + 1)),
                Order = g.Order
            }).ToList();

            context.Groups.AddRange(groups);
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ {groups.Count} groups created!");

            // Загрузить карточки
            var cardsResourceName = "FlashcardsApp.DAL.Seeds.cards.json";
            await using var cardsStream = assembly.GetManifestResourceStream(cardsResourceName);
            
            if (cardsStream == null)
            {
                Console.WriteLine($"⚠️ Cards resource not found: {cardsResourceName}");
                return;
            }

            using var cardsReader = new StreamReader(cardsStream);
            var cardsJson = await cardsReader.ReadToEndAsync();
            
            var cardsData = JsonSerializer.Deserialize<CardsJsonData>(cardsJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (cardsData?.Cards == null)
            {
                Console.WriteLine("⚠️ No cards found in JSON file");
                return;
            }

            // Создать карточки для каждой группы
            int totalCards = 0;
            foreach (var group in groups)
            {
                if (cardsData.Cards.TryGetValue(group.Id.ToString(), out var groupCards))
                {
                    var cardEntities = groupCards.Select((card, index) => new Card
                    {
                        CardId = Guid.NewGuid(),
                        GroupId = group.Id,
                        Question = card.Question,
                        Answer = card.Answer,
                        CreatedAt = DateTime.UtcNow.AddDays(-(index + 1)),
                        UpdatedAt = DateTime.UtcNow.AddDays(-(index + 1))
                    }).ToList();

                    context.Cards.AddRange(cardEntities);
                    totalCards += cardEntities.Count;
                }
            }

            await context.SaveChangesAsync();
            Console.WriteLine($"✅ {totalCards} cards created!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error loading groups and cards: {ex.Message}");
            throw;
        }
    }

    #region DTO для десериализации JSON

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

    private class GroupsJsonData
    {
        public List<GroupJsonDto> Groups { get; set; } = new();
    }

    private class GroupJsonDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public int Order { get; set; }
    }

    private class CardsJsonData
    {
        public Dictionary<string, List<CardJsonDto>> Cards { get; set; } = new();
    }

    private class CardJsonDto
    {
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
    }

    #endregion
}