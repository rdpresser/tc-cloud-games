namespace TC.CloudGames.Integration.Tests.Abstractions
{
    public static class ConnectionStringParser
    {
        public static Dictionary<string, string> Parse(string connectionString)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(connectionString))
                return dict;

            var pairs = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in pairs)
            {
                var kv = pair.Split('=', 2);
                if (kv.Length == 2)
                    dict[kv[0].Trim()] = kv[1].Trim();
            }
            return dict;
        }
    }
}
