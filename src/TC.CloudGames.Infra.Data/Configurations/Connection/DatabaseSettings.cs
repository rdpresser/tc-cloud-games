namespace TC.CloudGames.Infra.Data.Configurations.Connection;

public class DatabaseSettings
{
    public required string Host { get; init; }
    public required string Port { get; init; }
    public required string Name { get; init; }
    public required string User { get; init; }
    public required string Password { get; init; }
}