using TC.CloudGames.Domain.Aggregates.Game.ValueObjects;
using TC.CloudGames.Unit.Tests.Shared;
using DeveloperInfo = TC.CloudGames.Application.Games.CreateGame.DeveloperInfo;
using DomainGameDetails = TC.CloudGames.Domain.Aggregates.Game.ValueObjects.GameDetails;
using GameDetails = TC.CloudGames.Application.Games.CreateGame.GameDetails;
using Playtime = TC.CloudGames.Application.Games.CreateGame.Playtime;
using SystemRequirements = TC.CloudGames.Application.Games.CreateGame.SystemRequirements;

namespace TC.CloudGames.Unit.Tests.Application.Games
{
    public class CreateGameCommandValidatorTests : BaseTest
    {
        private readonly List<string> _genres;
        private readonly List<string> _platforms;
        private readonly List<string> _gameTags;
        private readonly List<string> _gameModes;
        private readonly List<string> _distributionFormats;
        private readonly List<string> _languages;
        private readonly List<string> _ageRatings;

        public CreateGameCommandValidatorTests()
        {
            _genres = [ "Action", "Adventure", "RPG", "Strategy", "Simulation", "Racing", "Sport", "Puzzle",
            "Fighter", "Platform", "FPS", "TPS", "Survival", "Horror", "Stealth", "Open World", "MMORPG", "Roguelike",
            "Visual Novel", "Beat 'em up", "Battle Royale", "Musical", "Party Game", "Metroidvania", "Idle/Incremental",
            "Tower Defense", "MOBA", "Sandbox", "Tycoon" ];

            _platforms = [.. DomainGameDetails.ValidPlatforms];

            _gameTags = [ "Indie", "Multiplayer", "Singleplayer", "Co-op", "PvP", "PvE", "Online Co-op",
            "Local Multiplayer", "Story Rich", "Difficult", "Casual", "Anime", "Pixel Graphics", "Retro", "Funny", "Atmospheric",
            "Horror", "Sci-fi", "Fantasy", "Cyberpunk", "Steampunk", "Post-apocalyptic", "Choices Matter", "Narration",
            "Character Customization", "Exploration", "Loot", "Crafting", "Building", "Resource Management", "Base Building",
            "Turn-Based", "Real Time", "Fast-Paced", "Third Person", "First Person", "Top-Down", "Isometric", "Stylized",
            "Realistic", "Female Protagonist", "Controller Support", "VR Support", "Moddable", "Replay Value", "Open World",
            "Procedural Generation", "Sandbox", "Nonlinear", "Mystery", "Psychological", "Dark", "Gore", "Violent" ];

            _gameModes = [.. DomainGameDetails.ValidGameModes];

            _distributionFormats = [.. DomainGameDetails.ValidDistributionFormats];

            _languages = ["PT-BR", "EN-US", "ES-ES", "FR-FR", "ZH-CN", "JA-JP", "RU-RU", "KO-KR"];

            _ageRatings = [.. AgeRating.ValidRatings];
        }

        [Fact]
        public void Should_Return_Error_When_Name_Is_Empty()
        {
            // Arrange
            var releaseDate = DateOnly.FromDateTime(DateTime.Now);
            var ageRating = Fake.PickRandom(_ageRatings.ToArray());
            var description = Fake.Lorem.Paragraph();
            var developerInfo = new DeveloperInfo(Fake.Company.CompanyName(), Fake.Company.CompanyName());
            var diskSize = Fake.Random.Decimal(1, 100);
            var price = Fake.Random.Decimal(10, 300);
            var playtime = new Playtime(Fake.Random.Int(1, 10), Fake.Random.Int(10, 100));

            var validator = new CreateGameCommandValidator();
            var command = new CreateGameCommand(
                Name: "",
                ReleaseDate: releaseDate,
                AgeRating: ageRating,
                Description: description,
                DeveloperInfo: developerInfo,
                DiskSize: diskSize,
                Price: price,
                Playtime: playtime,
                GameDetails: new GameDetails(
                    Genre: Fake.PickRandom(_genres),
                    Platform: Fake.PickRandom(_platforms, 3).ToArray(),
                    Tags: Fake.PickRandom(_gameTags),
                    GameMode: Fake.PickRandom(_gameModes),
                    DistributionFormat: Fake.PickRandom(_distributionFormats),
                    AvailableLanguages: Fake.PickRandom(_languages),
                    SupportsDlcs: true
                ),
                SystemRequirements: new SystemRequirements(Fake.Lorem.Paragraph(), Fake.Lorem.Paragraph()),
                Rating: null,
                OfficialLink: Fake.Internet.Url(),
                GameStatus: "Released"
            );

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Name");
        }

        [Fact]
        public void Should_Pass_When_Command_Is_Valid()
        {
            // Arrange
            var name = Fake.Commerce.ProductName();
            var releaseDate = DateOnly.FromDateTime(DateTime.Now);
            var ageRating = Fake.PickRandom(_ageRatings.ToArray());
            var description = Fake.Lorem.Paragraph();
            var developerInfo = new DeveloperInfo(Fake.Company.CompanyName(), Fake.Company.CompanyName());
            var diskSize = Fake.Random.Decimal(1, 100);
            var price = Fake.Random.Decimal(10, 300);
            var playtime = new Playtime(Fake.Random.Int(1, 10), Fake.Random.Int(10, 100));

            var validator = new CreateGameCommandValidator();
            var command = new CreateGameCommand(
                Name: name,
                ReleaseDate: releaseDate,
                AgeRating: ageRating,
                Description: description,
                DeveloperInfo: developerInfo,
                DiskSize: diskSize,
                Price: price,
                Playtime: playtime,
                GameDetails: new GameDetails(
                    Genre: Fake.PickRandom(_genres),
                    Platform: _platforms.ToArray(),
                    Tags: Fake.PickRandom(_gameTags),
                    GameMode: Fake.PickRandom(_gameModes),
                    DistributionFormat: Fake.PickRandom(_distributionFormats),
                    AvailableLanguages: Fake.PickRandom(_languages),
                    SupportsDlcs: true
                ),
                SystemRequirements: new SystemRequirements(Fake.Lorem.Paragraph(), Fake.Lorem.Paragraph()),
                Rating: null,
                OfficialLink: Fake.Internet.Url(),
                GameStatus: "Released"
            );

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
        }
    }
}
