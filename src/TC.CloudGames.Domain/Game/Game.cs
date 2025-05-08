using Ardalis.Result;
using System.Collections.Immutable;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Infra.CrossCutting.Commons.Extensions;

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
            "In Development", "Released", "Discontinued", "Available"
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
            string? gameStatus,
            DateTime createdOnUtc
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
            CreatedOnUtc = createdOnUtc;
        }

        public static Result<Game> Create(
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
           string? gameStatus,
           DateTime createdOnUtc
        )
        {
            List<ValidationError> validation = [];

            if (string.IsNullOrWhiteSpace(name))
            {
                validation.Add(new()
                {
                    Identifier = nameof(Name),
                    ErrorMessage = "Game name is required.",
                    ErrorCode = $"{nameof(Name)}.Required"
                });
            }

            if (ageRating == null)
            {
                validation.Add(new()
                {
                    Identifier = nameof(AgeRating),
                    ErrorMessage = "Age rating is required.",
                    ErrorCode = $"{nameof(AgeRating)}.Required"
                });
            }
            else if (!AgeRating.ValidRatings.Contains(ageRating.Value))
            {
                validation.Add(new()
                {
                    Identifier = nameof(AgeRating),
                    ErrorMessage = $"Invalid age rating specified. Valid age ratings are: {AgeRating.ValidRatings.JoinWithQuotes()}.",
                    ErrorCode = $"{nameof(AgeRating)}.Invalid"
                });
            }

            if (!string.IsNullOrWhiteSpace(gameStatus) && !ValidGameStatus.Contains(gameStatus))
            {
                validation.Add(new()
                {
                    Identifier = nameof(GameStatus),
                    ErrorMessage = $"Invalid game status specified. Valid status are: {ValidGameStatus.JoinWithQuotes()}.",
                    ErrorCode = $"{nameof(GameStatus)}.Invalid"
                });
            }

            if (validation.Count != 0)
            {
                return Result<Game>.Invalid(validation);
            }

            return new Game(
                Guid.NewGuid(),
                name,
                releaseDate,
                ageRating,
                description,
                developerInfo,
                diskSize,
                price,
                playtime,
                gameDetails,
                systemRequirements,
                rating,
                officialLink,
                gameStatus,
                createdOnUtc
            );
        }
    }

    public sealed record DeveloperInfo(string Developer, string? Publisher);

    public sealed record DiskSize(decimal SizeInGb); //TODO: fazer objeto de valor completo com validações

    public sealed record Price(decimal Amount); //TODO: fazer objeto de valor completo com validações

    public sealed record Playtime(int? Hours, int? PlayerCount); //TODO: fazer objeto de valor completo com validações

    public sealed record SystemRequirements(string Minimum, string? Recommended);
}