using Ardalis.Result;

namespace TC.CloudGames.Domain.Game
{
    public sealed record GameDetails
    {
        public string? Genre { get; }
        public string Platform { get; }
        public string? Tags { get; }
        public string? GameMode { get; }
        public string? DistributionFormat { get; }
        public string? AvailableLanguages { get; }
        public bool SupportsDlcs { get; }

        private static readonly HashSet<string> ValidPlatforms = new()
           {
               "Windows", "iOS", "Linux", "Android", "PlayStation", "Xbox", "Nintendo"
           };

        private static readonly HashSet<string> ValidGameModes = new()
           {
               "Singleplayer", "Multiplayer", "Co-op", "Online"
           };

        private static readonly HashSet<string> ValidDistributionFormats = new()
           {
               "Digital", "Physical"
           };

        private GameDetails(
            string? genre,
            string platform,
            string? tags,
            string? gameMode,
            string? distributionFormat,
            string? availableLanguages,
            bool supportsDlcs
        )
        {
            Genre = genre;
            Platform = platform;
            Tags = tags;
            GameMode = gameMode;
            DistributionFormat = distributionFormat;
            AvailableLanguages = availableLanguages;
            SupportsDlcs = supportsDlcs;
        }

        public static Result<GameDetails> Create(
            string? genre,
            string platform,
            string? tags,
            string? gameMode,
            string? distributionFormat,
            string? availableLanguages,
            bool supportsDlcs
        )
        {
            if (!ValidPlatforms.Contains(platform))
            {
                return Result<GameDetails>.Error($"Invalid platform. Valid platforms are: {string.Join(", ", ValidPlatforms)}.");
            }

            if (!string.IsNullOrEmpty(gameMode) && !ValidGameModes.Contains(gameMode))
            {
                return Result<GameDetails>.Error($"Invalid game mode. Valid game modes are: {string.Join(", ", ValidGameModes)}.");
            }

            if (!string.IsNullOrEmpty(distributionFormat) && !ValidDistributionFormats.Contains(distributionFormat))
            {
                return Result<GameDetails>.Error($"Invalid distribution format. Valid formats are: {string.Join(", ", ValidDistributionFormats)}.");
            }

            return Result<GameDetails>.Success(new GameDetails(
                genre,
                platform,
                tags,
                gameMode,
                distributionFormat,
                availableLanguages,
                supportsDlcs
            ));
        }

        public override string ToString()
        {
            return $"{Platform} - {GameMode} - {DistributionFormat}";
        }
    }
}
