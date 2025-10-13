using FlashcardsApp.Models;
using FlashcardsAppContracts.Constants;
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
                TotalStudyTime = TimeSpan.FromHours(12.5)
            };
            context.UserStatistics.Add(statistics);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ User statistics created!");
        }

        // 2. Создать достижения с Tailwind градиентами
        if (!await context.Achievements.AnyAsync())
        {
            var achievements = new List<Achievement>
            {
                new Achievement
                {
                    Id = Guid.NewGuid(), 
                    Name = "7 дней подряд", 
                    Description = "Занимайтесь 7 дней подряд", 
                    IconUrl = "🔥",
                    Gradient = "from-orange-400 to-red-500"
                },
                new Achievement
                {
                    Id = Guid.NewGuid(), 
                    Name = "Первые шаги", 
                    Description = "Завершите свой первый урок", 
                    IconUrl = "⭐",
                    Gradient = "from-yellow-400 to-orange-500"
                },
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Высший балл",
                    Description = "Получите 100% выигрыша в викторине",
                    IconUrl = "🎯",
                    Gradient = "from-green-400 to-emerald-500"
                },
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Быстро обучающийся",
                    Description = "Пройдите 10 уроков за один день",
                    IconUrl = "🏅",
                    Gradient = "from-blue-400 to-purple-500"
                },
                new Achievement
                {
                    Id = Guid.NewGuid(), 
                    Name = "Король знаний", 
                    Description = "Достигните 10-го уровня", 
                    IconUrl = "👑",
                    Gradient = "from-purple-400 to-pink-500"
                },
                new Achievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Восходящая звезда",
                    Description = "Заработайте 1000 очков опыта",
                    IconUrl = "🚀",
                    Gradient = "from-cyan-400 to-blue-500"
                }
            };

            context.Achievements.AddRange(achievements);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Achievements created!");

            // Разблокировать первые 3 достижения для тестового пользователя
            var userAchievements = new List<UserAchievement>
            {
                new UserAchievement
                {
                    UserId = userId, 
                    AchievementId = achievements[0].Id, // 7 дней подряд
                    UnlockedAt = DateTime.UtcNow.AddDays(-10)
                },
                new UserAchievement
                {
                    UserId = userId, 
                    AchievementId = achievements[1].Id, // Первые шаги
                    UnlockedAt = DateTime.UtcNow.AddDays(-3)
                },
                new UserAchievement
                {
                    UserId = userId, 
                    AchievementId = achievements[2].Id, // Высший балл
                    UnlockedAt = DateTime.UtcNow.AddDays(-1)
                }
            };

            context.UserAchievements.AddRange(userAchievements);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ User achievements unlocked!");
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