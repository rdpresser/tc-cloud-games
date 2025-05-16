using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using System.Collections.Immutable;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Infra.CrossCutting.Commons.Extensions;

namespace TC.CloudGames.Domain.Game
{
    public sealed record AgeRating
    {
        public string Value { get; }

        public static readonly IImmutableSet<string> ValidRatings =
            ImmutableHashSet.Create("E", "E10+", "T", "M", "A", "RP");

        private AgeRating(string value) => Value = value;

        public static Result<AgeRating> Create(string value)
        {
            var ageRating = new AgeRating(value);
            var validator = new AgeRatingValidator()
                .ValidationResult(ageRating);

            if (!validator.IsValid)
            {
                return Result.Invalid(validator.AsErrors());
            }

            return ageRating;
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
                    .WithMessage("Age rating cannot exceed 10 characters.")
                    .WithErrorCode($"{nameof(Game.AgeRating)}.MaximumLength")
                    .OverridePropertyName(nameof(AgeRating))
                .Must(rating => AgeRating.ValidRatings.Contains(rating))
                    .WithMessage($"Invalid age rating specified. Valid age rating are: {AgeRating.ValidRatings.JoinWithQuotes()}.")
                    .WithErrorCode($"{nameof(AgeRating)}.ValidRating")
                    .OverridePropertyName(nameof(AgeRating));
        }
    }
}
