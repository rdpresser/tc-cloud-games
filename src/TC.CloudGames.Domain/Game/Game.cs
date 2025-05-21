using System.Collections.Immutable;
using TC.CloudGames.Domain.Game.Abstractions;

namespace TC.CloudGames.Domain.Game
{
    public sealed class Game : Entity
    {
        public string Name { get; }
        public DateOnly ReleaseDate { get; }
        public AgeRating AgeRating { get; }
        public string? Description { get; }
        public DeveloperInfo DeveloperInfo { get; }
        public DiskSize DiskSize { get; }
        public Price Price { get; }
        public Playtime? Playtime { get; }
        public GameDetails GameDetails { get; }
        public SystemRequirements SystemRequirements { get; }
        public Rating? Rating { get; }
        public string? OfficialLink { get; }
        public string? GameStatus { get; }

        public static readonly IImmutableSet<string> ValidGameStatus = ImmutableHashSet.Create(
            "In Development", "Released", "Discontinued", "Available", "Soon", "Early Access"
        );

        private Game()
        {
            // EF Core
        }

        private Game(
            Guid id,
            string name,
            DateOnly releaseDate,
            AgeRating ageRating,
            string? description,
            DeveloperInfo developerInfo,
            DiskSize diskSize,
            Price price,
            Playtime? playtime,
            GameDetails gameDetails,
            SystemRequirements systemRequirements,
            Rating? rating,
            string? officialLink,
            string? gameStatus
        ) : base(id)
        {
            Id = id;
            Name = name;
            ReleaseDate = releaseDate;
            AgeRating = ageRating;
            Description = description;
            DeveloperInfo = developerInfo;
            DiskSize = diskSize;
            Price = price;
            Playtime = playtime;
            GameDetails = gameDetails;
            SystemRequirements = systemRequirements;
            Rating = rating;
            OfficialLink = officialLink;
            GameStatus = gameStatus;
        }

        // Builder pattern
        public static Result<Game> Create(Action<GameBuilder> configure)
        {
            var builder = new GameBuilder();
            configure(builder);
            return builder.Build();
        }

        public static Result<Game> CreateFromValueObjects(Action<GameBuilderFromValueObjects> configure)
        {
            var builder = new GameBuilderFromValueObjects();
            configure(builder);
            return builder.Build();
        }

        public class GameBuilderFromValueObjects
        {
            public string Name { get; set; } = string.Empty;
            public DateOnly ReleaseDate { get; set; }
            public AgeRating AgeRating { get; set; }
            public string? Description { get; set; }
            public DeveloperInfo DeveloperInfo { get; set; }
            public DiskSize DiskSize { get; set; }
            public Price Price { get; set; }
            public Playtime? Playtime { get; set; }
            public GameDetails GameDetails { get; set; }
            public SystemRequirements SystemRequirements { get; set; }
            public Rating? Rating { get; set; }
            public string? OfficialLink { get; set; }
            public string? GameStatus { get; set; }

            public Result<Game> Build()
            {
                var game = new Game(
                    Guid.NewGuid(),
                    Name,
                    ReleaseDate,
                    AgeRating,
                    Description,
                    DeveloperInfo,
                    DiskSize,
                    Price,
                    Playtime,
                    GameDetails,
                    SystemRequirements,
                    Rating,
                    OfficialLink,
                    GameStatus
                );

                var validator = new CreateGameValidator().ValidationResult(game);
                if (!validator.IsValid)
                    return Result.Invalid(validator.AsErrors());

                /*
                 * RaiseDomainEvent - Send onboarding email to the new user
                 */
                return game;
            }
        }

        public class GameBuilder
        {
            public string Name { get; set; } = string.Empty;
            public DateOnly ReleaseDate { get; set; }
            public string AgeRating { get; set; } = string.Empty;
            public string? Description { get; set; }
            public (string Developer, string? Publisher) DeveloperInfo { get; set; } = (string.Empty, null);
            public decimal DiskSize { get; set; }
            public decimal Price { get; set; }
            public (int? Hours, int? PlayerCount)? Playtime { get; set; }
            public (string? Genre, string[] Platform, string? Tags, string GameMode, string DistributionFormat, string? AvailableLanguages, bool SupportsDlcs) GameDetails { get; set; }
                = (null, [], null, string.Empty, string.Empty, null, false);
            public (string Minimum, string? Recommended) SystemRequirements { get; set; } = (string.Empty, null);
            public decimal? Rating { get; set; }
            public string? OfficialLink { get; set; }
            public string? GameStatus { get; set; }

            public Result<Game> Build()
            {
                // Value object creation
                var ageRatingResult = Domain.Game.AgeRating.Create(builder => builder.Value = AgeRating);
                var developerInfoResult = Domain.Game.DeveloperInfo.Create(builder =>
                {
                    builder.Developer = DeveloperInfo.Developer;
                    builder.Publisher = DeveloperInfo.Publisher;
                });
                var diskSizeResult = Domain.Game.DiskSize.Create(builder => builder.SizeInGb = DiskSize);
                var priceResult = Domain.Game.Price.Create(builder => builder.Amount = Price);
                var playtimeResult = Domain.Game.Playtime.Create(builder =>
                {
                    builder.Hours = !Playtime.HasValue || !Playtime.Value.Hours.HasValue ? default : Playtime.Value.Hours.Value;
                    builder.PlayerCount = !Playtime.HasValue || !Playtime.Value.PlayerCount.HasValue ? default : Playtime.Value.PlayerCount.Value;
                });

                var gameDetailsResult = Domain.Game.GameDetails.Create(builder =>
                {
                    builder.Genre = GameDetails.Genre;
                    builder.Platform = GameDetails.Platform;
                    builder.Tags = GameDetails.Tags;
                    builder.GameMode = GameDetails.GameMode;
                    builder.DistributionFormat = GameDetails.DistributionFormat;
                    builder.AvailableLanguages = GameDetails.AvailableLanguages;
                    builder.SupportsDlcs = GameDetails.SupportsDlcs;
                });

                var systemRequirementsResult = Domain.Game.SystemRequirements.Create(builder =>
                {
                    builder.Minimum = SystemRequirements.Minimum;
                    builder.Recommended = SystemRequirements.Recommended;
                });
                var ratingResult = Domain.Game.Rating.Create(builder => builder.Average = Rating);

                var valueObjectResults = new IResult[]
                {
                    ageRatingResult,
                    developerInfoResult,
                    diskSizeResult,
                    priceResult,
                    playtimeResult,
                    gameDetailsResult,
                    systemRequirementsResult,
                    ratingResult
                };

                var errors = CollectValidationErrors(valueObjectResults);
                if (errors.Count != 0)
                {
                    return Result.Invalid(errors);
                }

                var game = new Game(
                    Guid.NewGuid(),
                    Name,
                    ReleaseDate,
                    ageRatingResult,
                    Description,
                    developerInfoResult,
                    diskSizeResult,
                    priceResult,
                    playtimeResult,
                    gameDetailsResult,
                    systemRequirementsResult,
                    ratingResult,
                    OfficialLink,
                    GameStatus
                );

                var validator = new CreateGameValidator().ValidationResult(game);
                if (!validator.IsValid)
                {
                    return Result.Invalid(validator.AsErrors());
                }

                /*
                 * RaiseDomainEvent - Send onboarding email to the new user
                 */
                return game;
            }
        }
    }
}