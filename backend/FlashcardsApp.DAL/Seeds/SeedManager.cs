using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FlashcardsApp.DAL.Seeds;

public static class SeedManager
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<User> userManager)
    {
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // 1. Reference Data — справочники
        if (!await context.Achievements.AnyAsync())
        {
            await SeedAchievementsFromJsonAsync(context);
        }

        // 2. Test User
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
                    Console.WriteLine($"  - {error.Description}");
                return;
            }
            Console.WriteLine("✅ Test user created!");
        }

        // 3. User Statistics — инициализация
        if (!await context.UserStatistics.AnyAsync(s => s.UserId == userId))
        {
            var statistics = new UserStatistics
            {
                UserId = userId,
                TotalXP = 0,
                Level = 1,
                CurrentStreak = 0,
                BestStreak = 0,
                LastStudyDate = DateTime.UtcNow,
                TotalStudyTime = TimeSpan.Zero,
                TotalCardsStudied = 0,
                TotalCardsCreated = 0,
                PerfectRatingsStreak = 0
            };
            context.UserStatistics.Add(statistics);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ User statistics initialized!");
        }

        // 4. Test Content — группы и карточки
        if (!await context.Groups.AnyAsync(g => g.UserId == userId))
        {
            var cardsCreated = await SeedGroupsAndCardsFromJsonAsync(context, userId);

            // Обновляем TotalCardsCreated
            var stats = await context.UserStatistics.FirstAsync(s => s.UserId == userId);
            stats.TotalCardsCreated = cardsCreated;
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ TotalCardsCreated updated to {cardsCreated}");
        }

        Console.WriteLine("=== SEED COMPLETED ===");
    }

    /// <summary>
    /// Проверка достижений после seed — вызывается из MiddlewareExtensions
    /// </summary>
    public static Guid GetTestUserId() => Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static async Task<int> SeedGroupsAndCardsFromJsonAsync(ApplicationDbContext context, Guid userId)
    {
        var assembly = typeof(SeedManager).Assembly;

        // Загружаем JSON с группами
        var groupsJson = await LoadEmbeddedJsonAsync(assembly, "FlashcardsApp.DAL.Seeds.groups.json");
        var groupsData = JsonSerializer.Deserialize<GroupsJsonData>(groupsJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (groupsData?.Groups == null) return 0;

        // Загружаем JSON с карточками
        var cardsJson = await LoadEmbeddedJsonAsync(assembly, "FlashcardsApp.DAL.Seeds.cards.json");
        var cardsData = JsonSerializer.Deserialize<CardsJsonData>(cardsJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (cardsData?.Cards == null) return 0;

        // Создаём группы
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

        // Создаём карточки
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

        return totalCards;
    }

    private static async Task<string> LoadEmbeddedJsonAsync(System.Reflection.Assembly assembly, string resourceName)
    {
        await using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            throw new FileNotFoundException($"Embedded resource not found: {resourceName}");

        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    private static async Task SeedAchievementsFromJsonAsync(ApplicationDbContext context)
    {
        try
        {
            var assembly = typeof(SeedManager).Assembly;
            var json = await LoadEmbeddedJsonAsync(assembly, "FlashcardsApp.DAL.Seeds.achievements.json");

            var data = JsonSerializer.Deserialize<AchievementsJsonData>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (data?.Achievements == null || !data.Achievements.Any())
            {
                Console.WriteLine("⚠️ No achievements found in JSON file");
                return;
            }

            var achievements = data.Achievements.Select(a => new Achievement
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

    #region JSON DTOs

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