using System.Text.Json;
using System.Text.Json.Serialization;

namespace TC.CloudGames.Application.Games.GetGame
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
    }

    public sealed class DeveloperInfo
    {
        public string Developer { get; init; }
        public string? Publisher { get; init; }
    }

    public sealed class Price
    {
        public decimal Amount { get; init; }
    }

    public sealed class Playtime
    {
        public int? Hours { get; init; }
        public int? PlayerCount { get; init; }
    }

    public sealed class SystemRequirements
    {
        public string Minimum { get; init; }
        public string? Recommended { get; init; }
    }

    public sealed class GameDetails
    {
        public string? Genre { get; init; }

        [JsonIgnore]
        public string PlatformString { get; private set; }

        public string? Tags { get; init; }
        public string GameMode { get; init; }
        public string DistributionFormat { get; init; }
        public string? AvailableLanguages { get; init; }
        public bool SupportsDlcs { get; init; }

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
