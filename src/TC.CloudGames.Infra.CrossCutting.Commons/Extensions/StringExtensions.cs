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
}