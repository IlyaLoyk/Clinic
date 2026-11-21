using System.Globalization;
using Clinic.Core.Exceptions;

namespace Clinic.UI;

/// Допоміжні методи для безпечного введення даних з консолі та виведення повідомлень.
public static class InputHelper
{
    private const string DateFormat = "dd.MM.yyyy";
    private const string DateTimeFormat = "dd.MM.yyyy HH:mm";

    // ------------------------------ Введення ------------------------------

    /// Читає ціле число в заданому діапазоні, поки користувач не введе коректне значення.
    public static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int result) && result >= min && result <= max)
                return result;
            PrintError($"Введіть ціле число від {min} до {max}.");
        }
    }

    /// Читає непорожній рядок
    public static string ReadString(string prompt, int minLength = 1)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = (Console.ReadLine() ?? string.Empty).Trim();
            if (input.Length >= minLength)
                return input;
            PrintError($"Введіть щонайменше {minLength} символ(ів).");
        }
    }

    /// Читає рядок, який можна залишити порожнім.
    public static string ReadOptional(string prompt)
    {
        Console.Write(prompt);
        return (Console.ReadLine() ?? string.Empty).Trim();
    }

    public static DateTime ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (DateTime.TryParseExact(Console.ReadLine()?.Trim(), DateFormat,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                return result;
            PrintError("Невірний формат. Приклад: 25.12.2026");
        }
    }

    public static DateTime ReadDateTime(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (DateTime.TryParseExact(Console.ReadLine()?.Trim(), DateTimeFormat,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                return result;
            PrintError("Невірний формат. Приклад: 25.12.2026 14:30");
        }
    }

    /// Запитання "так/ні".
    public static bool Confirm(string prompt)
    {
        while (true)
        {
            Console.Write(prompt + " (т/н): ");
            string answer = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
            if (answer is "т" or "так" or "y" or "yes") return true;
            if (answer is "н" or "ні" or "n" or "no") return false;
            PrintError("Введіть 'т' або 'н'.");
        }
    }

    /// Показує пронумерований список значень enum та повертає обране.
    public static T ReadEnum<T>(string title) where T : struct, Enum
    {
        T[] values = Enum.GetValues<T>();
        Console.WriteLine(title + ":");
        for (int i = 0; i < values.Length; i++)
            Console.WriteLine($"  {i + 1}. {EnumNames.Get(values[i])}");
        int choice = ReadInt("Ваш вибір: ", 1, values.Length);
        return values[choice - 1];
    }

    /// Запитує значення і передає його в assign (зазвичай це присвоєння властивості,
    /// яка сама виконує валідацію). Якщо виникає помилка валідації - повторює запит.
    public static void Assign(string prompt, Action<string> assign)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine() ?? string.Empty;
            try
            {
                assign(input);
                return;
            }
            catch (Exception ex) when (ex is ClinicException or ArgumentException
                                           or FormatException or OverflowException)
            {
                PrintError(ex.Message);
            }
        }
    }

    // ------------------------------ Парсинг -------------------------------

    public static decimal ParseDecimal(string text)
    {
        string normalized = text.Trim().Replace(',', '.');
        if (decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result))
            return result;
        throw new FormatException("Введіть число, наприклад 450 або 450.50.");
    }

    public static DateTime ParseDate(string text)
    {
        if (DateTime.TryParseExact(text.Trim(), DateFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime result))
            return result;
        throw new FormatException("Невірний формат дати. Приклад: 25.12.1990");
    }

    public static TimeSpan ParseTime(string text)
    {
        if (TimeSpan.TryParseExact(text.Trim(), @"h\:mm", CultureInfo.InvariantCulture, out TimeSpan result))
            return result;
        throw new FormatException("Невірний формат часу. Приклад: 09:00");
    }

    // ------------------------------ Виведення -----------------------------

    public static void PrintError(string message) => PrintColored("[Помилка] " + message, ConsoleColor.Red);

    public static void PrintSuccess(string message) => PrintColored("[OK] " + message, ConsoleColor.Green);

    public static void PrintInfo(string message) => PrintColored(message, ConsoleColor.Yellow);

    private static void PrintColored(string message, ConsoleColor color)
    {
        ConsoleColor previous = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = previous;
    }

    /// Виконує дію меню і перехоплює очікувані помилки,
    /// щоб програма не завершувалась аварійно.
    public static void Execute(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex) when (ex is ClinicException or ArgumentException
                                       or FormatException or InvalidOperationException)
        {
            PrintError(ex.Message);
        }
        catch (Exception ex)
        {
            PrintError("Неочікувана помилка: " + ex.Message);
        }
    }
}
