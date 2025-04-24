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
            if (average is < 0 or > 100)
            {
                return Result<Rating>.Error("Average must be between 0 and 100.");
            }

            return Result<Rating>.Success(new Rating(average));
        }

        public override string ToString()
        {
            return Average.HasValue ? $"Rating: {Average.Value}" : "No Rating";
        }
    }
}
