using System.Globalization;

namespace Lab_1.Services;

public static class EntryExtensions
{
    public static string TrimmedText(this Entry entry)
    {
        return entry.Text?.Trim() ?? "";
    }

    public static string? OptionalText(this Entry entry)
    {
        return string.IsNullOrWhiteSpace(entry.Text) ? null : entry.Text.Trim();
    }

    public static int ToInt(this Entry entry)
    {
        return int.TryParse(entry.Text, out var value) ? value : 0;
    }

    public static decimal ToDecimal(this Entry entry)
    {
        var text = (entry.Text ?? "").Replace(',', '.');
        return decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var value) ? value : 0;
    }
}
