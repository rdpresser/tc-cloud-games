namespace TC.CloudGames.Application.Games.CreateGame
{
    public sealed record CreateGameResponse(
        Guid Id,
        string Name,
        DateOnly ReleaseDate,
        string AgeRating,
        string? Description,
        DeveloperInfo DeveloperInfo,
        decimal DiskSize,
        decimal Price,
        Playtime? Playtime,
        GameDetails GameDetails,
        SystemRequirements SystemRequirements,
        decimal? Rating,
        string? OfficialLink,
        string? GameStatus);
}
