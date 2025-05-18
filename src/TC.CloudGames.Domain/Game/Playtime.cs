namespace TC.CloudGames.Domain.Game
{
    public record Playtime
    {
        public int? Hours { get; }
        public int? PlayerCount { get; }

        private Playtime(int? hours, int? playerCount)
        {
            Hours = hours;
            PlayerCount = playerCount;
        }

        public static Result<Playtime> Create(int? hours, int? playerCount)
        {
            var playtime = new Playtime(hours, playerCount);
            var validator = new PlaytimeValidator().ValidationResult(playtime);

            if (!validator.IsValid)
            {
                return Result.Invalid(validator.AsErrors());
            }

            return playtime;
        }
    }

    public class PlaytimeValidator : BaseValidator<Playtime>
    {
        public PlaytimeValidator()
        {
            ValidatePlaytime();
        }

        protected void ValidatePlaytime()
        {
            When(x => x.Hours != null, () =>
            {
                RuleFor(x => x.Hours)
                    .GreaterThanOrEqualTo(0)
                        .WithMessage("Playtime hours must be greater than or equal to 0.")
                        .WithErrorCode($"{nameof(Playtime.Hours)}.GreaterThanOrEqualToZero");
            });

            When(x => x.PlayerCount != null, () =>
            {
                RuleFor(x => x.PlayerCount)
                    .GreaterThanOrEqualTo(1)
                        .WithMessage("Player count must be greater than or equal to 1.")
                        .WithErrorCode($"{nameof(Playtime.PlayerCount)}.GreaterThanOrEqualToOne");
            });
        }
    }
}
