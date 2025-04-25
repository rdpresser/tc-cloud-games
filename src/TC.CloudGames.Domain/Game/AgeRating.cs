using Ardalis.Result;

namespace TC.CloudGames.Domain.Game
{
    public sealed record AgeRating
    {
        public string Value { get; init; }

        public static readonly HashSet<string> ValidRatings =
           [
               "E", "E10+", "T", "M", "A", "RP"
           ];

        private AgeRating(string value) => Value = value;

        public static Result<AgeRating> Create(string value)
        {
            if (!ValidRatings.Contains(value))
            {
                return Result<AgeRating>.Error($"Invalid age rating: {value}. Valid ratings are: {string.Join(", ", ValidRatings)}");
            }

            return Result<AgeRating>.Success(new AgeRating(value));
        }

        public override string ToString() => Value;
    }
}
