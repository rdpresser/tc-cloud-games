namespace TC.CloudGames.Infra.CrossCutting.Commons.Caching
{
    public class CacheSettings
    {
        public required string Host { get; init; }
        public required string Port { get; init; }
        public string? Password { get; init; } = null;
        public required string InstanceName { get; init; }
    }
}
