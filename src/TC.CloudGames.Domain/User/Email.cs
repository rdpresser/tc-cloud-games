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
                return Result<Email>.Error("Email cannot be null or empty.");

            if (!EmailRegex.IsMatch(value))
                return Result<Email>.Error("Invalid email format.");

            return Result<Email>.Success(new Email(value));
        }

        public override string ToString() => Value;
    }
}
