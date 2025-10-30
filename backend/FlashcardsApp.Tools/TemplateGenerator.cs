using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;

namespace FlashcardsApp.Tools;

public static class TemplateGenerator
{
    public static async Task GenerateAsync(string outputPath)
    {
        using var package = new ExcelPackage();

        // Лист 1: GroupInfo (метаданные группы)
        CreateGroupInfoSheet(package);

        // Лист 2: Cards (карточки)
        CreateCardsSheet(package);

        // Лист 3: ColorReference (справочник цветов)
        CreateColorReferenceSheet(package);

        // Лист 4: IconReference (справочник иконок)
        CreateIconReferenceSheet(package);

        // Сохраняем файл
        await package.SaveAsAsync(new FileInfo(outputPath));
    }

    private static void CreateGroupInfoSheet(ExcelPackage package)
    {
        var sheet = package.Workbook.Worksheets.Add("GroupInfo");

        // Заголовки
        sheet.Cells["A1"].Value = "Field";
        sheet.Cells["B1"].Value = "Value";
        sheet.Cells["C1"].Value = "Example / Help";

        // Стиль заголовков
        using (var range = sheet.Cells["A1:C1"])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            // Рамка для заголовков
            range.Style.Border.Top.Style = ExcelBorderStyle.Medium;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
            range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
        }

        // Поля для заполнения
        sheet.Cells["A2"].Value = "GroupName";
        sheet.Cells["C2"].Value = "Например: Основы C#, LINQ, Async/Await";

        sheet.Cells["A3"].Value = "GroupColor";
        sheet.Cells["C3"].Value = "См. лист 'ColorReference' →";

        sheet.Cells["A4"].Value = "GroupIcon";
        sheet.Cells["C4"].Value = "См. лист 'IconReference' →";

        sheet.Cells["A5"].Value = "Order";
        sheet.Cells["B5"].Value = 1;
        sheet.Cells["C5"].Value = "Порядок отображения (1, 2, 3...)";

        // Подсветка обязательных полей
        sheet.Cells["A2:A4"].Style.Font.Color.SetColor(Color.Red);
        sheet.Cells["A2:A4"].Style.Font.Bold = true;

        // Рамка вокруг таблицы данных
        using (var range = sheet.Cells["A2:C5"])
        {
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }

        // Внешняя рамка потолще
        using (var range = sheet.Cells["A1:C5"])
        {
            range.Style.Border.Top.Style = ExcelBorderStyle.Medium;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
            range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
        }

        // Ширина колонок
        sheet.Column(1).Width = 15;
        sheet.Column(2).Width = 40;
        sheet.Column(3).Width = 35;

        // ВАЖНЫЕ ИНСТРУКЦИИ
        sheet.Cells["A7"].Value = "⚠️ ВАЖНО:";
        sheet.Cells["A7"].Style.Font.Bold = true;
        sheet.Cells["A7"].Style.Font.Size = 12;
        sheet.Cells["A7"].Style.Font.Color.SetColor(Color.DarkRed);

        sheet.Cells["A8"].Value = "• Поля GroupName, GroupColor и GroupIcon обязательны для заполнения!";
        sheet.Cells["A8"].Style.Font.Color.SetColor(Color.DarkRed);

        sheet.Cells["A9"].Value = "• Один файл = одна группа с карточками";
        sheet.Cells["A9"].Style.Font.Bold = true;
        sheet.Cells["A9"].Style.Font.Color.SetColor(Color.DarkBlue);

