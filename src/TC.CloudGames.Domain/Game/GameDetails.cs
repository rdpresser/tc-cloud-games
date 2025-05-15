using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Infra.CrossCutting.Commons.Extensions;

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
            var gameDetails = new GameDetails(
                genre,
                JsonSerializer.Serialize(platform),
                tags,
                gameMode,
                distributionFormat,
                availableLanguages,
                supportsDlcs
            );
            var validator = new GameDetailsValidator().ValidationResult(gameDetails);

            if (!validator.IsValid)
            {
                return Result.Invalid(validator.AsErrors());
            }

            return gameDetails;
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
            ValidatePlatform();
            ValidateGameMode();
            DistributionFormat();
            SupportsDlcs();
        }

        protected void ValidatePlatform()
        {
            RuleFor(x => x.Platform)
                .NotEmpty().WithMessage("Platform is required.").WithErrorCode($"{nameof(GameDetails.Platform)}.Required")
                .Must(platform => GameDetails.ValidPlatforms.All(x => platform.Contains(x)))
                    .WithMessage($"Invalid platform specified. Valid platforms are: {GameDetails.ValidPlatforms.JoinWithQuotes()}.")
                    .WithErrorCode($"{nameof(GameDetails.Platform)}.ValidPlatform");
        }

        protected void ValidateGameMode()
        {
            RuleFor(x => x.GameMode)
                .NotEmpty().WithMessage("Game mode is required.").WithErrorCode($"{nameof(GameDetails.GameMode)}.Required")
                .Must(mode => GameDetails.ValidGameModes.Contains(mode))
                    .WithMessage($"Invalid game mode specified. Valid game modes are: {GameDetails.ValidGameModes.JoinWithQuotes()}.")
                    .WithErrorCode($"{nameof(GameDetails.GameMode)}.ValidGame");
        }

        protected void DistributionFormat()
        {
            RuleFor(x => x.DistributionFormat)
                .NotEmpty().WithMessage("Distribution format is required.").WithErrorCode($"{nameof(GameDetails.DistributionFormat)}.Required")
                .Must(format => GameDetails.ValidDistributionFormats.Contains(format))
                    .WithMessage($"Invalid distribution format specified. Valid formats are: {GameDetails.ValidDistributionFormats.JoinWithQuotes()}.")
                    .WithErrorCode($"{nameof(GameDetails.DistributionFormat)}.ValidDistributionFormat");
        }

        protected void SupportsDlcs()
        {
            RuleFor(x => x.SupportsDlcs)
                .NotNull().WithMessage("Supports DLCs field is required.")
                .WithErrorCode($"{nameof(GameDetails.SupportsDlcs)}.Required");
        }
    }
}