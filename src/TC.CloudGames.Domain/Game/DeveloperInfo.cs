using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using TC.CloudGames.Domain.Abstractions;

namespace TC.CloudGames.Domain.Game
{
    public sealed record DeveloperInfo
    {
        public string Developer { get; }
        public string? Publisher { get; }

        private DeveloperInfo(string developer, string? publisher)
        {
            Developer = developer;
            Publisher = publisher;
        }

        public static Result<DeveloperInfo> Create(string developer, string? publisher)
        {
            var developerInfo = new DeveloperInfo(developer, publisher);
            var validator = new DeveloperInfoValidator()
                .ValidationResult(developerInfo);

            if (!validator.IsValid)
            {
                return Result.Invalid(validator.AsErrors());
            }

            return developerInfo;
        }
    }

    public class DeveloperInfoValidator : BaseValidator<DeveloperInfo>
    {
        public DeveloperInfoValidator()
        {
            ValidateDeveloperInfo();
            ValidatePublisherInfo();
        }

        protected void ValidateDeveloperInfo()
        {
            RuleFor(x => x.Developer)
                .NotEmpty()
                    .WithMessage("Developer name is required.")
                    .WithErrorCode($"{nameof(DeveloperInfo.Developer)}.Required")
                .MaximumLength(100)
                    .WithMessage("Developer name must not exceed 100 characters.")
                    .WithErrorCode($"{nameof(DeveloperInfo.Developer)}.MaximumLength");
        }

        protected void ValidatePublisherInfo()
        {
            When(x => x.Publisher != null, () =>
            {
                RuleFor(x => x.Publisher)
                    .MaximumLength(200)
                        .WithMessage("Publisher name must not exceed 200 characters.")
                        .WithErrorCode($"{nameof(DeveloperInfo.Publisher)}.MaximumLength");
            });
        }
    }
}
