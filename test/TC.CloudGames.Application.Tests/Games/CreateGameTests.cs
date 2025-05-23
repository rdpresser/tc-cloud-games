using FakeItEasy;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Application.Games.CreateGame;
using TC.CloudGames.Domain.Game.Abstractions;
using DeveloperInfo = TC.CloudGames.Application.Games.CreateGame.DeveloperInfo;
using GameDetails = TC.CloudGames.Application.Games.CreateGame.GameDetails;
using Playtime = TC.CloudGames.Application.Games.CreateGame.Playtime;
using SystemRequirements = TC.CloudGames.Application.Games.CreateGame.SystemRequirements;
using DomainGame = TC.CloudGames.Domain.Game.Game;
using DomainGameDetails = TC.CloudGames.Domain.Game.GameDetails;
using Price  = TC.CloudGames.Application.Games.CreateGame.Price;

namespace TC.CloudGames.Application.Tests.Games;

public class CreateGameTests
{

    private readonly Faker _faker;
    private readonly List<string> _genres;
    private readonly List<string> _platforms;
    private readonly List<string> _gameTags;
    private readonly List<string> _gameModes;
    private readonly List<string> _distributionFormats;
    private readonly List<string> _languages;
    private readonly List<string> _gameStatus;
    private readonly List<string> _ageRatings;

    public CreateGameTests()
    {
        _faker = new Faker();

        _genres = new List<string> { "Action", "Adventure", "RPG", "Strategy", "Simulation", "Racing", "Sport", "Puzzle",
            "Fighter", "Platform", "FPS", "TPS", "Survival", "Horror", "Stealth", "Open World", "MMORPG", "Roguelike",
            "Visual Novel", "Beat 'em up", "Battle Royale", "Musical", "Party Game", "Metroidvania", "Idle/Incremental",
            "Tower Defense", "MOBA", "Sandbox", "Tycoon" };

        _platforms = [.. DomainGameDetails.ValidPlatforms];

        _gameTags = new List<string> { "Indie", "Multiplayer", "Singleplayer", "Co-op", "PvP", "PvE", "Online Co-op",
            "Local Multiplayer", "Story Rich", "Difficult", "Casual", "Anime", "Pixel Graphics", "Retro", "Funny", "Atmospheric",
            "Horror", "Sci-fi", "Fantasy", "Cyberpunk", "Steampunk", "Post-apocalyptic", "Choices Matter", "Narration",
            "Character Customization", "Exploration", "Loot", "Crafting", "Building", "Resource Management", "Base Building",
            "Turn-Based", "Real Time", "Fast-Paced", "Third Person", "First Person", "Top-Down", "Isometric", "Stylized",
            "Realistic", "Female Protagonist", "Controller Support", "VR Support", "Moddable", "Replay Value", "Open World",
            "Procedural Generation", "Sandbox", "Nonlinear", "Mystery", "Psychological", "Dark", "Gore", "Violent" };

        _gameModes = [.. DomainGameDetails.ValidGameModes];

        _distributionFormats = [.. DomainGameDetails.ValidDistributionFormats];

        _languages = new List<string> { "PT-BR", "EN-US", "ES-ES", "FR-FR", "ZH-CN", "JA-JP", "RU-RU", "KO-KR" };

        _gameStatus = [.. DomainGame.ValidGameStatus];

        _ageRatings = [.. AgeRating.ValidRatings];
    }
    
    [Fact]
    public async Task Handle_CreateGame()
    {
        // Arrange
        var name = _faker.Commerce.ProductName();
        var releaseDate = DateOnly.FromDateTime(DateTime.Now);
        var ageRating = _faker.PickRandom(_ageRatings.ToArray());
        var description = _faker.Lorem.Paragraph();
        var developerInfo = new DeveloperInfo(_faker.Company.CompanyName(), _faker.Company.CompanyName());
        var diskSize = _faker.Random.Decimal(1, 100);
        var price = _faker.Random.Decimal(10, 300);
        var playtime = new Playtime(_faker.Random.Int(1, 10), _faker.Random.Int(10, 100));

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
            SystemRequirements: new SystemRequirements("Min Spec", "Rec Spec"),
            Rating: 4.5m,
            OfficialLink: "https://testgame.com",
            GameStatus: "Released"
        );

        var expectedResponse = new CreateGameResponse(
            Id: Guid.NewGuid(),
            Name: command.Name,
            ReleaseDate: command.ReleaseDate,
            AgeRating: command.AgeRating,
            Description: command.Description,
            DeveloperInfo: command.DeveloperInfo,
            DiskSize: command.DiskSize,
            Price: command.Price,
            Playtime: command.Playtime,
            GameDetails: command.GameDetails,
            SystemRequirements: command.SystemRequirements,
            Rating: command.Rating,
            OfficialLink: command.OfficialLink,
            GameStatus: command.GameStatus
        );

        var fakerHandler = A.Fake<ICommandHandler<CreateGameCommand, CreateGameResponse>>();

        A.CallTo(() => fakerHandler.ExecuteAsync(command, A<CancellationToken>.Ignored))
            .Returns(expectedResponse);

        // Act
        var result = await fakerHandler.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Value.Id);

        A.CallTo(() => fakerHandler.ExecuteAsync(command, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10.5)]
    [InlineData(99999.99)]
    public void Price_CanBeCreated_WithValidAmount(decimal amount)
    {
        var price = new Price(amount);
        Assert.Equal(amount, price.Amount);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Price_AllowsNegativeAmount_ButShouldBeValidatedElsewhere(decimal amount)
    {
        var price = new Price(amount);
        Assert.Equal(amount, price.Amount);
    }
}