using Ardalis.Result;
using System.Text.RegularExpressions;

namespace TC.CloudGames.Domain.User
{
    public sealed record Email
    {
        private static readonly Regex EmailRegex =
            new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static Result<Email> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result<Email>.Invalid(new ValidationError
                {
                    Identifier = nameof(Email),
                    ErrorMessage = "Email cannot be null or empty.",
                    ErrorCode = $"{nameof(Email)}.Required"
                });
            }

            if (!EmailRegex.IsMatch(value))
            {
                return Result<Email>.Invalid(new ValidationError
                {
                    Identifier = nameof(Email),
                    ErrorMessage = "Invalid email format.",
                    ErrorCode = $"{nameof(Email)}.Invalid"
                });
            }

            return Result<Email>.Success(new Email(value));
        }

        public override string ToString() => Value;
    }
}
