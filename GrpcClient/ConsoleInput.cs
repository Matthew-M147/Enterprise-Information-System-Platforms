using System.Globalization;

public static class ConsoleInput
{
    public static string ReadLine()
    {
        var line = Console.ReadLine();
        if (line == null)
            Environment.Exit(0);
        return line.Trim();
    }

    public static string ReadChoice()
    {
        Console.Write("Ваш вибір: ");
        return ReadLine();
    }

    public static string ReadText(string label, string? current = null)
    {
        Console.Write(current == null ? $"{label}: " : $"{label} [{current}]: ");
        var input = ReadLine();
        return input == "" && current != null ? current : input;
    }

    public static string? ReadOptionalText(string label, string? current = null)
    {
        Console.Write(current == null ? $"{label} (необов'язково): " : $"{label} [{current}] (\"-\" — очистити): ");
        var input = ReadLine();
        if (input == "-")
            return null;
        return input == "" ? current : input;
    }

    public static int ReadInt(string label, int? current = null)
    {
        while (true)
        {
            Console.Write(current == null ? $"{label}: " : $"{label} [{current}]: ");
            var input = ReadLine();
            if (input == "" && current != null)
                return current.Value;
            if (int.TryParse(input, out var value))
                return value;
            Console.WriteLine("Введіть ціле число.");
        }
    }

    public static decimal ReadDecimal(string label, decimal? current = null)
    {
        while (true)
        {
            Console.Write(current == null ? $"{label}: " : $"{label} [{current}]: ");
            var input = ReadLine().Replace(',', '.');
            if (input == "" && current != null)
                return current.Value;
            if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                return value;
            Console.WriteLine("Введіть число, наприклад 1500.50");
        }
    }

    public static string ReadDate(string label, string? current = null)
    {
        while (true)
        {
            Console.Write(current == null ? $"{label} (рррр-мм-дд): " : $"{label} [{current}]: ");
            var input = ReadLine();
            if (input == "" && current != null)
                return current;
            if (DateOnly.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                return input;
            Console.WriteLine("Введіть дату у форматі рррр-мм-дд, наприклад 2026-07-15");
        }
    }

    public static bool Confirm(string question)
    {
        Console.Write($"{question} (так/ні): ");
        var answer = ReadLine().ToLowerInvariant();
        return answer is "так" or "т" or "y" or "yes";
    }
}
