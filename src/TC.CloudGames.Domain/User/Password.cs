using Ardalis.Result;
using System.Text.RegularExpressions;

namespace TC.CloudGames.Domain.User
{
    public sealed record Password
    {
        private static readonly Regex UppercaseRegex = new(@"[A-Z]", RegexOptions.Compiled);
        private static readonly Regex LowercaseRegex = new(@"[a-z]", RegexOptions.Compiled);
        private static readonly Regex DigitRegex = new(@"\d", RegexOptions.Compiled);
        private static readonly Regex SpecialCharRegex = new(@"[\W_]", RegexOptions.Compiled);

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
        public static Result<Password> CreateHashed(string value)
        {
            return Result<Password>.Success(new Password(value));
        }

        public static Result<Password> Create(string value)
        {
            List<ValidationError> validation = [];

            if (string.IsNullOrWhiteSpace(value))
            {
                validation.Add(new()
                {
                    Identifier = nameof(Password),
                    ErrorMessage = "Password cannot be null or empty.",
                    ErrorCode = $"{nameof(Password)}.Required"
                });
            }

            if (value.Length < 8)
            {
                validation.Add(new()
                {
                    Identifier = nameof(Password),
                    ErrorMessage = "Password must be at least 8 characters long.",
                    ErrorCode = $"{nameof(Password)}.Invalid"
                });
            }

            if (!UppercaseRegex.IsMatch(value))
            {
                validation.Add(new()
                {
                    Identifier = nameof(Password),
                    ErrorMessage = "Password must contain at least one uppercase letter.",
                    ErrorCode = $"{nameof(Password)}.Invalid"
                });
            }

            if (!LowercaseRegex.IsMatch(value))
            {
                validation.Add(new()
                {
                    Identifier = nameof(Password),
                    ErrorMessage = "Password must contain at least one lowercase letter.",
                    ErrorCode = $"{nameof(Password)}.Invalid"
                });
            }

            if (!DigitRegex.IsMatch(value))
            {
                validation.Add(new()
                {
                    Identifier = nameof(Password),
                    ErrorMessage = "Password must contain at least one digit.",
                    ErrorCode = $"{nameof(Password)}.Invalid"
                });
            }

            if (!SpecialCharRegex.IsMatch(value))
            {
                validation.Add(new()
                {
                    Identifier = nameof(Password),
                    ErrorMessage = "Password must contain at least one special character.",
                    ErrorCode = $"{nameof(Password)}.Invalid"
                });
            }

            if (validation.Count != 0)
            {
                return Result<Password>.Invalid(validation);
            }

            return Result<Password>.Success(new Password(value));
        }

        public override string ToString() => Value;
    }
}
