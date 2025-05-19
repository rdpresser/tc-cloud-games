namespace TC.CloudGames.Domain.Game
{
    public sealed record SystemRequirements
    {
        public string Minimum { get; }
        public string? Recommended { get; }

        private SystemRequirements(string minimum, string? recommended)
        {
            Minimum = minimum;
            Recommended = recommended;
        }

        public static Result<SystemRequirements> Create(string minimum, string? recommended)
        {
            var systemRequirements = new SystemRequirements(minimum, recommended);
            var validator = new SystemRequirementsValidator()
                .ValidationResult(systemRequirements);

            if (!validator.IsValid)
            {
                return Result.Invalid(validator.AsErrors());
            }

            return systemRequirements;
        }
    }

    public class SystemRequirementsValidator : BaseValidator<SystemRequirements>
    {
        public SystemRequirementsValidator()
        {
            ValidateMinimum();
        }

        protected void ValidateMinimum()
        {
            RuleFor(x => x.Minimum)
                .NotEmpty()
                    .WithMessage("Minimum system requirements are required.")
                    .WithErrorCode($"{nameof(SystemRequirements.Minimum)}.Required")
                .MaximumLength(1000)
                    .WithMessage("Minimum system requirements must not exceed 1000 characters.")
                    .WithErrorCode($"{nameof(SystemRequirements.Minimum)}.MaximumLength");

            When(x => x.Recommended != null, () =>
            {
                RuleFor(x => x.Recommended)
                    .MaximumLength(1000)
                        .WithMessage("Recommended system requirements must not exceed 1000 characters.")
                        .WithErrorCode($"{nameof(SystemRequirements.Recommended)}.MaximumLength");
            });
        }
    }
}
