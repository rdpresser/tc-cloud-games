using Ardalis.Result;

namespace TC.CloudGames.Domain.Game
{
    public sealed record Rating
    {
        public decimal? Average { get; }

        private Rating(decimal? average)
        {
            Average = average;
        }

        public static Result<Rating> Create(decimal? average)
        {
            if (average is < 0 or > 10)
            {
                return Result<Rating>.Invalid(new ValidationError
                {
                    Identifier = nameof(Average),
                    ErrorMessage = "Average must be between 0 and 10.",
                    ErrorCode = $"{nameof(Average)}.Invalid"
                });
            }

            return Result<Rating>.Success(new Rating(average));
        }

        public override string ToString()
        {
            return Average.HasValue ? $"Rating: {Average.Value}" : "No Rating";
        }
    }
}
