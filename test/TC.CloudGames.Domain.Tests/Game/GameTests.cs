using Ardalis.Result;
using Bogus;
using Shouldly;
using TC.CloudGames.Domain.Game;
using DomainGame = TC.CloudGames.Domain.Game.Game;
using DomainGameDetails = TC.CloudGames.Domain.Game.GameDetails;

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
    private readonly List<string> _ageRatings;

    public GameTests()
    {
        _faker = new Faker();

        _genres = new List<string> { "Ação", "Aventura", "RPG", "Estratégia", "Simulação", "Corrida", "Esporte", "Puzzle",
            "Luta", "Plataforma", "FPS", "TPS", "Survival", "Horror", "Stealth", "Mundo Aberto", "MMORPG", "Roguelike",
            "Visual Novel", "Beat 'em up", "Battle Royale", "Musical", "Party Game", "Metroidvania", "Idle / Incremental",
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
    public void Create_Game_Should_Return_Invalid_When_Fields_Have_Invalid_Values()
    {
        // Arrange - Act
        var result = DomainGame.Create(
            name: $"{_faker.Commerce.ProductAdjective()} {_faker.Commerce.ProductMaterial()} {_faker.Commerce.Product()}",
            releaseDate: DateOnly.FromDateTime(_faker.Date.Past()),
            ageRating: "Invalid",
            description: _faker.Lorem.Paragraph(),
            developerInfo: (_faker.Company.CompanyName(), _faker.Company.CompanyName()),
            diskSize: _faker.Random.Int(1, 150),
            price: decimal.Parse(_faker.Commerce.Price(1.0m, 500.0m)),
            playtime: (_faker.Random.Int(1, 200), _faker.Random.Int(1, 2000)),
            gameDetails: (
                genre: _faker.Lorem.Word(),
                platform: new[] { "Invalid" },
                tags: _faker.Lorem.Word(),
                gameMode: "Invalid",
                distributionFormat: "Invalid",
                availableLanguages: "EN-US",
                supportsDlcs: _faker.Random.Bool()),
            systemRequirements: (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph()),
            rating: -1,
            officialLink: "Invalid_URL",
            gameStatus: "Invalid"
        );

        var errors = result.ValidationErrors;
        int errorCount = 5;

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldNotBeNull().ShouldNotBeEmpty();
        errors.ShouldBeOfType<List<ValidationError>>();
        errors.Count().ShouldBe(errorCount);

        // AgeRating validation errors
        errors.Count(x => x.Identifier == nameof(DomainGame.AgeRating)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.AgeRating) && x.ErrorCode == $"{nameof(AgeRating)}.ValidRating");

        // Platform validation errors
        errors.Count(x => x.Identifier == nameof(DomainGameDetails.Platform)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGameDetails.Platform) && x.ErrorCode == $"{nameof(DomainGameDetails.Platform)}.ValidPlatform");

        // GameMode validation errors
        errors.Count(x => x.Identifier == nameof(DomainGameDetails.GameMode)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGameDetails.GameMode) && x.ErrorCode == $"{nameof(DomainGameDetails.GameMode)}.ValidGameMode");

        // DistributionFormat validation errors
        errors.Count(x => x.Identifier == nameof(DomainGameDetails.DistributionFormat)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGameDetails.DistributionFormat) && x.ErrorCode == $"{nameof(DomainGameDetails.DistributionFormat)}.ValidDistributionFormat");

        // Rating validation errors
        errors.Count(x => x.Identifier == nameof(DomainGame.Rating)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.Rating) && x.ErrorCode == $"{nameof(DomainGame.Rating)}.GreaterThanOrEqualToZero");

        // If counter is 0, it means that all errors were found
        errorCount.ShouldBe(0);
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Root_Class_Fields_Have_Invalid_Values()
    {
        // Arrange
        var name = _faker.Commerce.ProductName();
        var releaseDate = DateOnly.FromDateTime(DateTime.Now);
        var ageRating = _faker.PickRandom(_ageRatings.ToArray());
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
            officialLink: "Invalid_URL",
            gameStatus: "Invalid"
        );

        var errors = result.ValidationErrors;
        int errorCount = 2;

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldNotBeNull().ShouldNotBeEmpty();
        errors.ShouldBeOfType<List<ValidationError>>();
        errors.Count().ShouldBe(errorCount);

        // OfficialLink validation errors
        errors.Count(x => x.Identifier == nameof(DomainGame.OfficialLink)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.OfficialLink) && x.ErrorCode == $"{nameof(DomainGame.OfficialLink)}.ValidUrl");

        // GameStatus validation errors
        errors.Count(x => x.Identifier == nameof(DomainGame.GameStatus)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.GameStatus) && x.ErrorCode == $"{nameof(DomainGame.GameStatus)}.ValidGameStatus");

        // If counter is 0, it means that all errors were found
        errorCount.ShouldBe(0);
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Required_Fields_Are_Empty_Or_Default_Values()
    {
        // Arrange - Act
        var result = DomainGame.Create(
            name: string.Empty, //Test with empty name
            releaseDate: DateOnly.MinValue, //Test with default date
            ageRating: string.Empty, //Test with empty age rating
            description: _faker.Lorem.Paragraph(),
            developerInfo: (string.Empty, _faker.Company.CompanyName()), //Test with empty developer name
            diskSize: 0, //Test with zero disk size
            price: -5, //Test with negative price
            playtime: (-1, 0), //Test with invalid playtime
            gameDetails: (
                genre: _faker.PickRandom(_genres),
                platform: [], //Test with empty array platform
                tags: _faker.PickRandom(_gameTags),
                gameMode: string.Empty, //Test with empty game mode
                distributionFormat: string.Empty, //Test with empty distribution format
                availableLanguages: _faker.PickRandom(_languages),
                supportsDlcs: _faker.Random.Bool()),
            systemRequirements: (string.Empty, _faker.Lorem.Paragraph()), //Test with empty system requirements
            rating: 11, //Test with zero rating
            officialLink: _faker.Internet.Url(),
            gameStatus: string.Empty //Test with empty game status
        );

        var errors = result.ValidationErrors;
        int errorCount = 16;

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldNotBeNull().ShouldNotBeEmpty();
        errors.ShouldBeOfType<List<ValidationError>>();
        errors.Count().ShouldBe(errorCount);

        errors.Count(x => x.Identifier == nameof(DomainGame.Name)).ShouldBe(0);
        errors.Count(x => x.Identifier == nameof(DomainGame.ReleaseDate)).ShouldBe(0);

        //AgeRating validation errors
        errors.Count(x => x.Identifier == nameof(DomainGame.AgeRating)).ShouldBe(3);
        errorCount -= 3;
        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x => x.Identifier == nameof(DomainGame.AgeRating) && x.ErrorCode == $"{nameof(AgeRating)}.Required").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(DomainGame.AgeRating) && x.ErrorCode == $"{nameof(AgeRating)}.MaximumLength").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(DomainGame.AgeRating) && x.ErrorCode == $"{nameof(AgeRating)}.ValidRating").ShouldBeTrue();
        });

        //DeveloperInfo validation errors
        errors.Count(x => x.Identifier == nameof(DeveloperInfo.Developer)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DeveloperInfo.Developer) && x.ErrorCode == $"{nameof(DeveloperInfo.Developer)}.Required");

        //DiskSize validation errors
        errors.Count(x => x.Identifier == nameof(DomainGame.DiskSize)).ShouldBe(2);
        errorCount -= 2;
        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x => x.Identifier == nameof(DomainGame.DiskSize) && x.ErrorCode == $"{nameof(DomainGame.DiskSize)}.Required").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(DomainGame.DiskSize) && x.ErrorCode == $"{nameof(DomainGame.DiskSize)}.GreaterThanZero").ShouldBeTrue();
        });

        //Price validation errors
        errors.Count(x => x.Identifier == nameof(DomainGame.Price)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.Price) && x.ErrorCode == $"{nameof(DomainGame.Price)}.GreaterThanOrEqualToZero");

        //Platform validation errors
        errors.Count(x => x.Identifier == nameof(DomainGameDetails.Platform)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(errors => errors.Identifier == nameof(DomainGameDetails.Platform) && errors.ErrorCode == $"{nameof(DomainGameDetails.Platform)}.Required");

        //GameDetails validation errors
        errors.Count(x => x.Identifier == nameof(DomainGameDetails.GameMode)).ShouldBe(2);
        errorCount -= 2;
        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x => x.Identifier == nameof(DomainGameDetails.GameMode) && x.ErrorCode == $"{nameof(DomainGameDetails.GameMode)}.Required").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(DomainGameDetails.GameMode) && x.ErrorCode == $"{nameof(DomainGameDetails.GameMode)}.ValidGameMode").ShouldBeTrue();
        });

        //DistributionFormat validation errors
        errors.Count(x => x.Identifier == nameof(DomainGameDetails.DistributionFormat)).ShouldBe(2);
        errorCount -= 2;
        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x => x.Identifier == nameof(DomainGameDetails.DistributionFormat) && x.ErrorCode == $"{nameof(DomainGameDetails.DistributionFormat)}.Required").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(DomainGameDetails.DistributionFormat) && x.ErrorCode == $"{nameof(DomainGameDetails.DistributionFormat)}.ValidDistributionFormat").ShouldBeTrue();
        });

        //Playtime Hours validation errors
        errors.Count(x => x.Identifier == nameof(Playtime.Hours)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(errors => errors.Identifier == nameof(Playtime.Hours) && errors.ErrorCode == $"{nameof(Playtime.Hours)}.GreaterThanOrEqualToZero");

        //Playtime PlayerCount validation errors
        errors.Count(x => x.Identifier == nameof(Playtime.PlayerCount)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(errors => errors.Identifier == nameof(Playtime.PlayerCount) && errors.ErrorCode == $"{nameof(Playtime.PlayerCount)}.GreaterThanOrEqualToOne");

        //SystemRequirements Minimum validation errors
        errors.Count(x => x.Identifier == nameof(SystemRequirements.Minimum)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(SystemRequirements.Minimum) && x.ErrorCode == $"{nameof(SystemRequirements.Minimum)}.Required");

        errors.Count(x => x.Identifier == nameof(DomainGame.Rating)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.Rating) && x.ErrorCode == $"{nameof(DomainGame.Rating)}.LessThanOrEqualToTen");

        errors.Count(x => x.Identifier == nameof(DomainGame.GameStatus)).ShouldBe(0);
        errors.Count(x => x.Identifier == nameof(DomainGame.OfficialLink)).ShouldBe(0);

        //If counter is 0, it means that all errors were found
        errorCount.ShouldBe(0);
    }

    [Fact]
    public void Create_Game_Should_Return_Success_When_All_Fields_Are_Valid()
    {
        // Arrange
        var name = _faker.Commerce.ProductName();
        var releaseDate = DateOnly.FromDateTime(DateTime.Now);
        var ageRating = _faker.PickRandom(_ageRatings.ToArray());
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
            ageRating: _faker.PickRandom(_ageRatings.ToArray()),
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

        var errors = result.ValidationErrors;

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldNotBeNull().ShouldNotBeEmpty();
        errors.ShouldBeOfType<List<ValidationError>>();
        errors.Count().ShouldBe(1);
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.Price));
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Release_Date_Is_Default_Value()
    {
        // Arrange - Act
        var result = DomainGame.Create(
            name: _faker.Commerce.ProductName(),
            releaseDate: DateOnly.MinValue,
            ageRating: _faker.PickRandom(_ageRatings.ToArray()),
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

        var errors = result.ValidationErrors;

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldNotBeNull().ShouldNotBeEmpty();
        errors.ShouldBeOfType<List<ValidationError>>();
        errors.Count().ShouldBe(2);
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.ReleaseDate));
        errors.Count(x => x.Identifier == nameof(DomainGame.ReleaseDate)).ShouldBe(2);
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_DiskSize_Is_Negative()
    {
        // Arrange - Act
        var result = DomainGame.Create(
            _faker.Commerce.ProductName(),
            DateOnly.FromDateTime(DateTime.Now),
            _faker.PickRandom(_ageRatings.ToArray()),
            _faker.Lorem.Paragraph(),
            (_faker.Company.CompanyName(), _faker.Company.CompanyName()),
            -1,
            50,
            (10, 2),
            (
                genre: _faker.PickRandom("RPG", "Ação"),
                platform: _faker.PickRandom(_platforms, 3).ToArray(),
                tags: "Indie",
                gameMode: _faker.PickRandom(_gameModes),
                distributionFormat: _faker.PickRandom(_distributionFormats),
                availableLanguages: "EN-US",
                supportsDlcs: true
            ),
            (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph()),
            5,
            _faker.Internet.Url(),
            "Available"
        );

        var errors = result.ValidationErrors;

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldNotBeNull().ShouldNotBeEmpty();
        errors.ShouldBeOfType<List<ValidationError>>();
        errors.Count().ShouldBe(1);
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.DiskSize));
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_String_Fields_Exceeds_Max_Length()
    {
        // Arrange - Act
        var result = DomainGame.Create(
            name: new string('A', 201), // Max 200
            releaseDate: DateOnly.FromDateTime(DateTime.Now),
            ageRating: _faker.PickRandom(_ageRatings.ToArray()),
            description: new string('D', 2001), // Max 2000
            developerInfo: (new string('D', 5001), new string('D', 5001)), // Max 5000 each
            diskSize: 10,
            price: 50,
            playtime: (10, 2),
            gameDetails: (
                genre: new string('D', 501), // Max 500
                platform: _faker.PickRandom(_platforms, 3).ToArray(),
                tags: new string('D', 501), // Max 500
                gameMode: _faker.PickRandom(_gameModes),
                distributionFormat: _faker.PickRandom(_distributionFormats),
                availableLanguages: new string('A', 201), // Max 200
                supportsDlcs: true
            ),
            systemRequirements: (new string('M', 2001), new string('R', 2001)), // Max 2000 each
            rating: 5,
            officialLink: _faker.Internet.Url(),
            gameStatus: new string('A', 201) // Max 200
        );

        var errors = result.ValidationErrors;
        int errorCount = 7;

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldNotBeNull().ShouldNotBeEmpty();
        errors.ShouldBeOfType<List<ValidationError>>();
        errors.Count().ShouldBe(errorCount);

        // DeveloperInfo.Developer max length
        errors.Count(x => x.Identifier == nameof(DeveloperInfo.Developer)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DeveloperInfo.Developer) && x.ErrorCode == $"{nameof(DeveloperInfo.Developer)}.MaximumLength");

        // DeveloperInfo.Publisher max length
        errors.Count(x => x.Identifier == nameof(DeveloperInfo.Publisher)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DeveloperInfo.Publisher) && x.ErrorCode == $"{nameof(DeveloperInfo.Publisher)}.MaximumLength");

        // GameDetails.Genre max length
        errors.Count(x => x.Identifier == nameof(DomainGameDetails.Genre)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGameDetails.Genre) && x.ErrorCode == $"{nameof(DomainGameDetails.Genre)}.MaximumLength");

        // GameDetails.Tags max length
        errors.Count(x => x.Identifier == nameof(DomainGameDetails.Tags)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGameDetails.Tags) && x.ErrorCode == $"{nameof(DomainGameDetails.Tags)}.MaximumLength");

        // GameDetails.AvailableLanguages max length
        errors.Count(x => x.Identifier == nameof(DomainGameDetails.AvailableLanguages)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGameDetails.AvailableLanguages) && x.ErrorCode == $"{nameof(DomainGameDetails.AvailableLanguages)}.MaximumLength");

        // SystemRequirements.Minimum max length
        errors.Count(x => x.Identifier == nameof(SystemRequirements.Minimum)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(SystemRequirements.Minimum) && x.ErrorCode == $"{nameof(SystemRequirements.Minimum)}.MaximumLength");

        // SystemRequirements.Recommended max length
        errors.Count(x => x.Identifier == nameof(SystemRequirements.Recommended)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(SystemRequirements.Recommended) && x.ErrorCode == $"{nameof(SystemRequirements.Recommended)}.MaximumLength");

        // If counter is 0, it means that all errors were found
        errorCount.ShouldBe(0);
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Root_Class_String_Fields_Exceeds_Max_Length()
    {
        /*
            name,
            description,
            officialLink,
            gameStatus
             */
        // Arrange
        var name = new string('A', 201); // Max 200
        var releaseDate = DateOnly.FromDateTime(DateTime.Now);
        var ageRating = _faker.PickRandom(_ageRatings.ToArray());
        var description = new string('A', 2001); // Max 2000
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

        var errors = result.ValidationErrors;
        int errorCount = 2;

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldNotBeNull().ShouldNotBeEmpty();
        errors.ShouldBeOfType<List<ValidationError>>();
        errors.Count().ShouldBe(errorCount);

        // Name max length
        errors.Count(x => x.Identifier == nameof(DomainGame.Name)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.Name) && x.ErrorCode == $"{nameof(DomainGame.Name)}.MaximumLength");

        // Description max length
        errors.Count(x => x.Identifier == nameof(DomainGame.Description)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.Description) && x.ErrorCode == $"{nameof(DomainGame.Description)}.MaximumLength");

        // If counter is 0, it means that all errors were found
        errorCount.ShouldBe(0);
    }
}