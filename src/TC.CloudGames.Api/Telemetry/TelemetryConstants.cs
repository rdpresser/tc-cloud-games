namespace TC.CloudGames.Api.Telemetry;

/// <summary>
/// Constants for telemetry across the application
/// </summary>
public static class TelemetryConstants
{
    // Versions
    public const string Version = "1.0.0";

    // Service Identity - Centralized for consistency (matches Docker Compose)
    public const string ServiceName = "tccloudgames-app";
    public const string ServiceNamespace = "tccloudgames-app-group";

    // Meter Names for OpenTelemetry Metrics
    public const string GameMeterName = "TC.CloudGames.Games.Metrics";
    public const string UserMeterName = "TC.CloudGames.Users.Metrics";

    // Activity Source Names for OpenTelemetry Tracing
    public const string GameActivitySource = "TC.CloudGames.Games";
    public const string UserActivitySource = "TC.CloudGames.Users";
    public const string DatabaseActivitySource = "TC.CloudGames.Database";
    public const string CacheActivitySource = "TC.CloudGames.Cache";

    // Header Names (standardized)
    public const string CorrelationIdHeader = "x-correlation-id";

    // Tag Names (using underscores for consistency with Loki labels)
    public const string ServiceComponent = "service.component";
    public const string GameType = "game_type";
    public const string UserId = "user_id";
    public const string GameId = "game_id";
    public const string GameDifficulty = "game_difficulty";
    public const string GameStatus = "game_status";
    public const string UserAction = "user_action";
    public const string SessionId = "session_id";
    public const string ErrorType = "error_type";

    // Default Values
    public const string DefaultDifficulty = "normal";
    public const string DefaultGameType = "unknown";
    public const string AnonymousUser = "anonymous";

    // Service Components
    public const string GameComponent = "game";
    public const string UserComponent = "user";
    public const string DatabaseComponent = "database";
    public const string CacheComponent = "cache";

    // Debug Info
    public static void LogTelemetryConfiguration()
    {
        Console.WriteLine($"=== TELEMETRY DEBUG INFO ===");
        Console.WriteLine($"Service Name: {ServiceName}");
        Console.WriteLine($"Service Namespace: {ServiceNamespace}");
        Console.WriteLine($"Correlation Header: {CorrelationIdHeader}");
        Console.WriteLine($"Game Meter: {GameMeterName}");
        Console.WriteLine($"User Meter: {UserMeterName}");
        Console.WriteLine($"Game Activity Source: {GameActivitySource}");
        Console.WriteLine($"User Activity Source: {UserActivitySource}");
        Console.WriteLine($"============================");
    }
}
