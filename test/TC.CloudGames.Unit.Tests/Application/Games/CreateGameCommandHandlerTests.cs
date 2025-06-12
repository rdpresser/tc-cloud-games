using TC.CloudGames.Domain.Aggregates.Game;
using TC.CloudGames.Domain.Aggregates.Game.Abstractions;
using TC.CloudGames.Domain.Aggregates.Game.ValueObjects;
using DeveloperInfo = TC.CloudGames.Application.Games.CreateGame.DeveloperInfo;
using DomainGameDetails = TC.CloudGames.Domain.Aggregates.Game.ValueObjects.GameDetails;
using GameDetails = TC.CloudGames.Application.Games.CreateGame.GameDetails;
using Playtime = TC.CloudGames.Application.Games.CreateGame.Playtime;
using SystemRequirements = TC.CloudGames.Application.Games.CreateGame.SystemRequirements;

namespace TC.CloudGames.Unit.Tests.Application.Games
{
    public class CreateGameCommandHandlerTests
    {
        private readonly Faker _faker;
        private readonly List<string> _genres;
        private readonly List<string> _platforms;
        private readonly List<string> _gameTags;
        private readonly List<string> _gameModes;
        private readonly List<string> _distributionFormats;
        private readonly List<string> _languages;
        private readonly List<string> _ageRatings;

        public CreateGameCommandHandlerTests()
        {
            _faker = new Faker();

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
        public async Task ExecuteAsync_ValidCommand_AddsGameAndSavesChanges()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            var name = _faker.Commerce.ProductName();
            var releaseDate = DateOnly.FromDateTime(DateTime.Now);
            var ageRating = _faker.PickRandom(_ageRatings.ToArray());
            var description = _faker.Lorem.Paragraph();
            var developerInfo = new DeveloperInfo(_faker.Company.CompanyName(), _faker.Company.CompanyName());
            var diskSize = _faker.Random.Decimal(1, 100);
            var price = _faker.Random.Decimal(10, 300);
            var playtime = new Playtime(_faker.Random.Int(1, 10), _faker.Random.Int(10, 100));

            var unitOfWorkFake = A.Fake<IUnitOfWork>();
            var repoFake = A.Fake<IGameEfRepository>();

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
                    Genre: _faker.PickRandom(_genres),
                    Platform: _faker.PickRandom(_platforms, 3).ToArray(),
                    Tags: _faker.PickRandom(_gameTags),
                    GameMode: _faker.PickRandom(_gameModes),
                    DistributionFormat: _faker.PickRandom(_distributionFormats),
                    AvailableLanguages: _faker.PickRandom(_languages),
                    SupportsDlcs: true
                ),
                SystemRequirements: new SystemRequirements(_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph()),
                Rating: null,
                OfficialLink: _faker.Internet.Url(),
                GameStatus: "Released"
            );

            var handler = new CreateGameCommandHandler(unitOfWorkFake, repoFake);

            // Act
            var result = await handler.ExecuteAsync(command, TestContext.Current.CancellationToken);

            // Assert
            A.CallTo(() => repoFake.Add(A<Game>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => unitOfWorkFake.SaveChangesAsync(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.True(result.IsSuccess);
        }
    }
}
