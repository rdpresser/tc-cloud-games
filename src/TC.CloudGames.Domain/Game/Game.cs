using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using System.Collections.Immutable;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Domain.Game.Abstractions;

namespace TC.CloudGames.Domain.Game
{
    public sealed class Game : Entity
    {
        public string Name { get; private set; }
        public DateOnly ReleaseDate { get; private set; }
        public AgeRating AgeRating { get; private set; }
        public string? Description { get; private set; }
        public DeveloperInfo DeveloperInfo { get; private set; }
        public DiskSize DiskSize { get; private set; }
        public Price Price { get; private set; }
        public Playtime? Playtime { get; private set; }
        public GameDetails GameDetails { get; private set; }
        public SystemRequirements SystemRequirements { get; private set; }
        public Rating? Rating { get; private set; }
        public string? OfficialLink { get; private set; }
        public string? GameStatus { get; private set; }

        public static readonly IImmutableSet<string> ValidGameStatus = ImmutableHashSet.Create(
            "In Development", "Released", "Discontinued", "Available", "Soon", "Early Access"
        );

        private Game()
        {
            //EF Core
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

        public static Result<Game> Create(
           string name,
           DateOnly releaseDate,
           string ageRating,
           string? description,
           (string developer, string? publisher) developerInfo,
           decimal diskSize,
           decimal price,
           (int? hours, int? playerCount)? playtime,
           (string? genre, string[] platform, string? tags, string gameMode, string distributionFormat, string? availableLanguages, bool supportsDlcs) gameDetails,
           (string minimum, string? recommended) systemRequirements,
           decimal? rating,
           string? officialLink,
           string? gameStatus
        )
        {
            var ageRatingResult = AgeRating.Create(ageRating);
            var developerInfoResult = DeveloperInfo.Create(developerInfo.developer, developerInfo.publisher);
            var diskSizeResult = DiskSize.Create(diskSize);
            var priceResult = Price.Create(price);
            var playtimeResult = Playtime.Create(!playtime.HasValue || !playtime.Value.hours.HasValue ? default : playtime.Value.hours.Value, !playtime.HasValue || !playtime.Value.playerCount.HasValue ? default : playtime.Value.playerCount.Value);
            var gameDetailsResult = GameDetails.Create(gameDetails.genre, gameDetails.platform, gameDetails.tags, gameDetails.gameMode, gameDetails.distributionFormat, gameDetails.availableLanguages, gameDetails.supportsDlcs);
            var systemRequirementsResult = SystemRequirements.Create(systemRequirements.minimum, systemRequirements.recommended);
            var ratingResult = Rating.Create(rating);

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
                    name,
                    releaseDate,
                    ageRatingResult,
                    description,
                    developerInfoResult,
                    diskSizeResult,
                    priceResult,
                    playtimeResult,
                    gameDetailsResult,
                    systemRequirementsResult,
                    ratingResult,
                    officialLink,
                    gameStatus
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
}