using Ardalis.Result;
using TC.CloudGames.CrossCutting.Commons.Extensions;

namespace TC.CloudGames.Domain.Game
{
    public sealed record AgeRating
    {
        public string Value { get; }

        public static readonly HashSet<string> ValidRatings =
           [
               "E", "E10+", "T", "M", "A", "RP"
           ];

        private AgeRating(string value) => Value = value;

        public static Result<AgeRating> Create(string value)
        {
            if (!ValidRatings.Contains(value))
            {
                return Result<AgeRating>.Invalid(new ValidationError()
                {
                    Identifier = nameof(AgeRating),
                    ErrorMessage = $"Invalid age rating: {value}. Valid ratings are: {ValidRatings.JoinWithQuotes()}",
                    ErrorCode = $"{nameof(AgeRating)}.Invalid"
                });
            }

            return Result<AgeRating>.Success(new AgeRating(value));
        }

        public override string ToString() => Value;
    }
}
