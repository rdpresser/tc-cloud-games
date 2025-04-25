namespace TC.CloudGames.Infra.Data.Configurations.Data
{
    public class DatabaseSettings
    {
        public required string Host { get; set; }
        public required string Port { get; set; }
        public required string Name { get; set; }
        public required string User { get; set; }
        public required string Password { get; set; }
    }
}
