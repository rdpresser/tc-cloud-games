using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using TC.CloudGames.Api.Telemetry;

namespace TC.CloudGames.Api.Examples;

// Example of how to use the enhanced telemetry in your endpoints
[ExcludeFromCodeCoverage]
public class ExampleGameEndpoint
{
    private readonly GameMetrics _gameMetrics;
    private readonly UserMetrics _userMetrics;
    private readonly ILogger<ExampleGameEndpoint> _logger;

    public ExampleGameEndpoint(
        GameMetrics gameMetrics,
        UserMetrics userMetrics,
        ILogger<ExampleGameEndpoint> logger)
    {
        _gameMetrics = gameMetrics;
        _userMetrics = userMetrics;
        _logger = logger;
    }

    public async Task<Microsoft.AspNetCore.Http.IResult> CreateGameAsync(string gameType, string userId)
    {
        var gameId = Guid.NewGuid().ToString();
        using var activity = ActivitySources.StartGameOperation("create", gameId, userId);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Creating game {GameId} of type {GameType} for user {UserId}",
                gameId, gameType, userId);

            // Set additional activity tags
            activity?.SetTag("game.id", gameId);
            activity?.SetTag("game.type", gameType);

            // Simulate game creation logic
            await Task.Delay(100); // Simulate work

            stopwatch.Stop();

            // Record metrics
            _gameMetrics.RecordGameCreated(gameType, userId: userId);
            _gameMetrics.RecordGameLoadTime(stopwatch.Elapsed.TotalSeconds, gameType);
            _gameMetrics.GameStarted(gameType);

            // Record user action
            _userMetrics.RecordUserAction("create_game", userId, gameType);

            activity?.SetTag("game.created", true);
            activity?.SetStatus(ActivityStatusCode.Ok);

            _logger.LogInformation("Game {GameId} created successfully in {Duration}ms",
                gameId, stopwatch.ElapsedMilliseconds);

            return Results.Ok(new { GameId = gameId, Type = gameType, Duration = stopwatch.ElapsedMilliseconds });
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // Record error metrics
            _gameMetrics.RecordGameError(gameId, gameType, ex.GetType().Name, userId);

            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("error.type", ex.GetType().Name);
            activity?.SetTag("error.message", ex.Message);

            _logger.LogError(ex, "Failed to create game of type {GameType} for user {UserId}", gameType, userId);

            return Results.Problem("Failed to create game");
        }
    }

    public async Task<Microsoft.AspNetCore.Http.IResult> CompleteGameAsync(string gameId, string gameType, double score, string userId)
    {
        using var activity = ActivitySources.StartGameOperation("complete", gameId, userId);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Completing game {GameId} with score {Score} for user {UserId}",
                gameId, score, userId);

            // Set activity tags
            activity?.SetTag("game.id", gameId);
            activity?.SetTag("game.type", gameType);
            activity?.SetTag("game.score", score);

            // Simulate game completion logic
            await Task.Delay(50);

            stopwatch.Stop();

            // Record metrics
            _gameMetrics.RecordGameCompleted(gameId, gameType, stopwatch.Elapsed.TotalSeconds, userId: userId);
            _gameMetrics.GameEnded(gameType);

            // Record user action
            _userMetrics.RecordUserAction("complete_game", userId, $"{gameType}:{score}");

            activity?.SetTag("game.completed", true);
            activity?.SetStatus(ActivityStatusCode.Ok);

            _logger.LogInformation("Game {GameId} completed successfully with score {Score} in {Duration}ms",
                gameId, score, stopwatch.ElapsedMilliseconds);

            return Results.Ok(new { GameId = gameId, Score = score, Duration = stopwatch.ElapsedMilliseconds });
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _gameMetrics.RecordGameError(gameId, gameType, ex.GetType().Name, userId);

            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("error.type", ex.GetType().Name);

            _logger.LogError(ex, "Failed to complete game {GameId} for user {UserId}", gameId, userId);

            return Results.Problem("Failed to complete game");
        }
    }
}