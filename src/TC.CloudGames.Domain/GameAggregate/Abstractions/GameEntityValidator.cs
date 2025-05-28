using TC.CloudGames.Infra.CrossCutting.Commons.Extensions;

namespace TC.CloudGames.Domain.GameAggregate.Abstractions
{
    public class GameEntityValidator : BaseValidator<Game>
    {
        protected void ValidateName()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                    .WithMessage("Game name is required.")
                    .WithErrorCode($"{nameof(Game.Name)}.Required")
                .MaximumLength(200)
                    .WithMessage("Name cannot exceed 200 characters.")
                    .WithErrorCode($"{nameof(Game.Name)}.MaximumLength");
        }

        protected void ValidateReleaseDate()
        {
            RuleFor(x => x.ReleaseDate)
                .NotEmpty()
                    .WithMessage("Release date is required.")
                    .WithErrorCode($"{nameof(Game.ReleaseDate)}.Required")
                .Must(date => date > DateOnly.MinValue)
                    .WithMessage("Release date must be a valid date.")
                    .WithErrorCode($"{nameof(Game.ReleaseDate)}.ValidDate");
        }

        protected void ValidateAgeRating()
        {
            RuleFor(x => x.AgeRating)
                .NotEmpty()
                    .WithMessage("Age rating object is required.")
                    .WithErrorCode($"{nameof(Game.AgeRating)}.Required");
        }

        protected void ValidateDescription()
        {
            RuleFor(game => game.Description)
                .MaximumLength(2000)
                    .WithMessage("Description cannot exceed 2000 characters.")
                    .WithErrorCode($"{nameof(Game.Description)}.MaximumLength");
        }

        protected void ValidateDeveloperInfo()
        {
            RuleFor(x => x.DeveloperInfo)
                .NotNull()
                    .WithMessage("Developer information object is required.")
                    .WithErrorCode($"{nameof(Game.DeveloperInfo)}.Required");
        }

        protected void ValidateDiskSize()
        {
            RuleFor(x => x.DiskSize)
                .NotNull()
                    .WithMessage("Disk size object is required.")
                    .WithErrorCode($"{nameof(Game.DiskSize)}.Required");
        }

        protected void ValidatePrice()
        {
            RuleFor(x => x.Price)
                .NotNull()
                    .WithMessage("Price object is required.")
                    .WithErrorCode($"{nameof(Game.Price)}.Required");
        }

        protected void ValidatePlaytime()
        {
            RuleFor(x => x.Playtime)
                .Must(playtime => playtime == null || playtime.Hours != null || playtime.PlayerCount != null)
                    .WithMessage("Playtime Hours or Player Count must be provided if playtime object specified.")
                    .WithErrorCode($"{nameof(Game.Playtime)}.RequiredConditional");
        }

        protected void ValidateGameDetails()
        {
            RuleFor(x => x.GameDetails)
                .NotNull()
                    .WithMessage("Game details object are required.")
                    .WithErrorCode($"{nameof(Game.GameDetails)}.Required");
        }

        protected void ValidateSystemRequirements()
        {
            RuleFor(x => x.SystemRequirements)
                .NotNull().WithMessage("System requirements object are required.").WithErrorCode($"{nameof(Game.SystemRequirements)}.Required");
        }

        protected void ValidateRating()
        {
            //because Rating is nullable, agreggate root should not validate something that the value object it self should validate
            //When(x => x.Rating != null && x.Rating.Average.HasValue, () =>
            //{
            //    RuleFor(x => x.Rating!.Average)
            //        .GreaterThan(0)
            //            .WithMessage("Rating must be greater than 0.")
            //            .WithErrorCode($"{nameof(Game.Rating)}.GreaterThanZero");
            //});
        }

        protected void ValidateGameStatus()
        {
            When(x => x.GameStatus != null, () =>
            {
                RuleFor(x => x.GameStatus)
                    .Must(status => Game.ValidGameStatus.Contains(status!))
                        .WithMessage($"Invalid game status specified. Valid status are: {Game.ValidGameStatus.JoinWithQuotes()}.")
                        .WithErrorCode($"{nameof(Game.GameStatus)}.ValidGameStatus")
                    .MaximumLength(200)
                        .WithMessage("Game status must not exceed 200 characters.")
                        .WithErrorCode($"{nameof(Game.GameStatus)}.MaximumLength");
            });
        }

        protected void ValidateOfficialLink()
        {
            When(x => !string.IsNullOrWhiteSpace(x.OfficialLink), () =>
            {
                RuleFor(game => game.OfficialLink)
                    .MaximumLength(200)
                        .WithMessage("Official link must not exceed 200 characters.")
                        .WithErrorCode($"{nameof(Game.OfficialLink)}.MaximumLength")
                    .Must(link => Uri.IsWellFormedUriString(link, UriKind.Absolute))
                        .WithMessage("Official link must be a valid URL.")
                        .WithErrorCode($"{nameof(Game.OfficialLink)}.ValidUrl");
            });
        }
    }
}
