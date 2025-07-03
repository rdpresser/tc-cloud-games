namespace TC.CloudGames.Integration.Tests.Abstractions
{
    public static class CacheStringParser
    {
        public static Dictionary<string, string> Parse(string connectionString)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(connectionString))
                return dict;

            // Split the first part (host:port) from the rest
            var firstComma = connectionString.IndexOf(',');
            string hostPortPart;
            string optionsPart = null;

            if (firstComma >= 0)
            {
                hostPortPart = connectionString.Substring(0, firstComma);
                optionsPart = connectionString.Substring(firstComma + 1);
            }
            else
            {
                hostPortPart = connectionString;
            }

            // Parse host and port
            var hostPortSplit = hostPortPart.Split(':', 2);
            if (hostPortSplit.Length == 2)
            {
                dict["host"] = hostPortSplit[0].Trim();
                dict["port"] = hostPortSplit[1].Trim();
            }
            else if (hostPortSplit.Length == 1)
            {
                dict["host"] = hostPortSplit[0].Trim();
            }

            // Parse the rest of the options (key=value)
            if (!string.IsNullOrWhiteSpace(optionsPart))
            {
                var pairs = optionsPart.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var pair in pairs)
                {
                    var kv = pair.Split('=', 2);
                    if (kv.Length == 2)
                        dict[kv[0].Trim()] = kv[1].Trim();
                }
            }

            return dict;
        }
    }
}
