using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TC.CloudGames.Application.Games.GetGameById
{
    public class GameByIdResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public DateOnly ReleaseDate { get; init; }
        public string AgeRating { get; init; }
        public string? Description { get; init; }
        public DeveloperInfo DeveloperInfo { get; set; }
        public decimal DiskSize { get; init; }
        public decimal Price { get; init; }
        public Playtime? Playtime { get; set; }
        public GameDetails GameDetails { get; set; }
        public SystemRequirements SystemRequirements { get; set; }
        public decimal? Rating { get; init; }
        public string? OfficialLink { get; init; }
        public string? GameStatus { get; init; }

        public static GameByIdResponse Create(Action<GameByIdBuilder> configure)
        {
            var builder = new GameByIdBuilder();
            configure(builder);
            return builder.Build();
        }

        public class GameByIdBuilder
        {
            public Guid Id { get; set; } = Guid.Empty;
            public string Name { get; set; } = string.Empty;
            public DateOnly ReleaseDate { get; set; }
            public string AgeRating { get; set; } = string.Empty;
            public string? Description { get; set; }
            public DeveloperInfo DeveloperInfo { get; set; } = new("", null);
            public decimal DiskSize { get; set; }
            public decimal Price { get; set; }
            public Playtime? Playtime { get; set; } = new(null, null);
            public GameDetails GameDetails { get; set; } = new(null, [], null, "", "", null, false);
            public SystemRequirements SystemRequirements { get; set; } = new("", null);
            public decimal? Rating { get; set; }
            public string? OfficialLink { get; set; }
            public string? GameStatus { get; set; }
            public GameByIdResponse Build() =>
                new()
                {
                    Id = Id,
                    Name = Name,
                    ReleaseDate = ReleaseDate,
                    AgeRating = AgeRating,
                    Description = Description,
                    DeveloperInfo = DeveloperInfo,
                    DiskSize = DiskSize,
                    Price = Price,
                    Playtime = Playtime,
                    GameDetails = GameDetails,
                    SystemRequirements = SystemRequirements,
                    Rating = Rating,
                    OfficialLink = OfficialLink,
                    GameStatus = GameStatus
                };
        }
    }

    public sealed class DeveloperInfo(string developer, string? publisher)
    {
        public string Developer { get; init; } = developer;
        public string? Publisher { get; init; } = publisher;
    }

    public sealed class Price(decimal amount)
    {
        public decimal Amount { get; init; } = amount;
    }

    public sealed class Playtime(int? hours, int? playerCount)
    {
        public int? Hours { get; init; } = hours;
        public int? PlayerCount { get; init; } = playerCount;
    }

    public sealed class SystemRequirements(string minimum, string? recommended)
    {
        public string Minimum { get; init; } = minimum;
        public string? Recommended { get; init; } = recommended;
    }

    public sealed class GameDetails
    {
        public GameDetails()
        {
            //Dapper
        }

        public GameDetails(string? genre, string[] platform, string? tags, string gameMode, string distributionFormat, string? availableLanguages, bool supportsDlcs)
        {
            Genre = genre;
            PlatformString = JsonSerializer.Serialize(platform);
            Tags = tags;
            GameMode = gameMode;
            DistributionFormat = distributionFormat;
            AvailableLanguages = availableLanguages;
            SupportsDlcs = supportsDlcs;
        }

        public string? Genre { get; init; }

        [JsonIgnore]
        public string PlatformString { get; private set; }

        public string? Tags { get; init; }
        public string GameMode { get; init; }
        public string DistributionFormat { get; init; }
        public string? AvailableLanguages { get; init; }
        public bool SupportsDlcs { get; init; }

        [ExcludeFromCodeCoverage]
        public string[] Platform
        {
            get
            {
                if (PlatformString == null)
                {
                    return [];
                }
                return JsonSerializer.Deserialize<string[]>(PlatformString) ?? [];
            }
            set
            {
                if (value == null)
                {
                    PlatformString = string.Empty;
                }
                else
                {
                    PlatformString = JsonSerializer.Serialize(value);
                }
            }
        }
    }
}
