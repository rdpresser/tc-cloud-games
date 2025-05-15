using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using TC.CloudGames.Domain.Abstractions;

namespace TC.CloudGames.Domain.User
{
    public sealed record Password
    {
        public string Value { get; }

        private Password(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Used when the password is already hashed. Never use this method to create a password, only when retrieve from database
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Result<Password> CreateMap(string value)
        {
            return new Password(value);
        }

        public static Result<Password> Create(string value)
        {
            var password = new Password(value);
            var validator = new PasswordValidator().ValidationResult(password);

            if (!validator.IsValid)
            {
                return Result.Invalid(validator.AsErrors());
            }

            return password;
        }

        public override string ToString() => Value;
    }

    public class PasswordValidator : BaseValidator<Password>
    {
        public PasswordValidator()
        {
            ValidatePassword();
        }

        protected void ValidatePassword()
        {
            RuleFor(x => x.Value)
                .NotEmpty()
                    .WithMessage("Password is required.")
                    .WithErrorCode($"{nameof(Password)}.Required")
                    .OverridePropertyName(nameof(Password))
                .MinimumLength(8)
                    .WithMessage("Password must be at least 8 characters long.")
                    .WithErrorCode($"{nameof(Password)}.MinimumLength")
                    .OverridePropertyName(nameof(Password))
                .Matches(@"[A-Z]")
                    .WithMessage("Password must contain at least one uppercase letter.")
                    .WithErrorCode($"{nameof(Password)}.Uppercase")
                    .OverridePropertyName(nameof(Password))
                .Matches(@"[a-z]")
                    .WithMessage("Password must contain at least one lowercase letter.")
                    .WithErrorCode($"{nameof(Password)}.Lowercase")
                    .OverridePropertyName(nameof(Password))
                .Matches(@"[0-9]")
                    .WithMessage("Password must contain at least one digit.")
                    .WithErrorCode($"{nameof(Password)}.Digit")
                    .OverridePropertyName(nameof(Password))
                .Matches(@"[\W_]")
                    .WithMessage("Password must contain at least one special character.")
                    .WithErrorCode($"{nameof(Password)}.SpecialCharacter")
                    .OverridePropertyName(nameof(Password));
        }
    }
}
