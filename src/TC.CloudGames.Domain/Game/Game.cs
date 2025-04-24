using TC.CloudGames.Domain.Abstractions;

namespace TC.CloudGames.Domain.Game
{
    public sealed class Game : Entity
    {
        public string Name { get; private set; } = string.Empty;
        public DateTime ReleaseDate { get; private set; }
        public AgeRating AgeRating { get; private set; }
        public string? Description { get; private set; }
        public DeveloperInfo DeveloperInfo { get; private set; }
        public DiskSize DiskSize { get; private set; }
        public Price Price { get; private set; }
        public Playtime Playtime { get; private set; }
        public GameDetails GameDetails { get; private set; }
        public SystemRequirements SystemRequirements { get; private set; }
        public Rating Rating { get; private set; }
        public string? OfficialLink { get; private set; }
        public string? GameStatus { get; private set; }

        private Game()
        {
            //EF Core
        }

        private Game(
            Guid id,
            string name,
            DateTime releaseDate,
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

        public static Game Create(
           string name,
           DateTime releaseDate,
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

    public record AgeRating(string Value);

    public record DeveloperInfo(string Developer, string Publisher);

    public record DiskSize(decimal SizeInGb);

    public record Price(decimal Amount);

    public record Playtime(int? Hours, int? PlayerCount);

    public record GameDetails(
        string? Genre,
        string Platform,
        string? Tags,
        string? GameMode,
        string? DistributionFormat,
        string? AvailableLanguages,
        bool SupportsDlcs
    );

    public record SystemRequirements(string? Minimum, string? Recommended);

    public record Rating(decimal? Average);
}
