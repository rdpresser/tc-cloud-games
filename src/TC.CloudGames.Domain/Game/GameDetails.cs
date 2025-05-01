using Ardalis.Result;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using TC.CloudGames.CrossCutting.Commons.Extensions;

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
            List<ValidationError> validation = [];

            if (!ValidPlatforms.Any(platform.Contains))
            {
                validation.Add(new()
                {
                    Identifier = nameof(Platform),
                    ErrorMessage = $"Invalid platform specified. Valid platforms are: {ValidPlatforms.JoinWithQuotes()}.",
                    ErrorCode = $"{nameof(Platform)}.Invalid"
                });
            }

            if (string.IsNullOrWhiteSpace(gameMode) || !ValidGameModes.Contains(gameMode))
            {
                validation.Add(new()
                {
                    Identifier = nameof(GameMode),
                    ErrorMessage = $"Invalid game mode specified. Valid game modes are: {ValidGameModes.JoinWithQuotes()}.",
                    ErrorCode = $"{nameof(GameMode)}.Invalid"
                });
            }

            if (string.IsNullOrWhiteSpace(distributionFormat) || !ValidDistributionFormats.Contains(distributionFormat))
            {
                validation.Add(new()
                {
                    Identifier = nameof(DistributionFormat),
                    ErrorMessage = $"Invalid distribution format specified. Valid formats are: {ValidDistributionFormats.JoinWithQuotes()}.",
                    ErrorCode = $"{nameof(DistributionFormat)}.Invalid"
                });
            }

            if (validation.Count != 0)
            {
                return Result<GameDetails>.Invalid(validation);
            }

            return new GameDetails(
                genre,
                JsonSerializer.Serialize(platform),
                tags,
                gameMode,
                distributionFormat,
                availableLanguages,
                supportsDlcs
            );
        }

        public override string ToString()
        {
            return $"{Platform} - {GameMode} - {DistributionFormat}";
        }
    }
}