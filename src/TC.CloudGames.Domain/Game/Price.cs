using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using TC.CloudGames.Domain.Abstractions;

namespace TC.CloudGames.Domain.Game
{
    public record Price
    {
        public decimal Amount { get; }

        private Price(decimal amount)
        {
            Amount = amount;
        }

        public static Result<Price> Create(decimal amount)
        {
            var price = new Price(amount);
            var validator = new PriceValidator().ValidationResult(price);

            if (!validator.IsValid)
            {
                return Result.Invalid(validator.AsErrors());
            }

            return price;
        }
    }

    public class PriceValidator : BaseValidator<Price>
    {
        public PriceValidator()
        {
            ValidatePrice();
        }

        protected void ValidatePrice()
        {
            RuleFor(x => x.Amount)
                .NotNull()
                    .WithMessage("Price amount is required.")
                    .WithErrorCode($"{nameof(Price)}.Required")
                    .OverridePropertyName(nameof(Price))
                .GreaterThanOrEqualTo(0)
                    .WithMessage("Price must be greater than or equal to 0.")
                    .WithErrorCode($"{nameof(Price)}.GreaterThanOrEqualToZero")
                    .OverridePropertyName(nameof(Price));
        }
    }
}
