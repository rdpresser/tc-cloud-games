namespace TC.CloudGames.Infra.CrossCutting.Commons.Extensions;

public static class StringExtensions
{
    public static string JoinWithQuotes(this IEnumerable<string> values, string separator = ", ")
    {
        return string.Join(separator, values.Select(v => $"'{v}'"));
    }
}