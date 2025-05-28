using System.Diagnostics.CodeAnalysis;
using TC.CloudGames.Domain.UserAggregate.Abstractions;

namespace TC.CloudGames.Domain.UserAggregate.ValueObjects
{
    public sealed record Email
    {
        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static async Task<Result<Email>> CreateAsync(Action<EmailBuilder> configure, IUserEfRepository userEfRepository)
        {
            var builder = new EmailBuilder();
            configure(builder);
            return await builder.Build(userEfRepository).ConfigureAwait(false);
        }

        public class EmailBuilder
        {
            public string Value { get; set; } = string.Empty;

            public async Task<Result<Email>> Build(IUserEfRepository userEfRepository)
            {
                var email = new Email(Value);
                var validator = await new EmailValidator(userEfRepository)
                    .ValidationResultAsync(email).ConfigureAwait(false);

                if (!validator.IsValid)
                {
                    return Result.Invalid(validator.AsErrors());
                }

                return email;
            }
        }

        /// <summary>
        /// Used only for EF Core mapping on User Data Configurations
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public static Result<Email> CreateMap(string value)
        {
            return new Email(value);
        }

        public override string ToString() => Value;
    }

    public class EmailValidator : BaseValidator<Email>
    {
        private readonly IUserEfRepository _userEfRepository;

        public EmailValidator(IUserEfRepository userEfRepository)
        {
            _userEfRepository = userEfRepository;
            ValidateEmail();
        }

        protected void ValidateEmail()
        {
            RuleFor(x => x.Value)
                .NotEmpty()
                    .WithMessage("Email is required.")
                    .WithErrorCode($"{nameof(Email)}.Required")
                    .OverridePropertyName(nameof(Email))
                .EmailAddress()
                    .WithMessage("Invalid email format.")
                    .WithErrorCode($"{nameof(Email)}.InvalidFormat")
                    .OverridePropertyName(nameof(Email))
                .MustAsync(async (email, cancellation) => !await _userEfRepository.EmailExistsAsync(email, cancellation).ConfigureAwait(false))
                    .WithMessage("Email already exists.")
                    .WithErrorCode($"{nameof(Email)}.AlreadyExists")
                    .OverridePropertyName(nameof(Email))
                .MaximumLength(200)
                    .WithMessage("Email cannot exceed 200 characters.")
                    .WithErrorCode($"{nameof(User.Email)}.MaximumLength")
                    .OverridePropertyName(nameof(Email));
        }
    }
}
