using OfficeOpenXml;
using Ookii.Dialogs.Wpf;
using System.IO;

ExcelPackage.License.SetNonCommercialPersonal("Your Name");

Console.WriteLine("╔═══════════════════════════════════════════╗");
Console.WriteLine("║  Flashcards Tools - Utilities for Import  ║");
Console.WriteLine("╚═══════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine("Choose an option:");
Console.WriteLine("  1. Generate Excel template");
Console.WriteLine("  2. Convert Excel to JSON");
Console.WriteLine("  0. Exit");
Console.WriteLine();
Console.Write("Your choice: ");

var choice = Console.ReadLine();
Console.WriteLine();

switch (choice)
{
    case "1":
        await GenerateTemplate();
        break;
    case "2":
        await ConvertExcelToJson();
        break;
    case "0":
        Console.WriteLine("Goodbye! 👋");
        break;
    default:
        Console.WriteLine("❌ Invalid choice!");
        break;
}

static async Task GenerateTemplate()
{
    Console.WriteLine("=== Generate Excel Template ===");
    Console.WriteLine();

    var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    var outputPath = Path.Combine(desktopPath, "FlashcardsTemplate.xlsx");

    Console.WriteLine($"📝 Creating template on Desktop...");
    Console.WriteLine();

    try
    {
        await FlashcardsApp.Tools.TemplateGenerator.GenerateAsync(outputPath);
        
        Console.WriteLine($"✅ Template created successfully!");
        Console.WriteLine($"📁 Location: {outputPath}");
        Console.WriteLine();
        Console.WriteLine($"💡 Next steps:");
        Console.WriteLine($"   1. Open the file in Excel");
        Console.WriteLine($"   2. Fill in GroupInfo and Cards sheets");
        Console.WriteLine($"   3. Use option 2 to convert to JSON");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error: {ex.Message}");
    }
}

static async Task ConvertExcelToJson()
{
    Console.WriteLine("=== Convert Excel to JSON ===");
    Console.WriteLine();
    Console.WriteLine("📂 Opening file dialog...");
    Console.WriteLine();

    string? excelPath = null;
    
    // Открываем диалог в отдельном потоке (требование WPF)
    var thread = new Thread(() =>
    {
        var dialog = new VistaOpenFileDialog
        {
            Title = "Select Excel file with flashcards",
            Filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*",
            FilterIndex = 1,
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            Multiselect = false
        };

        if (dialog.ShowDialog() == true)
        {
            excelPath = dialog.FileName;
        }
    });

    thread.SetApartmentState(ApartmentState.STA);
    thread.Start();
    thread.Join();

    // Пользователь отменил выбор
    if (string.IsNullOrEmpty(excelPath))
    {
        Console.WriteLine("❌ File selection cancelled.");
        return;
    }

    Console.WriteLine($"📂 Selected file: {Path.GetFileName(excelPath)}");
    Console.WriteLine();
    Console.WriteLine("🔄 Converting...");
    Console.WriteLine();

    var result = await FlashcardsApp.Tools.ExcelToJsonConverter.ConvertAsync(excelPath);

    if (!result.IsSuccess)
    {
        Console.WriteLine($"❌ Conversion failed:");
        Console.WriteLine($"   {result.ErrorMessage}");
        return;
    }

    // Статистика
    Console.WriteLine($"✅ Conversion successful!");
    Console.WriteLine($"   Group: {result.GroupName}");
    Console.WriteLine($"   Total cards: {result.TotalCards}");

    if (result.SkippedRows.Any())
    {
        Console.WriteLine($"   ⚠️  Skipped rows (incomplete): {string.Join(", ", result.SkippedRows)}");
    }

    Console.WriteLine();

    // Сохраняем JSON на Desktop
    var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    var jsonFileName = $"{SanitizeFileName(result.GroupName)}.json";
    var jsonPath = Path.Combine(desktopPath, jsonFileName);

    await File.WriteAllTextAsync(jsonPath, result.Json);

    Console.WriteLine($"📁 JSON saved to Desktop:");
    Console.WriteLine($"   {jsonPath}");
    Console.WriteLine();
    Console.WriteLine($"💡 Next step:");
    Console.WriteLine($"   Copy the JSON content and send it to your API:");
    Console.WriteLine($"   POST {{{{baseUrl}}}}/api/import/group-with-cards");
    Console.WriteLine();
    Console.WriteLine("📋 Preview (first 500 characters):");
    Console.WriteLine(result.Json!.Length > 500 
        ? result.Json.Substring(0, 500) + "..." 
        : result.Json);
}

static string SanitizeFileName(string? fileName)
{
    if (string.IsNullOrEmpty(fileName))
        return "group";
    
    // Убираем недопустимые символы из имени файла
    var invalid = Path.GetInvalidFileNameChars();
    var sanitized = string.Join("_", fileName.Split(invalid, StringSplitOptions.RemoveEmptyEntries));
    return string.IsNullOrEmpty(sanitized) ? "group" : sanitized;
}