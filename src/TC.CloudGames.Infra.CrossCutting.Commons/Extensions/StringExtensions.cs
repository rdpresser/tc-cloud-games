namespace TC.CloudGames.Infra.CrossCutting.Commons.Extensions;

public static class StringExtensions
{
    public static string JoinWithQuotes(this IEnumerable<string> values, string separator = ", ")
    {
        return string.Join(separator, values.Select(v => $"'{v}'"));
    }

    /// <summary>
    /// Returns a string containing only the digit characters from the input string.
    /// </summary>
    public static string OnlyDigits(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return new string(value.Where(char.IsDigit).ToArray());
    }

    /// <summary>
    /// Returns a string containing only the letter characters from the input string.
    /// </summary>
    public static string OnlyLetters(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return new string(value.Where(char.IsLetter).ToArray());
    }

    public static string ToCamelCase(this string value)
    {
        if (string.IsNullOrEmpty(value) || !char.IsUpper(value[0]))
            return value;

        char[] chars = value.ToCharArray();
        chars[0] = char.ToLowerInvariant(chars[0]);
        return new string(chars);
    }

    public static string ToPascalCaseFirst(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        if (char.IsUpper(value[0]))
            return value;

        return char.ToUpperInvariant(value[0]) + value.Substring(1);
    }
}