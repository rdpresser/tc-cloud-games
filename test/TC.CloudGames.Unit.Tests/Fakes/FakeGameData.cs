using TC.CloudGames.Unit.Tests.FakeModels;

namespace TC.CloudGames.Unit.Tests.Fakes;

public static class FakeGameData
{
    public static Game GameValid()
    {
        return new Game
        {
            Name = "The Witcher 3: Wild Hunt",
            Description = "An open-world RPG set in a fantasy universe.",
            ReleaseDate = new DateTime(2015, 5, 19),
            AgeRating = "M",
            DeveloperName = "CD Projekt Red",
            PublisherName = "CD Projekt",
            DiskSize = 50.0m,
            Price = 60.00m,
            PlayersTime = 50,
            PlayersCount = 1,
            Genre = "RPG",
            Platforms = new List<string> { "PC", "PlayStation 4", "Xbox One" },
            Tags = "Open World",
            GameMode = "Singleplayer",
            DistributionFormat = "Digital",
            Language = "EN-US",
            AgeRatings = new List<string> { "M" },
            SystemMinimalRequirements = " Intel CPU Core i5-2500K 3.3GHz / AMD A10-5800K APU (3.8GHz), 6GB RAM, NVIDIA GeForce GTX 660",
            SystemRecommendRequirements = "Intel Core i5-7400 / Ryzen 5 1600, 8GB RAM, NVIDIA GeForce GTX 1070",
            URL = "https://www.thewitcher.com/br/en/witcher3",
            GameStatus = "Released",
            Metacritic = 92.0m
        };
    }
}