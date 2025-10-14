using FlashcardsApp.Models;
using FlashcardsAppContracts.Constants;
using FlashcardsAppContracts.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Data;

public static class DbSeeder
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

        // 2. Создать достижения
        if (!await context.Achievements.AnyAsync())
        {
            var achievements = new List<Achievement>
            {
                // === КАРТОЧКИ - ИЗУЧЕНИЕ ===
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Первые шаги",
                    Description = "Изучите 10 карточек",
                    IconUrl = "⭐",
                    Gradient = "from-yellow-400 to-orange-500",
                    ConditionType = AchievementConditionType.TotalCardsStudied,
                    ConditionValue = 10,
                    Rarity = AchievementRarity.Common,
                    DisplayOrder = 1,
                    IsActive = true
                },
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Ученик",
                    Description = "Изучите 50 карточек",
                    IconUrl = "📚",
                    Gradient = "from-blue-400 to-blue-600",
                    ConditionType = AchievementConditionType.TotalCardsStudied,
                    ConditionValue = 50,
                    Rarity = AchievementRarity.Rare,
                    DisplayOrder = 2,
                    IsActive = true
                },
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Мастер обучения",
                    Description = "Изучите 100 карточек",
                    IconUrl = "🎓",
                    Gradient = "from-indigo-400 to-purple-600",
                    ConditionType = AchievementConditionType.TotalCardsStudied,
                    ConditionValue = 100,
                    Rarity = AchievementRarity.Epic,
                    DisplayOrder = 3,
                    IsActive = true
                },
                
                // === КАРТОЧКИ - СОЗДАНИЕ ===
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Создатель",
                    Description = "Создайте 20 карточек",
                    IconUrl = "✍️",
                    Gradient = "from-green-400 to-teal-500",
                    ConditionType = AchievementConditionType.TotalCardsCreated,
                    ConditionValue = 20,
                    Rarity = AchievementRarity.Common,
                    DisplayOrder = 10,
                    IsActive = true
                },
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Архитектор знаний",
                    Description = "Создайте 50 карточек",
                    IconUrl = "🏗️",
                    Gradient = "from-emerald-400 to-green-600",
                    ConditionType = AchievementConditionType.TotalCardsCreated,
                    ConditionValue = 50,
                    Rarity = AchievementRarity.Rare,
                    DisplayOrder = 11,
                    IsActive = true
                },
                
                // === STREAK ===
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "7 дней подряд",
                    Description = "Занимайтесь 7 дней подряд",
                    IconUrl = "🔥",
                    Gradient = "from-orange-400 to-red-500",
                    ConditionType = AchievementConditionType.CurrentStreak,
                    ConditionValue = 7,
                    Rarity = AchievementRarity.Rare,
                    DisplayOrder = 20,
                    IsActive = true
                },
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Месяц силы",
                    Description = "Занимайтесь 30 дней подряд",
                    IconUrl = "🔥",
                    Gradient = "from-red-500 to-red-700",
                    ConditionType = AchievementConditionType.CurrentStreak,
                    ConditionValue = 30,
                    Rarity = AchievementRarity.Epic,
                    DisplayOrder = 21,
                    IsActive = true
                },
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Легенда постоянства",
                    Description = "Лучший streak 30 дней",
                    IconUrl = "🏆",
                    Gradient = "from-amber-400 to-orange-600",
                    ConditionType = AchievementConditionType.BestStreak,
                    ConditionValue = 30,
                    Rarity = AchievementRarity.Epic,
                    DisplayOrder = 22,
                    IsActive = true
                },
                
                // === УРОВЕНЬ ===
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Быстро обучающийся",
                    Description = "Достигните 5 уровня",
                    IconUrl = "🏅",
                    Gradient = "from-blue-400 to-purple-500",
                    ConditionType = AchievementConditionType.Level,
                    ConditionValue = 5,
                    Rarity = AchievementRarity.Common,
                    DisplayOrder = 30,
                    IsActive = true
                },
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Король знаний",
                    Description = "Достигните 10 уровня",
                    IconUrl = "👑",
                    Gradient = "from-purple-400 to-pink-500",
                    ConditionType = AchievementConditionType.Level,
                    ConditionValue = 10,
                    Rarity = AchievementRarity.Epic,
                    DisplayOrder = 31,
                    IsActive = true
                },
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Гроссмейстер",
                    Description = "Достигните 25 уровня",
                    IconUrl = "💎",
                    Gradient = "from-cyan-400 to-blue-600",
                    ConditionType = AchievementConditionType.Level,
                    ConditionValue = 25,
                    Rarity = AchievementRarity.Legendary,
                    DisplayOrder = 32,
                    IsActive = true
                },
                
                // === XP ===
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Высший балл",
                    Description = "Наберите 500 XP",
                    IconUrl = "🎯",
                    Gradient = "from-green-400 to-emerald-500",
                    ConditionType = AchievementConditionType.TotalXP,
                    ConditionValue = 500,
                    Rarity = AchievementRarity.Common,
                    DisplayOrder = 40,
                    IsActive = true
                },
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Восходящая звезда",
                    Description = "Наберите 1000 XP",
                    IconUrl = "🚀",
                    Gradient = "from-cyan-400 to-blue-500",
                    ConditionType = AchievementConditionType.TotalXP,
                    ConditionValue = 1000,
                    Rarity = AchievementRarity.Rare,
                    DisplayOrder = 41,
                    IsActive = true
                },
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Мастер опыта",
                    Description = "Наберите 5000 XP",
                    IconUrl = "⚡",
                    Gradient = "from-yellow-400 to-orange-500",
                    ConditionType = AchievementConditionType.TotalXP,
                    ConditionValue = 5000,
                    Rarity = AchievementRarity.Epic,
                    DisplayOrder = 42,
                    IsActive = true
                },
                
                // === ПЕРФЕКЦИОНИЗМ ===
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Перфекционист",
                    Description = "10 карточек подряд с оценкой 5",
                    IconUrl = "💯",
                    Gradient = "from-pink-400 to-rose-500",
                    ConditionType = AchievementConditionType.PerfectRatingsStreak,
                    ConditionValue = 10,
                    Rarity = AchievementRarity.Rare,
                    DisplayOrder = 50,
                    IsActive = true
                },
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Безупречность",
                    Description = "25 карточек подряд с оценкой 5",
                    IconUrl = "✨",
                    Gradient = "from-purple-400 to-pink-600",
                    ConditionType = AchievementConditionType.PerfectRatingsStreak,
                    ConditionValue = 25,
                    Rarity = AchievementRarity.Epic,
                    DisplayOrder = 51,
                    IsActive = true
                }
            };

            context.Achievements.AddRange(achievements);
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ {achievements.Count} achievements created!");

            // Разблокировать достижения для тестового пользователя на основе его статистики
            var stats = await context.UserStatistics.FirstAsync(s => s.UserId == userId);
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

        // 3. Создать группы
        if (!await context.Groups.AnyAsync())
        {
            var groupsData = new[]
            {
                new { Name = "Английский язык", Color = GroupColor.Red },
                new { Name = "Программирование", Color = GroupColor.Green },
                new { Name = "История", Color = GroupColor.Yellow },
                new { Name = "Математика", Color = GroupColor.Orange },
                new { Name = "Биология", Color = GroupColor.Purple }
            };

            var groups = new List<Group>();

            for (int i = 0; i < groupsData.Length; i++)
            {
                var group = new Group
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    GroupName = groupsData[i].Name,
                    GroupColor = groupsData[i].Color,
                    CreatedAt = DateTime.UtcNow.AddDays(-(i + 1)),
                    Order = i + 1
                };
                groups.Add(group);
            }

            context.Groups.AddRange(groups);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Groups created!");

            // 4. Создать карточки для каждой группы
            var cardsData = new Dictionary<string, List<(string Question, string Answer)>>
            {
                ["Английский язык"] = new()
                {
                    ("Как сказать 'привет' по-английски?", "Hello"),
                    ("Перевод слова 'book'", "Книга"),
                    ("Как сказать 'спасибо'?", "Thank you"),
                    ("Перевод 'water'", "Вода"),
                    ("Как спросить 'Как дела?'", "How are you?")
                },
                ["Программирование"] = new()
                {
                    ("Что такое переменная?", "Контейнер для хранения данных"),
                    ("Что означает ООП?", "Объектно-ориентированное программирование"),
                    ("Что такое функция?", "Блок кода, который можно переиспользовать"),
                    ("Что такое массив?", "Упорядоченная коллекция элементов"),
                    ("Что такое цикл?", "Конструкция для повторения кода")
                },
                ["История"] = new()
                {
                    ("В каком году открыли Америку?", "1492"),
                    ("Кто был первым президентом США?", "Джордж Вашингтон"),
                    ("Когда началась Вторая мировая война?", "1 сентября 1939"),
                    ("Столица Древнего Рима?", "Рим"),
                    ("Когда пала Римская империя?", "476 год н.э.")
                },
                ["Математика"] = new()
                {
                    ("Чему равно π?", "≈3.14159"),
                    ("Что такое производная?", "Скорость изменения функции"),
                    ("Теорема Пифагора?", "a² + b² = c²"),
                    ("Что такое интеграл?", "Площадь под кривой"),
                    ("Чему равен sin(90°)?", "1")
                },
                ["Биология"] = new()
                {
                    ("Что такое ДНК?", "Дезоксирибонуклеиновая кислота"),
                    ("Сколько хромосом у человека?", "46 (23 пары)"),
                    ("Что такое фотосинтез?", "Процесс преобразования света в энергию"),
                    ("Основная единица жизни?", "Клетка"),
                    ("Что такое митоз?", "Деление клетки")
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

        Console.WriteLine("=== SEED COMPLETED SUCCESSFULLY ===");
    }
}