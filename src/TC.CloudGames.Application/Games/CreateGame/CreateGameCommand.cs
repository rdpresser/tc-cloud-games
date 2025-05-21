namespace TC.CloudGames.Application.Games.CreateGame
{
    public sealed record CreateGameCommand(
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
        string? GameStatus)
        : Abstractions.Messaging.ICommand<CreateGameResponse>
    {
        public static CreateGameCommand Create(Action<CreateGameCommandBuilder> configure)
        {
            var builder = new CreateGameCommandBuilder();
            configure(builder);
            return builder.Build();
        }

        public class CreateGameCommandBuilder
        {
            public string Name { get; set; } = string.Empty;
            public DateOnly ReleaseDate { get; set; }
            public string AgeRating { get; set; } = string.Empty;
            public string? Description { get; set; }
            public DeveloperInfo DeveloperInfo { get; set; } = new("", null);
            public decimal DiskSize { get; set; }
            public decimal Price { get; set; }
            public Playtime Playtime { get; set; } = new(null, null);
            public GameDetails GameDetails { get; set; } = new(null, [], null, "", "", null, false);
            public SystemRequirements SystemRequirements { get; set; } = new("", null);
            public decimal? Rating { get; set; }
            public string? OfficialLink { get; set; }
            public string? GameStatus { get; set; }

            public CreateGameCommand Build() =>
                new(
                    Name,
                    ReleaseDate,
                    AgeRating,
                    Description,
                    DeveloperInfo,
                    DiskSize,
                    Price,
                    Playtime,
                    GameDetails,
                    SystemRequirements,
                    Rating,
                    OfficialLink,
                    GameStatus
                );
        }
    }

    public sealed record DeveloperInfo(string Developer, string? Publisher);

    public sealed record Price(decimal Amount);

    public sealed record Playtime(int? Hours, int? PlayerCount);

    public sealed record SystemRequirements(string Minimum, string? Recommended);

    public sealed record GameDetails(
        string? Genre,
        string[] Platform,
        string? Tags,
        string GameMode,
        string DistributionFormat,
        string? AvailableLanguages,
        bool SupportsDlcs
    );
}
