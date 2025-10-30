using OfficeOpenXml;
using System.Text.Json;
using FlashcardsApp.Tools.Models;
using System.IO;

namespace FlashcardsApp.Tools;

public static class ExcelToJsonConverter
{
    public static async Task<ConversionResult> ConvertAsync(string excelPath)
    {
        var result = new ConversionResult();

        // Проверка существования файла
        if (!File.Exists(excelPath))
        {
            result.IsSuccess = false;
            result.ErrorMessage = "File not found!";
            return result;
        }

        try
        {
            using var package = new ExcelPackage(new FileInfo(excelPath));

            // ===== ШАГ 1: Читаем метаданные группы =====
            var groupInfo = ReadGroupInfo(package, result);
            if (groupInfo == null)
            {
                return result;
            }

            // ===== ШАГ 2: Читаем карточки =====
            var cards = ReadCards(package, result);
            if (cards.Count == 0)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "No valid cards found! Please add at least one card.";
                return result;
            }

            // ===== ШАГ 3: Формируем JSON =====
            var importData = new
            {
                groupName = groupInfo.GroupName,
                groupColor = groupInfo.GroupColor,
                groupIcon = groupInfo.GroupIcon,
                order = groupInfo.Order,
                cards = cards.Select(c => new
                {
                    question = c.Question,
                    answer = c.Answer
                }).ToList()
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            result.Json = JsonSerializer.Serialize(importData, options);
            result.IsSuccess = true;
            result.TotalCards = cards.Count;
            result.GroupName = groupInfo.GroupName;

            return result;
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = $"Error reading Excel file: {ex.Message}";
            return result;
        }
    }

    private static GroupInfo? ReadGroupInfo(ExcelPackage package, ConversionResult result)
    {
        var sheet = package.Workbook.Worksheets["GroupInfo"];
        if (sheet == null)
        {
            result.IsSuccess = false;
            result.ErrorMessage = "Sheet 'GroupInfo' not found! Please use the correct template.";
            return null;
        }

        var groupInfo = new GroupInfo();
        var errors = new List<string>();

        // Читаем построчно (Field в колонке A, Value в колонке B)
        for (int row = 2; row <= Math.Min(sheet.Dimension?.Rows ?? 0, 10); row++)
        {
            var field = sheet.Cells[row, 1].Text.Trim();
            var value = sheet.Cells[row, 2].Text.Trim();

            switch (field)
            {
                case "GroupName":
                    groupInfo.GroupName = value;
                    break;
                case "GroupColor":
                    groupInfo.GroupColor = value;
                    break;
                case "GroupIcon":
                    groupInfo.GroupIcon = value;
                    break;
                case "Order":
                    int.TryParse(value, out var order);
                    groupInfo.Order = order;
                    break;
            }
        }

        // Валидация обязательных полей
        if (string.IsNullOrEmpty(groupInfo.GroupName))
            errors.Add("GroupName is required");
        
        if (string.IsNullOrEmpty(groupInfo.GroupColor))
            errors.Add("GroupColor is required");
        
        if (string.IsNullOrEmpty(groupInfo.GroupIcon))
            errors.Add("GroupIcon is required");

        if (errors.Any())
        {
            result.IsSuccess = false;
            result.ErrorMessage = "Missing required fields:\n  - " + string.Join("\n  - ", errors);
            return null;
        }

        return groupInfo;
    }

    private static List<CardInfo> ReadCards(ExcelPackage package, ConversionResult result)
    {
        var sheet = package.Workbook.Worksheets["Cards"];
        if (sheet == null)
        {
            result.IsSuccess = false;
            result.ErrorMessage = "Sheet 'Cards' not found! Please use the correct template.";
            return new List<CardInfo>();
        }

        var cards = new List<CardInfo>();
        var skippedRows = new List<int>();
        var rowCount = sheet.Dimension?.Rows ?? 0;

        // Начинаем со 2-й строки (первая - заголовки)
        for (int row = 2; row <= rowCount; row++)
        {
            var question = sheet.Cells[row, 1].Text.Trim();
            var answer = sheet.Cells[row, 2].Text.Trim();

            // Пропускаем пустые строки
            if (string.IsNullOrEmpty(question) && string.IsNullOrEmpty(answer))
            {
                continue;
            }

            // Проверяем что оба поля заполнены
            if (string.IsNullOrEmpty(question) || string.IsNullOrEmpty(answer))
            {
                skippedRows.Add(row);
                continue;
            }

            cards.Add(new CardInfo
            {
                Question = question,
                Answer = answer
            });
        }

        result.SkippedRows = skippedRows;
        return cards;
    }
}