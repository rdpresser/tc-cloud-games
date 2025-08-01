﻿using System.Collections.Immutable;
using TC.CloudGames.Infra.CrossCutting.Commons.Extensions;

namespace TC.CloudGames.Domain.Aggregates.Game.ValueObjects
{
    public sealed record AgeRating
    {
        public string Value { get; }

        public static readonly IImmutableSet<string> ValidRatings =
            ImmutableHashSet.Create("E", "E10+", "T", "M", "A", "RP");

        private AgeRating(string value) => Value = value;

        /// <summary>
        /// Builder pattern for AgeRating.
        /// </summary>
        public static Result<AgeRating> Create(Action<AgeRatingBuilder> configure)
        {
            var builder = new AgeRatingBuilder();
            configure(builder);
            return builder.Build();
        }

        public class AgeRatingBuilder
        {
            public string Value { get; set; } = string.Empty;

            public Result<AgeRating> Build()
            {
                var ageRating = new AgeRating(Value);
                var validator = new AgeRatingValidator()
                    .ValidationResult(ageRating);

                if (!validator.IsValid)
                {
                    return Result.Invalid(validator.AsErrors());
                }

                return ageRating;
            }
        }

        public override string ToString() => Value;
    }

    public class AgeRatingValidator : BaseValidator<AgeRating>
    {
        public AgeRatingValidator()
        {
            ValidateAgeRating();
        }

        protected void ValidateAgeRating()
        {
            RuleFor(x => x.Value)
                .NotEmpty()
                    .WithMessage("Age rating value is required.")
                    .WithErrorCode($"{nameof(AgeRating)}.Required")
                    .OverridePropertyName(nameof(AgeRating))
                .MaximumLength(10)
                    .WithMessage("Age rating value cannot exceed 10 characters.")
                    .WithErrorCode($"{nameof(Game.AgeRating)}.MaximumLength")
                    .OverridePropertyName(nameof(AgeRating))
                .Must(rating => AgeRating.ValidRatings.Contains(rating))
                    .WithMessage($"Invalid age rating value specified. Valid age rating are: {AgeRating.ValidRatings.JoinWithQuotes()}.")
                    .WithErrorCode($"{nameof(AgeRating)}.ValidRating")
                    .OverridePropertyName(nameof(AgeRating));
        }
    }
}
