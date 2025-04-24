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

        public static Result<Password> Create(string value)
        {
            var errorList = new List<string>();

            if (string.IsNullOrWhiteSpace(value))
                errorList.Add("Password cannot be null or empty.");

            if (value.Length < 8)
                errorList.Add("Password must be at least 8 characters long.");

            if (!UppercaseRegex.IsMatch(value))
                errorList.Add("Password must contain at least one uppercase letter.");

            if (!LowercaseRegex.IsMatch(value))
                errorList.Add("Password must contain at least one lowercase letter.");

            if (!DigitRegex.IsMatch(value))
                errorList.Add("Password must contain at least one digit.");

            if (!SpecialCharRegex.IsMatch(value))
                errorList.Add("Password must contain at least one special character.");

            if (errorList.Count != 0)
            {
                return Result<Password>.Error(new ErrorList(errorList));
            }

            return Result<Password>.Success(new Password(value));
        }

        public override string ToString() => Value;
    }
}
