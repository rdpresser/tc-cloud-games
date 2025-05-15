using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Domain.User.Abstractions;

namespace TC.CloudGames.Domain.User
{
    public sealed record Email
    {
        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static async Task<Result<Email>> Create(string value, IUserEfRepository userEfRepository)
        {
            var email = new Email(value);
            var validator = await new EmailValidator(userEfRepository)
                .ValidationResultAsync(email);

            if (!validator.IsValid)
            {
                return Result.Invalid(validator.AsErrors());
            }

            return email;
        }

        /// <summary>
        /// Used only for EF Core mapping on User Data Configurations
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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
                .MustAsync(async (email, cancellation) => !await _userEfRepository.EmailExistsAsync(email, cancellation))
                    .WithMessage("Email already exists.")
                    .WithErrorCode($"{nameof(Email)}.AlreadyExists")
                    .OverridePropertyName(nameof(Email));
        }
    }
}
