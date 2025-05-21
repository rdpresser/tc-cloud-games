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

        public static Result<SystemRequirements> Create(Action<SystemRequirementsBuilder> configure)
        {
            var builder = new SystemRequirementsBuilder();
            configure(builder);
            return builder.Build();
        }

        public class SystemRequirementsBuilder
        {
            public string Minimum { get; set; } = string.Empty;
            public string? Recommended { get; set; }

            public Result<SystemRequirements> Build()
            {
                var systemRequirements = new SystemRequirements(Minimum, Recommended);
                var validator = new SystemRequirementsValidator()
                    .ValidationResult(systemRequirements);

                if (!validator.IsValid)
                {
                    return Result.Invalid(validator.AsErrors());
                }

                return systemRequirements;
            }
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Recommended) ? Minimum : $"{Minimum} (Recommended: {Recommended})";
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
