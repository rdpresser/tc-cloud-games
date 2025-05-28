namespace TC.CloudGames.Domain.GameAggregate.ValueObjects
{
    public sealed record Price
    {
        public decimal Amount { get; }

        private Price(decimal amount)
        {
            Amount = amount;
        }

        public static Result<Price> Create(Action<PriceBuilder> configure)
        {
            var builder = new PriceBuilder();
            configure(builder);
            return builder.Build();
        }

        public class PriceBuilder
        {
            public decimal Amount { get; set; }

            public Result<Price> Build()
            {
                var price = new Price(Amount);
                var validator = new PriceValidator().ValidationResult(price);

                if (!validator.IsValid)
                {
                    return Result.Invalid(validator.AsErrors());
                }

                return price;
            }
        }

        public override string ToString()
        {
            return $"{Amount:C}";
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
                    .WithMessage("Price amount must be greater than or equal to 0.")
                    .WithErrorCode($"{nameof(Price)}.GreaterThanOrEqualToZero")
                    .OverridePropertyName(nameof(Price));
        }
    }
}
