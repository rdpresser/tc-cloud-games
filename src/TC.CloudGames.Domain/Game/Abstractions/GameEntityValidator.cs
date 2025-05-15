using FluentValidation;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Infra.CrossCutting.Commons.Extensions;

namespace TC.CloudGames.Domain.Game.Abstractions
{
    public class GameEntityValidator : BaseValidator<Game>
    {
        protected void ValidateName()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Game name is required.")
                .WithErrorCode($"{nameof(Game.Name)}.Required")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Name)
                        .Length(1, 200).WithMessage("Name must be between 1 and 200 characters.")
                        .WithErrorCode($"{nameof(Game.Name)}.Length");
                });
        }

        protected void ValidateReleaseDate()
        {
            RuleFor(x => x.ReleaseDate)
                .NotEmpty().WithMessage("Release date is required.")
                .WithErrorCode($"{nameof(Game.ReleaseDate)}.Required")
                .Must(date => date > DateOnly.MinValue).WithMessage("Release date must be a valid date.")
                    .WithErrorCode($"{nameof(Game.ReleaseDate)}.ValidDate");
        }

        protected void ValidateAgeRating()
        {
            RuleFor(x => x.AgeRating)
                .NotEmpty().WithMessage("Age rating is required.")
                .WithErrorCode($"{nameof(Game.AgeRating)}.Required");
        }

        protected void ValidateDescription()
        {
            RuleFor(game => game.Description)
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.")
                .WithErrorCode($"{nameof(Game.Description)}.MaximumLength");
        }

        protected void ValidateDeveloperInfo()
        {
            RuleFor(x => x.DeveloperInfo)
                .NotNull().WithMessage("Developer information is required.").WithErrorCode($"{nameof(Game.DeveloperInfo)}.Required");
        }

        protected void ValidateDiskSize()
        {
            RuleFor(x => x.DiskSize)
                .NotEmpty().WithMessage("Disk size is required.").WithErrorCode($"{nameof(Game.DiskSize)}.Required");
        }

        protected void ValidatePrice()
        {
            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Price is required.").WithErrorCode($"{nameof(Game.Price)}.Required");
        }

        protected void ValidatePlaytime()
        {
            RuleFor(x => x.Playtime)
                .Must(playtime => playtime == null || playtime.Hours != null || playtime.PlayerCount != null)
                .WithMessage("Playtime Hours or Player Count must be provided if specified.").WithErrorCode($"{nameof(Game.Playtime)}.RequiredConditional");
        }

        protected void ValidateGameDetails()
        {
            RuleFor(x => x.GameDetails)
                .NotNull().WithMessage("Game details are required.").WithErrorCode($"{nameof(Game.GameDetails)}.Required");
        }

        protected void ValidateSystemRequirements()
        {
            RuleFor(x => x.SystemRequirements)
                .NotNull().WithMessage("System requirements are required.").WithErrorCode($"{nameof(Game.SystemRequirements)}.Required")
                .DependentRules(() =>
                {
                    RuleFor(x => x.SystemRequirements.Minimum)
                        .NotEmpty().WithMessage("Minimum system requirements are required.").WithErrorCode($"{nameof(Game.SystemRequirements.Minimum)}.Required")
                        .MaximumLength(1000).WithMessage("Minimum system requirements must not exceed 1000 characters.")
                        .WithErrorCode($"{nameof(Game.SystemRequirements.Minimum)}.MaximumLength");
                });
        }

        protected void ValidateRating()
        {
            When(x => x.Rating != null && x.Rating.Average.HasValue, () =>
            {
                RuleFor(x => x.Rating!.Average)
                    .GreaterThan(0).WithMessage("Rating must be greater than 0.")
                    .WithErrorCode($"{nameof(Game.Rating)}.GreaterThanZero");
            });
        }

        protected void ValidateGameStatus()
        {
            When(x => x.GameStatus != null, () =>
            {
                RuleFor(x => x.GameStatus)
                    .Must(status => Game.ValidGameStatus.Contains(status!))
                    .WithMessage($"Invalid game status specified. Valid status are: {Game.ValidGameStatus.JoinWithQuotes()}.")
                    .WithErrorCode($"{nameof(Game.GameStatus)}.ValidGameStatus")
                .MaximumLength(200).WithMessage("Game status must not exceed 200 characters.")
                .WithErrorCode($"{nameof(Game.GameStatus)}.MaximumLength");
            });
        }

        protected void ValidateOfficialLink()
        {
            RuleFor(game => game.OfficialLink)
                .Must(link => string.IsNullOrEmpty(link) || Uri.IsWellFormedUriString(link, UriKind.Absolute))
                .WithMessage("Official link must be a valid URL.")
                .WithErrorCode($"{nameof(Game.OfficialLink)}.ValidUrl");
        }
    }
}
