using System.Text.RegularExpressions;

namespace FlashcardsApp.Tools;

public static class HelpersMethods
{
    public static string GenerateSlug(string phrase)
    {
        string str = phrase.ToLower();
        str = Regex.Replace(str, @"\s+", "-");
        str = Regex.Replace(str, @"[^\w\-а-яё]", "");
        return str;
    }

    // Генератор цветов для ТЕГОВ (простые названия или классы)
    public static string GetRandomTagColor()
    {
        var colors = new[]
        {
            "blue", "green", "red", "yellow", "purple", "pink", "indigo", "teal", "orange"
        };
        return colors[new Random().Next(colors.Length)];
    }

    // Генератор градиентов для ГРУПП (Tailwind)
    public static string GetRandomGradient()
    {
        var gradients = new[]
        {
            "from-blue-500 to-cyan-500", "from-emerald-500 to-teal-500", "from-orange-500 to-yellow-500",
            "from-pink-500 to-rose-500", "from-purple-600 to-blue-600", "from-indigo-500 to-purple-500",
            "from-red-500 to-orange-500", "from-lime-500 to-green-500", "from-teal-400 to-blue-500",
            "from-fuchsia-600 to-pink-600", "from-rose-400 to-red-500", "from-sky-500 to-indigo-500",
            "from-violet-600 to-indigo-600", "from-amber-500 to-orange-600", "from-cyan-500 to-blue-500"
        };
        return gradients[new Random().Next(gradients.Length)];
    }

    // --- УМНЫЙ ВЫБОР ИКОНКИ ---
    public static string GetSmartIcon(string groupName, List<string>? tags)
    {
        // 1. Список всех доступных иконок (должен совпадать с Frontend data.ts)
        var allIcons = new[]
        {
            "BookOpen", "Code", "Globe", "Languages", "Brain", "Calculator", "Dna", "Atom", "Music", "Palette", "Briefcase",
            "Coffee", "Dumbbell", "Gamepad2", "GraduationCap", "Heart", "Lightbulb", "Microscope", "Rocket", "Smile",
            "Terminal", "Beaker", "Zap"
        };

        // 2. Нормализуем текст для поиска (все в нижний регистр)
        var textToAnalyze = (groupName + " " + string.Join(" ", tags ?? new List<string>())).ToLower();

        // 3. Простые правила ассоциаций
        if (textToAnalyze.Contains("c#") || textToAnalyze.Contains("code") || textToAnalyze.Contains("программирование") ||
            textToAnalyze.Contains("python") || textToAnalyze.Contains("js"))
            return "Code";

        if (textToAnalyze.Contains("console") || textToAnalyze.Contains("bash") || textToAnalyze.Contains("linux"))
            return "Terminal";

        if (textToAnalyze.Contains("english") || textToAnalyze.Contains("английский") || textToAnalyze.Contains("words") ||
            textToAnalyze.Contains("язык"))
            return "Languages";

        if (textToAnalyze.Contains("geography") || textToAnalyze.Contains("travel") || textToAnalyze.Contains("страны") ||
            textToAnalyze.Contains("города"))
            return "Globe";

        if (textToAnalyze.Contains("math") || textToAnalyze.Contains("алгебра") || textToAnalyze.Contains("счет"))
            return "Calculator";

        if (textToAnalyze.Contains("science") || textToAnalyze.Contains("physics") || textToAnalyze.Contains("наука"))
            return "Atom";

        if (textToAnalyze.Contains("chemistry") || textToAnalyze.Contains("химия"))
            return "Beaker";

        if (textToAnalyze.Contains("biology") || textToAnalyze.Contains("медицина"))
            return "Dna";

        // 4. Если ничего не подошло — возвращаем случайную
        return allIcons[new Random().Next(allIcons.Length)];
    }
}
