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

        // Shared construction and validation logic
        private static Result<Game> BuildGame(
            string name,
            DateOnly releaseDate,
            Result<AgeRating> ageRating,
            string? description,
            Result<DeveloperInfo> developerInfo,
            Result<DiskSize> diskSize,
            Result<Price> price,
            Result<Playtime>? playtime,
            Result<GameDetails> gameDetails,
            Result<SystemRequirements> systemRequirements,
            Result<Rating>? rating,
            string? officialLink,
            string? gameStatus)
        {
            var valueObjectResults = new IResult[]
            {
                EnsureResult(ageRating, nameof(AgeRating)),
                EnsureResult(developerInfo, nameof(developerInfo)),
                EnsureResult(diskSize, nameof(DiskSize)),
                EnsureResult(price, nameof(Price)),
                EnsureResult(playtime, nameof(Playtime)),
                EnsureResult(gameDetails, nameof(GameDetails)),
                EnsureResult(systemRequirements, nameof(SystemRequirements)),
                EnsureResult(rating, nameof(Rating))
            };

            var errors = CollectValidationErrors(valueObjectResults);

            var game = new Game(
                Guid.NewGuid(),
                name,
                releaseDate,
                ageRating.Value,
                description,
                developerInfo.Value,
                diskSize.Value,
                price.Value,
                playtime?.Value,
                gameDetails.Value,
                systemRequirements.Value,
                rating?.Value,
                officialLink,
                gameStatus
            );

            var validator = new CreateGameValidator().ValidationResult(game);
            if (!validator.IsValid)
            {
                errors.AddRange(validator.AsErrors());
            }

            if (errors.Count != 0)
            {
                return Result.Invalid(errors);
            }

            /*
             * RaiseDomainEvent - Send onboarding email to the new user
            */
            return game;
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

        public static Result<Game> CreateFromResult(Action<GameBuilderFromResultValueObjects> configure)
        {
            var builder = new GameBuilderFromResultValueObjects();
            configure(builder);
            return builder.Build();
        }

        public class GameBuilderFromResultValueObjects
        {
            public string Name { get; set; } = string.Empty;
            public DateOnly ReleaseDate { get; set; }
            public Result<AgeRating> AgeRating { get; set; }
            public string? Description { get; set; }
            public Result<DeveloperInfo> DeveloperInfo { get; set; }
            public Result<DiskSize> DiskSize { get; set; }
            public Result<Price> Price { get; set; }
            public Result<Playtime>? Playtime { get; set; }
            public Result<GameDetails> GameDetails { get; set; }
            public Result<SystemRequirements> SystemRequirements { get; set; }
            public Result<Rating>? Rating { get; set; }
            public string? OfficialLink { get; set; }
            public string? GameStatus { get; set; }

            public Result<Game> Build()
            {
                return BuildGame(
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
            }
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
                // Wrap value objects in Result<T> for shared logic
                return BuildGame(
                    Name,
                    ReleaseDate,
                    EnsureResult(AgeRating, nameof(AgeRating)),
                    Description,
                    EnsureResult(DeveloperInfo, nameof(DeveloperInfo)),
                    EnsureResult(DiskSize, nameof(DiskSize)),
                    EnsureResult(Price, nameof(Price)),
                    EnsureResult(Playtime, nameof(Playtime)),
                    EnsureResult(GameDetails, nameof(GameDetails)),
                    EnsureResult(SystemRequirements, nameof(SystemRequirements)),
                    EnsureResult(Rating, nameof(Rating)),
                    OfficialLink,
                    GameStatus
                );
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
                // Create value objects from raw values
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

                return BuildGame(
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
            }
        }
    }
}