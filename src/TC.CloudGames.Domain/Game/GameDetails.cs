using Ardalis.Result;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace TC.CloudGames.Domain.Game
{
    public record GameDetails
    {
        public string? Genre { get; }

        [NotMapped]
        public List<string> PlatformList
        {
            get
            {
                if (Platform == null)
                {
                    return [];
                }
                return JsonSerializer.Deserialize<List<string>>(Platform) ?? [];
            }
            set
            {
                if (value == null)
                {
                    Platform = string.Empty;
                }
                else
                {
                    Platform = JsonSerializer.Serialize(value);
                }
            }
        }

        public string Platform { get; private set; }
        public string? Tags { get; }
        public string GameMode { get; } //TODO: mudar para array 
        public string DistributionFormat { get; } //TODO: mudar para array 
        public string? AvailableLanguages { get; }
        public bool SupportsDlcs { get; }

        public static readonly IImmutableSet<string> ValidPlatforms = ImmutableHashSet.Create(
            "Windows", "iOS", "Linux", "Android", "PlayStation", "Xbox", "Nintendo"
        );

        public static readonly IImmutableSet<string> ValidGameModes = ImmutableHashSet.Create(
            "Singleplayer", "Multiplayer", "Co-op", "Online"
        );

        public static readonly IImmutableSet<string> ValidDistributionFormats = ImmutableHashSet.Create(
            "Digital", "Physical"
        );

        private GameDetails(
            string? genre,
            string platform,
            string? tags,
            string gameMode,
            string distributionFormat,
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
            string[] platform,
            string? tags,
            string gameMode,
            string distributionFormat,
            string? availableLanguages,
            bool supportsDlcs
        )
        {
            var errorList = new List<string>();

            if (platform != null && !ValidPlatforms.Any(x => platform.Contains(x)))
            {
                errorList.Add($"Invalid platform specified. Valid platforms are: {string.Join(", ", ValidPlatforms)}.");
            }

            if (string.IsNullOrWhiteSpace(gameMode) || !ValidGameModes.Contains(gameMode))
            {
                errorList.Add($"Invalid game mode specified. Valid game modes are: {string.Join(", ", ValidGameModes)}.");
            }

            if (string.IsNullOrWhiteSpace(distributionFormat) || !ValidDistributionFormats.Contains(distributionFormat))
            {
                errorList.Add($"Invalid distribution format specified. Valid formats are: {string.Join(", ", ValidDistributionFormats)}.");
            }

            if (errorList.Count != 0)
            {
                return Result<GameDetails>.Error(new ErrorList(errorList));
            }

            return Result<GameDetails>.Success(new GameDetails(
                genre,
                JsonSerializer.Serialize(platform),
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