using Ardalis.Result;
using System.Collections.Immutable;
using TC.CloudGames.Domain.Abstractions;

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
            Playtime playtime,
            GameDetails gameDetails,
            SystemRequirements systemRequirements,
            Rating rating,
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
           AgeRating ageRating,
           string? description,
           DeveloperInfo developerInfo,
           DiskSize diskSize,
           Price price,
           Playtime playtime,
           GameDetails gameDetails,
           SystemRequirements systemRequirements,
           Rating rating,
           string? officialLink,
           string? gameStatus
        )
        {
            var errorList = new List<string>();

            if (string.IsNullOrWhiteSpace(name))
            {
                errorList.Add("Game name is required.");
            }

            if (ageRating == null)
            {
                errorList.Add("Age rating is required.");
            }
            else if (!AgeRating.ValidRatings.Contains(ageRating.Value))
            {
                errorList.Add($"Invalid age rating specified. Valid age rating are: {string.Join(", ", AgeRating.ValidRatings)}.");
            }

            if (!string.IsNullOrWhiteSpace(gameStatus) && !ValidGameStatus.Contains(gameStatus))
            {
                errorList.Add($"Invalid game status specified. Valid status are: {string.Join(", ", ValidGameStatus)}.");
            }

            if (errorList.Count != 0)
            {
                return Result<Game>.Error(new ErrorList(errorList));
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
                gameStatus
            );
        }
    }

    public sealed record DeveloperInfo(string Developer, string? Publisher);

    public sealed record DiskSize(decimal SizeInGb); //TODO: fazer objeto de valor completo com validações

    public sealed record Price(decimal Amount); //TODO: fazer objeto de valor completo com validações

    public sealed record Playtime(int? Hours, int? PlayerCount); //TODO: fazer objeto de valor completo com validações

    public sealed record SystemRequirements(string Minimum, string? Recommended);
}