        sheet.Cells["A10"].Value = "• Для создания нескольких групп используйте отдельные файлы";
        sheet.Cells["A10"].Style.Font.Color.SetColor(Color.DarkBlue);
    }

    private static void CreateCardsSheet(ExcelPackage package)
    {
        var sheet = package.Workbook.Worksheets.Add("Cards");

        // Заголовки
        sheet.Cells["A1"].Value = "Question";
        sheet.Cells["B1"].Value = "Answer";

        // Стиль заголовков
        using (var range = sheet.Cells["A1:B1"])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            // Рамка для заголовков
            range.Style.Border.Top.Style = ExcelBorderStyle.Medium;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
            range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
        }

        // Примеры карточек
        sheet.Cells["A2"].Value = "Что такое класс?";
        sheet.Cells["B2"].Value = "Шаблон для создания объектов";

        sheet.Cells["A3"].Value = "Что такое интерфейс?";
        sheet.Cells["B3"].Value = "Контракт, который должен реализовать класс";

        sheet.Cells["A4"].Value = "Что такое namespace?";
        sheet.Cells["B4"].Value = "Логическая группировка классов и типов";

        // Стиль примеров (серый цвет)
        using (var range = sheet.Cells["A2:B4"])
        {
            range.Style.Font.Italic = true;
            range.Style.Font.Color.SetColor(Color.Gray);
            // Рамка вокруг примеров
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }

        // Внешняя рамка вокруг заголовков и примеров
        using (var range = sheet.Cells["A1:B4"])
        {
            range.Style.Border.Top.Style = ExcelBorderStyle.Medium;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
            range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
        }

        // Ширина колонок
        sheet.Column(1).Width = 50;
        sheet.Column(2).Width = 50;

        // Инструкции
        sheet.Cells["A6"].Value = "💡 ИНСТРУКЦИЯ:";
        sheet.Cells["A6"].Style.Font.Bold = true;
        sheet.Cells["A6"].Style.Font.Size = 12;

        sheet.Cells["A7"].Value = "• Удалите примеры выше (строки 2-4)";
        sheet.Cells["A8"].Value = "• Добавьте свои карточки начиная со строки 2";
        sheet.Cells["A9"].Value = "• Можно добавить до 100 карточек в одну группу";
        sheet.Cells["A10"].Value = "• Каждая карточка должна иметь и вопрос, и ответ";
    }

    private static void CreateColorReferenceSheet(ExcelPackage package)
    {
        var sheet = package.Workbook.Worksheets.Add("ColorReference");

        // Заголовки
        sheet.Cells["A1"].Value = "Color Name";
        sheet.Cells["B1"].Value = "CSS Gradient (копируйте в GroupColor)";
        sheet.Cells["C1"].Value = "Preview";

        // Стиль заголовков
        using (var range = sheet.Cells["A1:C1"])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        // Данные цветов с RGB для предпросмотра
        var colors = new[]
        {
            ("Blue → Purple", "from-blue-500 to-purple-500", Color.FromArgb(59, 130, 246)),
            ("Green → Emerald", "from-green-500 to-emerald-500", Color.FromArgb(34, 197, 94)),
            ("Orange → Yellow", "from-orange-500 to-yellow-500", Color.FromArgb(249, 115, 22)),
            ("Purple → Pink", "from-purple-500 to-pink-500", Color.FromArgb(168, 85, 247)),
            ("Red → Rose", "from-red-500 to-rose-500", Color.FromArgb(239, 68, 68)),
            ("Cyan → Blue", "from-cyan-400 to-blue-600", Color.FromArgb(34, 211, 238)),
            ("Teal → Green", "from-teal-400 to-green-600", Color.FromArgb(45, 212, 191)),
            ("Indigo → Purple", "from-indigo-400 to-purple-600", Color.FromArgb(129, 140, 248)),
            ("Pink → Rose", "from-pink-400 to-rose-500", Color.FromArgb(244, 114, 182)),
            ("Amber → Orange", "from-amber-400 to-orange-600", Color.FromArgb(251, 191, 36))
        };

        for (int i = 0; i < colors.Length; i++)
        {
            var row = i + 2;
            sheet.Cells[row, 1].Value = colors[i].Item1;
            sheet.Cells[row, 2].Value = colors[i].Item2;

            // Закрашиваем Preview ячейку реальным цветом
            sheet.Cells[row, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[row, 3].Style.Fill.BackgroundColor.SetColor(colors[i].Item3);
            sheet.Cells[row, 3].Value = "";
        }

        // Рамки для таблицы
        using (var range = sheet.Cells["A2:C11"])
        {
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }

        // Внешняя рамка
        using (var range = sheet.Cells["A1:C11"])
        {
            range.Style.Border.Top.Style = ExcelBorderStyle.Medium;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
            range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
        }

        // Ширина колонок
        sheet.Column(1).Width = 20;
        sheet.Column(2).Width = 40;
        sheet.Column(3).Width = 12;

        // Примечание
        sheet.Cells["A13"].Value = "📋 Скопируйте нужный градиент из колонки B в поле GroupColor";
        sheet.Cells["A13"].Style.Font.Bold = true;
        sheet.Cells["A14"].Value = "💡 Колонка C показывает примерный цвет (в приложении будет градиент)";
    }

    private static void CreateIconReferenceSheet(ExcelPackage package)
    {
        var sheet = package.Workbook.Worksheets.Add("IconReference");

        // Заголовки
        sheet.Cells["A1"].Value = "Category";
        sheet.Cells["B1"].Value = "Icons (кликните на нужную иконку и скопируйте)";

        // Стиль заголовков
        using (var range = sheet.Cells["A1:B1"])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        // Данные иконок
        var icons = new[]
        {
            ("Programming", "💻 ⌨️ 🖥️ 📱 🔧 ⚙️ 🛠️ 🔌"), ("Learning", "📚 📖 ✏️ 📝 🎓 🧠 💡 🔍"),
            ("Languages", "🇬🇧 🇷🇺 🇪🇸 🇫🇷 🇩🇪 🇨🇳 🇯🇵 🇰🇷"), ("Science", "⚗️ 🔬 🧪 🧬 🔭 🌡️ 🧮 🔢"),
            ("Math", "➕ ➖ ✖️ ➗ 🔢 📐 📏 🧮"), ("Business", "💼 📊 📈 💰 💵 🏢 📞 ✉️"),
            ("Art & Design", "🎨 🖌️ 🖍️ ✏️ 🎭 🎪 🎬 📷"), ("Music", "🎵 🎶 🎸 🎹 🎤 🎧 🎼 🎺"),
            ("Sports", "⚽ 🏀 🎾 🏈 ⚾ 🏐 🎱 🏓"), ("Food", "🍕 🍔 🍟 🌮 🍱 🍜 🍰 ☕"), ("Nature", "🌳 🌲 🌴 🌵 🌿 🍀 🌾 🌻"),
            ("Weather", "☀️ 🌙 ⭐ ☁️ ⛅ 🌧️ ⛈️ 🌈"), ("Transport", "🚗 🚕 🚙 🚌 🚎 🏎️ 🚓 🚑"),
            ("Other", "⭐ 🎯 🔥 ⚡ 💎 🏆 🎁 🎉")
        };

        for (int i = 0; i < icons.Length; i++)
        {
            var row = i + 2;
            sheet.Cells[row, 1].Value = icons[i].Item1;
            sheet.Cells[row, 2].Value = icons[i].Item2;

            // Увеличиваем размер шрифта для иконок
            sheet.Cells[row, 2].Style.Font.Size = 16;
        }

        // Рамки для таблицы
        using (var range = sheet.Cells["A2:B15"])
        {
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }

        // Внешняя рамка
        using (var range = sheet.Cells["A1:B15"])
        {
            range.Style.Border.Top.Style = ExcelBorderStyle.Medium;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
            range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
        }

        // Ширина колонок
        sheet.Column(1).Width = 20;
        sheet.Column(2).Width = 50;

        // Примечание
        sheet.Cells["A17"].Value = "📋 Выберите одну иконку из списка для GroupIcon";
        sheet.Cells["A17"].Style.Font.Bold = true;
        sheet.Cells["A18"].Value = "💡 В Excel иконки чёрно-белые, но в приложении будут цветными!";
        sheet.Cells["A18"].Style.Font.Italic = true;
    }

}
