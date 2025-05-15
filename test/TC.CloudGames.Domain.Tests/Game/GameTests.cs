using Ardalis.Result;
using Bogus;
using Shouldly;
using DomainGame = TC.CloudGames.Domain.Game.Game;

namespace TC.CloudGames.Domain.Tests.Game;

public class GameTests
{
    private readonly Faker _faker;
    private readonly List<string> _genres;
    private readonly List<string> _platforms;
    private readonly List<string> _gameTags;
    private readonly List<string> _gameModes;
    private readonly List<string> _distributionFormats;
    private readonly List<string> _languages;
    private readonly List<string> _gameStatus;

    public GameTests()
    {
        _faker = new Faker();

        _genres = new List<string> { "Ação", "Aventura", "RPG", "Estratégia", "Simulação", "Corrida", "Esporte", "Puzzle",
            "Luta", "Plataforma", "FPS", "TPS", "Survival", "Horror", "Stealth", "Mundo Aberto", "MMORPG", "Roguelike",
            "Visual Novel", "Beat 'em up", "Battle Royale", "Musical", "Party Game", "Metroidvania", "Idle / Incremental",
            "Tower Defense", "MOBA", "Sandbox", "Tycoon" };

        _platforms = new List<string> { "PC", "PlayStation 4", "PlayStation 5", "Xbox One", "Xbox Series X|S", "Nintendo Switch",
            "Nintendo 3DS", "Wii U", "PlayStation Vita", "Android", "iOS", "macOS", "Linux", "Stadia", "Steam Deck", "Browser",
            "VR (Oculus Quest)", "VR (HTC Vive)", "VR (PlayStation VR)" };

        _gameTags = new List<string> { "Indie", "Multiplayer", "Singleplayer", "Co-op", "PvP", "PvE", "Online Co-op",
            "Local Multiplayer", "Story Rich", "Difficult", "Casual", "Anime", "Pixel Graphics", "Retro", "Funny", "Atmospheric",
            "Horror", "Sci-fi", "Fantasy", "Cyberpunk", "Steampunk", "Post-apocalyptic", "Choices Matter", "Narration",
            "Character Customization", "Exploration", "Loot", "Crafting", "Building", "Resource Management", "Base Building",
            "Turn-Based", "Real Time", "Fast-Paced", "Third Person", "First Person", "Top-Down", "Isometric", "Stylized",
            "Realistic", "Female Protagonist", "Controller Support", "VR Support", "Moddable", "Replay Value", "Open World",
            "Procedural Generation", "Sandbox", "Nonlinear", "Mystery", "Psychological", "Dark", "Gore", "Violent" };

        _gameModes = new List<string> { "Singleplayer", "Multiplayer", "Co-op", "PvP", "PvE", "Battle Royale", "Survival",
            "Sandbox", "Casual" };

        _distributionFormats = new List<string> { "Digital", "Physical" };

        _languages = new List<string> { "PT-BR", "EN-US", "ES-ES", "FR-FR", "ZH-CN", "JA-JP", "RU-RU", "KO-KR" };

        _gameStatus = new List<string> { "Available", "Soon", "Early Access" };
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Required_Fields_Are_Empty()
    {
        // Arrange - Act
        var result = DomainGame.Create(
            name: string.Empty,
            releaseDate: DateOnly.MinValue,
            ageRating: string.Empty,
            description: _faker.Lorem.Paragraph(),
            developerInfo: (string.Empty, _faker.Company.CompanyName()),
            diskSize: 0,
            price: 0,
            playtime: (_faker.Random.Int(1, 1000), _faker.Random.Int(1, 1000000)),
            gameDetails: (
                genre: _faker.PickRandom(_genres),
                platform: ["", string.Empty],
                tags: _faker.PickRandom(_gameTags),
                gameMode: string.Empty,
                distributionFormat: string.Empty,
                availableLanguages: _faker.PickRandom(_languages),
                supportsDlcs: _faker.Random.Bool()),
            systemRequirements: (string.Empty, _faker.Lorem.Paragraph()),
            rating: null,
            officialLink: _faker.Internet.Url(),
            gameStatus: string.Empty
        );


        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        var errors = result.ValidationErrors;

        errors.ShouldNotBeNull()
            .ShouldNotBeEmpty();
        errors.ShouldBeOfType<List<ValidationError>>();
        errors.Count().ShouldBe(13);
        
        errors.Count(x => x.Identifier == nameof(DomainGame.Name)).ShouldBe(0);
        errors.Count(x => x.Identifier == nameof(DomainGame.ReleaseDate)).ShouldBe(0);
        errors.Count(x => x.Identifier == nameof(DomainGame.AgeRating)).ShouldBe(3);
        errors.Count(x => x.Identifier == nameof(DomainGame.DeveloperInfo.Developer)).ShouldBe(1);
        errors.Count(x => x.Identifier == nameof(DomainGame.DiskSize)).ShouldBe(2);
        errors.Count(x => x.Identifier == nameof(DomainGame.Price)).ShouldBe(1);
        errors.Count(x => x.Identifier == nameof(DomainGame.GameDetails.Platform)).ShouldBe(1);
        errors.Count(x => x.Identifier == nameof(DomainGame.GameDetails.GameMode)).ShouldBe(2);
        errors.Count(x => x.Identifier == nameof(DomainGame.GameDetails.DistributionFormat)).ShouldBe(2);
        errors.Count(x => x.Identifier == nameof(DomainGame.SystemRequirements.Minimum)).ShouldBe(1);
        errors.Count(x => x.Identifier == nameof(DomainGame.Rating)).ShouldBe(0);
        errors.Count(x => x.Identifier == nameof(DomainGame.GameStatus)).ShouldBe(0);
    }
    
    [Fact]
    public void Create_Game_Should_Return_Success_When_All_Fields_Are_Valid()
    {
        // Arrange
        var name = _faker.Commerce.ProductName();
        var releaseDate = DateOnly.FromDateTime(DateTime.Now);
        var ageRating = _faker.PickRandom(Domain.Game.AgeRating.ValidRatings.ToArray());
        var description = _faker.Lorem.Paragraph();
        var developerInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
        var diskSize = _faker.Random.Decimal(1, 100);
        var price = _faker.Random.Decimal(10, 300);
        var playtime = (_faker.Random.Int(1, 10), _faker.Random.Int(10, 100));

        // Act
        var result = DomainGame.Create(
            name: name,
            releaseDate: releaseDate,
            ageRating: ageRating,
            description: description,
            developerInfo: developerInfo,
            diskSize: diskSize,
            price: price,
            playtime: playtime,
            gameDetails: (
                genre: _faker.PickRandom(_genres),
                platform: _faker.PickRandom(_platforms, 3).ToArray(),
                tags: _faker.PickRandom(_gameTags),
                gameMode: _faker.PickRandom(_gameModes),
                distributionFormat: _faker.PickRandom(_distributionFormats),
                availableLanguages: _faker.PickRandom(_languages),
                supportsDlcs: true),
            systemRequirements: (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph()),
            rating: null,
            officialLink: _faker.Internet.Url(),
            gameStatus: _faker.PickRandom(_gameStatus)
        );

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldNotBeNull();
        result.Value.Name.ShouldBe(name);
        result.Value.ReleaseDate.ShouldBe(releaseDate);
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Price_Is_Negative()
    {
        // Arrange - Act
        var result = DomainGame.Create(
            name: _faker.Commerce.ProductName(),
            releaseDate: DateOnly.FromDateTime(DateTime.Now),
            ageRating: _faker.PickRandom(Domain.Game.AgeRating.ValidRatings.ToArray()),
            description: _faker.Lorem.Paragraph(),
            developerInfo: (_faker.Company.CompanyName(), _faker.Company.CompanyName()),
            diskSize: (decimal)10.5,
            price: -50,
            playtime: (_faker.Random.Int(1, 10), _faker.Random.Int(10, 100)),
            gameDetails: (
                genre: _faker.PickRandom(_genres),
                platform: _faker.PickRandom(_platforms, 3).ToArray(),
                tags: _faker.PickRandom(_gameTags),
                gameMode: _faker.PickRandom(_gameModes),
                distributionFormat: _faker.PickRandom(_distributionFormats),
                availableLanguages: _faker.PickRandom(_languages),
                supportsDlcs: true),
            systemRequirements: (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph()),
            rating: null,
            officialLink: _faker.Internet.Url(),
            gameStatus: _faker.PickRandom(_gameStatus)
        );

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(x => x.Identifier == nameof(DomainGame.Price));
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Release_Date_Is_Default_Value()
    {
        // Arrange - Act
        var result = DomainGame.Create(
            name: _faker.Commerce.ProductName(),
            releaseDate: DateOnly.MinValue,
            ageRating: _faker.PickRandom(Domain.Game.AgeRating.ValidRatings.ToArray()),
            description: _faker.Lorem.Paragraph(),
            developerInfo: (_faker.Company.CompanyName(), _faker.Company.CompanyName()),
            diskSize: (decimal)10.5,
            price: 50,
            playtime: (_faker.Random.Int(1, 10), _faker.Random.Int(10, 100)),
            gameDetails: (
                genre: _faker.PickRandom(_genres),
                platform: _faker.PickRandom(_platforms, 3).ToArray(),
                tags: _faker.PickRandom(_gameTags),
                gameMode: _faker.PickRandom(_gameModes),
                distributionFormat: _faker.PickRandom(_distributionFormats),
                availableLanguages: _faker.PickRandom(_languages),
                supportsDlcs: true),
            systemRequirements: (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph()),
            rating: null,
            officialLink: _faker.Internet.Url(),
            gameStatus: _faker.PickRandom(_gameStatus)
        );

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(x => x.Identifier == nameof(DomainGame.ReleaseDate));
    }
}