namespace TC.CloudGames.Domain.GameAggregate.ValueObjects
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

        public static Result<DeveloperInfo> Create(Action<DeveloperInfoBuilder> configure)
        {
            var builder = new DeveloperInfoBuilder();
            configure(builder);
            return builder.Build();
        }

        public class DeveloperInfoBuilder
        {
            public string Developer { get; set; } = string.Empty;
            public string? Publisher { get; set; }

            public Result<DeveloperInfo> Build()
            {
                var developerInfo = new DeveloperInfo(Developer, Publisher);

                var validator = new DeveloperInfoValidator().ValidationResult(developerInfo);
                if (!validator.IsValid)
                {
                    return Result.Invalid(validator.AsErrors());
                }

                return developerInfo;
            }
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Publisher) ? Developer : $"{Developer} - {Publisher}";
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
