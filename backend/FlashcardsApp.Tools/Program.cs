using System.Text.Json;
using FlashcardsApp.Tools;
using Microsoft.EntityFrameworkCore;
using Ookii.Dialogs.Wpf;
using FlashcardsApp.DAL;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Tools.Models.DTO;
using System.IO;
using Group = FlashcardsApp.DAL.Models.Group;

// --- КОНФИГУРАЦИЯ ---
// ID твоего Админа (от чьего имени создаются колоды)
var adminUserId = Guid.Parse("11111111-1111-1111-1111-111111111111"); 
// --------------------

Console.WriteLine("╔═══════════════════════════════════════════╗");
Console.WriteLine("║  Flashcards Admin Seeder (JSON Import)    ║");
Console.WriteLine("╚═══════════════════════════════════════════╝");

// 1. ВЫБОР СРЕДЫ
Console.WriteLine("\nSelect Environment:");
Console.WriteLine("  1. LOCALHOST (Port 5432)");
Console.WriteLine("  2. PRODUCTION (via SSH Tunnel on Port 5434)");
Console.Write("Choice: ");
var envChoice = Console.ReadLine();

string connectionString;
string dbPassword = "";

if (envChoice == "2")
{
    // --- ЛОГИКА ДЛЯ ПРОДАКШЕНА (Пункт 2) ---
    Console.WriteLine("\n⚠️  Ensure SSH Tunnel is open!");
    Console.WriteLine("   Run: ssh -L 5434:localhost:5432 root@78.140.246.181 -N");
    Console.WriteLine("   Press any key when ready...");
    Console.ReadKey();

    Console.Write("\n🔑 Enter PRODUCTION DB Password: "); // Запрашиваем пароль от Прода
    dbPassword = ReadPassword();
    
    // Порт 5434 (Туннель), База FlashcardsDb
    connectionString = $"Host=localhost;Port=5434;Database=FlashcardsDb;Username=postgres;Password={dbPassword}";
}
else
{
    // --- ЛОГИКА ДЛЯ ЛОКАЛКИ (Пункт 1) ---
    Console.Write("\n🔑 Enter LOCALHOST DB Password: "); // Запрашиваем пароль от Локалки (123)
    dbPassword = ReadPassword();

    // Порт 5432 (Стандартный), База FlashcardsDb (Исправлено имя!)
    connectionString = $"Host=localhost;Port=5432;Database=FlashcardsDb;Username=postgres;Password={dbPassword}";
}

Console.WriteLine($"\n🔌 Connection configured.");

// 2. ЗАПУСК ИМПОРТА
await ImportJsonToDatabase(connectionString);

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();


// --- ЛОГИКА ИМПОРТА ---

