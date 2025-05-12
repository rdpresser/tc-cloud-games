using FastEndpoints;
using FluentValidation;
using TC.CloudGames.Domain.Game;
using TC.CloudGames.Infra.CrossCutting.Commons.Extensions;

namespace TC.CloudGames.Application.Games.CreateGame
{
    public sealed class CreateGameCommandValidator : Validator<CreateGameCommand>
    {
        public CreateGameCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Game name is required.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Name)
                        .MinimumLength(3)
                        .WithMessage("Game name must be at least 3 characters long.");
                })
                .MaximumLength(100)
                .WithMessage("Game name must not exceed 100 characters.");

            RuleFor(x => x.ReleaseDate)
                .NotEmpty()
                .WithMessage("Release date is required.")
                .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Release date must be in the past or present.");

            RuleFor(x => x.AgeRating)
                .NotEmpty()
                .WithMessage("Age rating is required.")
                .MaximumLength(10)
                .WithMessage("Age rating must not exceed 10 characters.")
                .Must(rating => AgeRating.ValidRatings.Contains(rating))
                .WithMessage($"Invalid age rating specified. Valid age rating are: {AgeRating.ValidRatings.JoinWithQuotes()}.");

            RuleFor(x => x.DeveloperInfo)
                .NotNull()
                .WithMessage("Developer information is required.")
                .DependentRules(() =>
                 {
                     RuleFor(x => x.DeveloperInfo.Developer)
                    .NotEmpty()
                    .WithMessage("Developer name is required.")
                    .MaximumLength(100)
                    .WithMessage("Developer name must not exceed 100 characters.");
                 });

            RuleFor(x => x.DiskSize)
                .NotEmpty()
                .WithMessage("Disk size is required.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.DiskSize)
                    .GreaterThan(0)
                    .WithMessage("Disk size must be greater than 0.");
                });

            RuleFor(x => x.Price)
                .NotEmpty()
                .WithMessage("Price is required.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Price)
                        .GreaterThanOrEqualTo(0)
                        .WithMessage("Price must be greater than or equal to 0.");
                });

            RuleFor(x => x.Playtime)
                .Must(playtime => playtime == null || playtime.Hours != null || playtime.PlayerCount != null)
                .WithMessage("At least one of Playtime Hours or Player Count must be provided.")
                .DependentRules(() =>
                {
                    When(x => x.Playtime?.Hours != null, () =>
                    {
                        RuleFor(x => x.Playtime.Hours)
                            .GreaterThanOrEqualTo(0)
                            .WithMessage("Playtime hours must be greater than or equal to 0.");
                    });

                    When(x => x.Playtime?.PlayerCount != null, () =>
                    {
                        RuleFor(x => x.Playtime.PlayerCount)
                            .GreaterThanOrEqualTo(1)
                            .WithMessage("Player count must be greater than or equal to 1.");
                    });
                });

            RuleFor(x => x.GameDetails)
                .NotNull()
                .WithMessage("Game details are required.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.GameDetails.Platform)
                        .NotEmpty()
                        .WithMessage("Platform is required.")
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.GameDetails.Platform)
                                .Must(platform => Domain.Game.GameDetails.ValidPlatforms.All(x => platform.Contains(x)))
                                .WithMessage($"Invalid platform specified. Valid platforms are: {Domain.Game.GameDetails.ValidPlatforms.JoinWithQuotes()}.");
                        });

                    RuleFor(x => x.GameDetails.GameMode)
                        .NotEmpty()
                        .WithMessage("Game mode is required.")
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.GameDetails.GameMode)
                                .Must(mode => Domain.Game.GameDetails.ValidGameModes.Contains(mode))
                                .WithMessage($"Invalid game mode specified. Valid game modes are: {Domain.Game.GameDetails.ValidGameModes.JoinWithQuotes()}.");
                        });

                    RuleFor(x => x.GameDetails.DistributionFormat)
                        .NotEmpty()
                        .WithMessage("Distribution format is required.")
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.GameDetails.DistributionFormat)
                                .Must(format => Domain.Game.GameDetails.ValidDistributionFormats.Contains(format))
                                .WithMessage($"Invalid distribution format specified. Valid formats are: {Domain.Game.GameDetails.ValidDistributionFormats.JoinWithQuotes()}.");
                        });

                    RuleFor(x => x.GameDetails.SupportsDlcs)
                        .NotNull()
                        .WithMessage("Supports DLCs field is required.");
                });

            RuleFor(x => x.SystemRequirements)
                .NotNull()
                .WithMessage("System requirements are required.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.SystemRequirements.Minimum)
                        .NotEmpty()
                        .WithMessage("Minimum system requirements are required.")
                        .MaximumLength(1000)
                        .WithMessage("Minimum system requirements must not exceed 1000 characters.");
                });

            RuleFor(x => x.Rating)
                .Must(rating => rating == null || (rating >= 0 && rating <= 10))
                .WithMessage("Rating must be null or between 0 and 10.");

            RuleFor(x => x.GameStatus)
                .Must(status => status == null || Game.ValidGameStatus.Contains(status))
                .WithMessage($"Invalid game status specified. Valid status are: {Game.ValidGameStatus.JoinWithQuotes()}.")
                .MaximumLength(50)
                .When(x => x.GameStatus != null)
                .WithMessage("Game status must not exceed 50 characters.");
        }
    }
}
