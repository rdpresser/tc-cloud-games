using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using TC.CloudGames.Infra.CrossCutting.Commons.Extensions;

namespace TC.CloudGames.Domain.Game
{
    public sealed record GameDetails
    {
        public string? Genre { get; }

        [NotMapped]
        [ExcludeFromCodeCoverage]
        public IReadOnlyCollection<string> PlatformList
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
            "PC", "PlayStation 4", "PlayStation 5", "Xbox One", "Xbox Series X|S", "Nintendo Switch",
            "Nintendo 3DS", "Wii U", "PlayStation Vita", "Android", "iOS", "macOS", "Linux", "Stadia", "Steam Deck", "Browser",
            "VR (Oculus Quest)", "VR (HTC Vive)", "VR (PlayStation VR)"
        );

        public static readonly IImmutableSet<string> ValidGameModes = ImmutableHashSet.Create(
            "Singleplayer", "Multiplayer", "Co-op", "PvP", "PvE", "Battle Royale", "Survival",
            "Sandbox", "Casual"
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

        /// <summary>
        /// Builder pattern for GameDetails.
        /// </summary>
        public static Result<GameDetails> Create(Action<GameDetailsBuilder> configure)
        {
            var builder = new GameDetailsBuilder();
            configure(builder);
            return builder.Build();
        }

        public class GameDetailsBuilder
        {
            public string? Genre { get; set; }
            public string[] Platform { get; set; } = [];
            public string? Tags { get; set; }
            public string GameMode { get; set; } = string.Empty;
            public string DistributionFormat { get; set; } = string.Empty;
            public string? AvailableLanguages { get; set; }
            public bool SupportsDlcs { get; set; }

            public Result<GameDetails> Build()
            {
                var gameDetails = new GameDetails(
                    Genre,
                    JsonSerializer.Serialize(Platform),
                    Tags,
                    GameMode,
                    DistributionFormat,
                    AvailableLanguages,
                    SupportsDlcs
                );

                var validator = new GameDetailsValidator().ValidationResult(gameDetails);
                if (!validator.IsValid)
                {
                    return Result.Invalid(validator.AsErrors());
                }

                return gameDetails;
            }
        }

        public override string ToString()
        {
            return $"{Platform} - {GameMode} - {DistributionFormat}";
        }
    }

    public class GameDetailsValidator : BaseValidator<GameDetails>
    {
        public GameDetailsValidator()
        {
            ValidateGenre();
            ValidatePlatform();
            ValidateTags();
            ValidateGameMode();
            ValidateDistributionFormat();
            ValidateAvailableLanguages();
            ValidateSupportsDlcs();
        }

        protected void ValidateGenre()
        {
            When(x => x.Genre != null, () =>
            {
                RuleFor(x => x.Genre)
                    .MaximumLength(50)
                        .WithMessage("Genre must not exceed 50 characters.")
                        .WithErrorCode($"{nameof(GameDetails.Genre)}.MaximumLength");
            });
        }

        protected void ValidatePlatform()
        {
            RuleFor(x => x.PlatformList)
                .NotEmpty()
                    .WithMessage("Platform is required.")
                    .WithErrorCode($"{nameof(GameDetails.Platform)}.Required")
                    .OverridePropertyName(nameof(GameDetails.Platform));

            RuleFor(x => x.PlatformList)
                .Must(platform => platform.All(x => GameDetails.ValidPlatforms.Contains(x)))
                    .WithMessage($"Invalid platform specified. Valid platforms are: {GameDetails.ValidPlatforms.JoinWithQuotes()}.")
                    .WithErrorCode($"{nameof(GameDetails.Platform)}.ValidPlatform")
                    .OverridePropertyName(nameof(GameDetails.Platform));
        }

        protected void ValidateTags()
        {
            When(x => x.Tags != null, () =>
            {
                RuleFor(x => x.Tags)
                    .MaximumLength(200)
                        .WithMessage("Tags must not exceed 200 characters.")
                        .WithErrorCode($"{nameof(GameDetails.Tags)}.MaximumLength");
            });
        }

        protected void ValidateGameMode()
        {
            RuleFor(x => x.GameMode)
                .NotEmpty()
                    .WithMessage("Game mode is required.")
                    .WithErrorCode($"{nameof(GameDetails.GameMode)}.Required")
                .Must(mode => GameDetails.ValidGameModes.Contains(mode))
                    .WithMessage($"Invalid game mode specified. Valid game modes are: {GameDetails.ValidGameModes.JoinWithQuotes()}.")
                    .WithErrorCode($"{nameof(GameDetails.GameMode)}.ValidGameMode");
        }

        protected void ValidateDistributionFormat()
        {
            RuleFor(x => x.DistributionFormat)
                .NotEmpty()
                    .WithMessage("Distribution format is required.")
                    .WithErrorCode($"{nameof(GameDetails.DistributionFormat)}.Required")
                .Must(format => GameDetails.ValidDistributionFormats.Contains(format))
                    .WithMessage($"Invalid distribution format specified. Valid formats are: {GameDetails.ValidDistributionFormats.JoinWithQuotes()}.")
                    .WithErrorCode($"{nameof(GameDetails.DistributionFormat)}.ValidDistributionFormat");
        }

        protected void ValidateAvailableLanguages()
        {
            When(x => x.AvailableLanguages != null, () =>
            {
                RuleFor(x => x.AvailableLanguages)
                    .MaximumLength(100)
                        .WithMessage("Available languages must not exceed 100 characters.")
                        .WithErrorCode($"{nameof(GameDetails.AvailableLanguages)}.MaximumLength");
            });
        }

        protected void ValidateSupportsDlcs()
        {
            RuleFor(x => x.SupportsDlcs)
                .NotNull().WithMessage("Supports DLCs field is required.")
                .WithErrorCode($"{nameof(GameDetails.SupportsDlcs)}.Required");
        }
    }
}