async Task ImportJsonToDatabase(string connString)
{
    Console.WriteLine("\n=== Select JSON File ===");
    string? jsonPath = SelectFile("JSON Files (*.json)|*.json");

    if (string.IsNullOrEmpty(jsonPath))
    {
        Console.WriteLine("❌ No file selected.");
        return;
    }

    Console.WriteLine($"📂 Reading: {Path.GetFileName(jsonPath)}");
    
    try 
    {
        var jsonString = await File.ReadAllTextAsync(jsonPath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var importGroups = JsonSerializer.Deserialize<List<ImportGroupDto>>(jsonString, options);

        if (importGroups == null || !importGroups.Any())
        {
            Console.WriteLine("❌ JSON is empty or invalid.");
            return;
        }

        Console.WriteLine($"📊 Found {importGroups.Count} groups to import.");
        Console.WriteLine("🔄 Connecting to Database...");
        
        var dbOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        dbOptionsBuilder.UseNpgsql(connString);

        using var context = new ApplicationDbContext(dbOptionsBuilder.Options);
        
        Console.Write("🔄 Testing connection... ");
        try 
        {
            await context.Database.OpenConnectionAsync(); // Пытаемся открыть
            await context.Database.CloseConnectionAsync(); // Если ок, закрываем
            Console.WriteLine("✅ OK!");
        }
        catch (Exception connEx)
        {
            Console.WriteLine();
            Console.WriteLine($"❌ CONNECTION FAILED: {connEx.Message}");

            if (connEx.InnerException != null)
            {
                Console.WriteLine($"   Inner: {connEx.InnerException.Message}");
            }
            return; // Останавливаемся
        }
        Console.WriteLine("✅ Connected.");

        Console.WriteLine("🚀 Starting import...");

        foreach (var dto in importGroups)
        {
            Console.Write($"   Processing '{dto.GroupName}'... ");

            // Работа с Тегами (Get or Create)
            var groupTags = new List<Tag>();
            if (dto.Tags != null)
            {
                foreach (var tagName in dto.Tags)
                {
                    var slug = HelpersMethods.GenerateSlug(tagName);
                    var existingTag = await context.Tags.FirstOrDefaultAsync(t => t.Slug == slug);

                    if (existingTag != null)
                    {
                        groupTags.Add(existingTag);
                    }
                    else
                    {
                        // Создаем новый тег с рандомным цветом
                        var newTag = new Tag
                        {
                            Id = Guid.NewGuid(),
                            Name = tagName.Trim(),
                            Slug = slug,
                            Color = HelpersMethods.GetRandomTagColor() 
                        };
                        context.Tags.Add(newTag);
                        groupTags.Add(newTag);
                    }
                }
            }

            // Создание Группы
            var newGroup = new Group
            {
                Id = Guid.NewGuid(),
                UserId = adminUserId,
                GroupName = dto.GroupName,
                // Если цвет не пришел в JSON, генерируем красивый градиент
                GroupColor = !string.IsNullOrWhiteSpace(dto.GroupColor) 
                    ? dto.GroupColor 
                    : HelpersMethods.GetRandomGradient(),
                // Если в JSON иконка указана - берем её. Если нет - пытаемся угадать.
                GroupIcon = !string.IsNullOrWhiteSpace(dto.GroupIcon) 
                    ? dto.GroupIcon 
                    : HelpersMethods.GetSmartIcon(dto.GroupName, dto.Tags),                
                CreatedAt = DateTime.UtcNow,
                IsPublished = true,
                SubscriberCount = 0,
                Tags = groupTags
            };

            context.Groups.Add(newGroup);

            // Создание Карточек
            if (dto.Cards != null)
            {
                foreach (var cardDto in dto.Cards)
                {
                    context.Cards.Add(new Card
                    {
                        CardId = Guid.NewGuid(),
                        GroupId = newGroup.Id,
                        Question = cardDto.Question,
                        Answer = cardDto.Answer,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }
            
            await context.SaveChangesAsync();
            Console.WriteLine($"DONE (Tags: {groupTags.Count}, Cards: {dto.Cards?.Count ?? 0})");
        }

        Console.WriteLine("\n✅ ALL DONE SUCCESSFULLY!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ Critical Error: {ex.Message}");
        if (ex.InnerException != null) Console.WriteLine($"   Inner: {ex.InnerException.Message}");
    }
}

// --- ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ---

string ReadPassword()
{
    string pass = "";
    do
    {
        ConsoleKeyInfo key = Console.ReadKey(true);
        if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
        {
            pass += key.KeyChar;
            Console.Write("*");
        }
        else
        {
            if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
            {
                pass = pass.Substring(0, (pass.Length - 1));
                Console.Write("\b \b");
            }
            else if(key.Key == ConsoleKey.Enter) break;
        }
    } while (true);
    Console.WriteLine();
    return pass;
}

string? SelectFile(string filter)
{
    string? filePath = null;
    var thread = new Thread(() =>
    {
        var dialog = new VistaOpenFileDialog
        {
            Filter = filter,
            Multiselect = false,
            Title = "Select content.json"
        };
        if (dialog.ShowDialog() == true) filePath = dialog.FileName;
    });
    thread.SetApartmentState(ApartmentState.STA);
    thread.Start();
    thread.Join();
    return filePath;
}
