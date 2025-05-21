namespace TC.CloudGames.Domain.Game
{
    public sealed record Rating
    {
        public decimal? Average { get; }

        private Rating(decimal? average)
        {
            Average = average;
        }

        public static Result<Rating> Create(Action<RatingBuilder> configure)
        {
            var builder = new RatingBuilder();
            configure(builder);
            return builder.Build();
        }

        public class RatingBuilder
        {
            public decimal? Average { get; set; }

            public Result<Rating> Build()
            {
                var rating = new Rating(Average);
                var validator = new RatingValidator().ValidationResult(rating);

                if (!validator.IsValid)
                {
                    return Result.Invalid(validator.AsErrors());
                }

                return rating;
            }
        }

        public override string ToString()
        {
            return Average.HasValue ? $"Rating: {Average.Value}" : "No Rating";
        }
    }

    public class RatingValidator : BaseValidator<Rating>
    {
        public RatingValidator()
        {
            ValidateRating();
        }

        protected void ValidateRating()
        {
            When(x => x.Average != null, () =>
            {
                RuleFor(x => x.Average)
                    .GreaterThanOrEqualTo(0)
                        .WithMessage("Rating value must be greater than or equal to 0.")
                        .WithErrorCode($"{nameof(Rating)}.GreaterThanOrEqualToZero")
                        .OverridePropertyName(nameof(Rating))
                    .LessThanOrEqualTo(10)
                        .WithMessage("Rating value must be less than or equal to 10.")
                        .WithErrorCode($"{nameof(Rating)}.LessThanOrEqualToTen")
                        .OverridePropertyName(nameof(Rating));
            });
        }
    }
}
