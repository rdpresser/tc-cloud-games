using TC.CloudGames.Domain.Aggregates.Game.ValueObjects;
using DeveloperInfo = TC.CloudGames.Domain.Aggregates.Game.ValueObjects.DeveloperInfo;
using DomainGame = TC.CloudGames.Domain.Aggregates.Game.Game;
using GameDetails = TC.CloudGames.Domain.Aggregates.Game.ValueObjects.GameDetails;
using Playtime = TC.CloudGames.Domain.Aggregates.Game.ValueObjects.Playtime;
using Price = TC.CloudGames.Domain.Aggregates.Game.ValueObjects.Price;
using SystemRequirements = TC.CloudGames.Domain.Aggregates.Game.ValueObjects.SystemRequirements;

namespace TC.CloudGames.Unit.Tests.Domain.Game;

public class GameTests
{
    private readonly List<string> _ageRatings;
    private readonly List<string> _distributionFormats;
    private readonly Faker _faker;
    private readonly List<string> _gameModes;
    private readonly List<string> _gameStatus;
    private readonly List<string> _gameTags;
    private readonly List<string> _genres;
    private readonly List<string> _languages;
    private readonly List<string> _platforms;

    public GameTests()
    {
        _faker = new Faker();

        _genres =
        [
            "Action", "Adventure", "RPG", "Strategy", "Simulation", "Racing", "Sport", "Puzzle",
            "Fighter", "Platform", "FPS", "TPS", "Survival", "Horror", "Stealth", "Open World", "MMORPG", "Roguelike",
            "Visual Novel", "Beat 'em up", "Battle Royale", "Musical", "Party Game", "Metroidvania", "Idle/Incremental",
            "Tower Defense", "MOBA", "Sandbox", "Tycoon"
        ];

        _platforms = [.. GameDetails.ValidPlatforms];

        _gameTags =
        [
            "Indie", "Multiplayer", "Singleplayer", "Co-op", "PvP", "PvE", "Online Co-op",
            "Local Multiplayer", "Story Rich", "Difficult", "Casual", "Anime", "Pixel Graphics", "Retro", "Funny",
            "Atmospheric",
            "Horror", "Sci-fi", "Fantasy", "Cyberpunk", "Steampunk", "Post-apocalyptic", "Choices Matter", "Narration",
            "Character Customization", "Exploration", "Loot", "Crafting", "Building", "Resource Management",
            "Base Building",
            "Turn-Based", "Real Time", "Fast-Paced", "Third Person", "First Person", "Top-Down", "Isometric",
            "Stylized",
            "Realistic", "Female Protagonist", "Controller Support", "VR Support", "Moddable", "Replay Value",
            "Open World",
            "Procedural Generation", "Sandbox", "Nonlinear", "Mystery", "Psychological", "Dark", "Gore", "Violent"
        ];

        _gameModes = [.. GameDetails.ValidGameModes];

        _distributionFormats = [.. GameDetails.ValidDistributionFormats];

        _languages = ["PT-BR", "EN-US", "ES-ES", "FR-FR", "ZH-CN", "JA-JP", "RU-RU", "KO-KR"];

        _gameStatus = [.. DomainGame.ValidGameStatus];

        _ageRatings = [.. AgeRating.ValidRatings];
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_AgeRating_Is_Invalid()
    {
        // Arrange
        var configure = new[] { "Invalid" };

        // Act
        var result = DomainGame.Create(builder =>
        {
            builder.Name =
                $"{_faker.Commerce.ProductAdjective()} {_faker.Commerce.ProductMaterial()} {_faker.Commerce.Product()}";
            builder.ReleaseDate = DateOnly.FromDateTime(_faker.Date.Past());
            builder.AgeRating = "Invalid";
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = _faker.Random.Int(1, 150);
            builder.Price = decimal.Parse(_faker.Commerce.Price(1.0m, 500.0m));
            builder.Playtime = (_faker.Random.Int(1, 200), _faker.Random.Int(1, 2000));
            builder.GameDetails = (
                Genre: _faker.Lorem.Word(),
                Platform: configure,
                Tags: _faker.Lorem.Word(),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: "EN-US",
                SupportsDlcs: _faker.Random.Bool());
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldNotBeNull().ShouldNotBeEmpty();
        errors.Count(x => x.Identifier == nameof(DomainGame.AgeRating)).ShouldBe(2);
        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x =>
                    x.Identifier == nameof(DomainGame.AgeRating) && x.ErrorCode == $"{nameof(AgeRating)}.Required")
                .ShouldBeTrue();
            errors.Any(x =>
                    x.Identifier == nameof(DomainGame.AgeRating) && x.ErrorCode == $"{nameof(AgeRating)}.ValidRating")
                .ShouldBeTrue();
        });
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Platform_Is_Invalid()
    {
        // Arrange
        var configure = new[] { "Invalid" };

        // Act
        var result = DomainGame.Create(builder =>
        {
            builder.Name =
                $"{_faker.Commerce.ProductAdjective()} {_faker.Commerce.ProductMaterial()} {_faker.Commerce.Product()}";
            builder.ReleaseDate = DateOnly.FromDateTime(_faker.Date.Past());
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = _faker.Random.Int(1, 150);
            builder.Price = decimal.Parse(_faker.Commerce.Price(1.0m, 500.0m));
            builder.Playtime = (_faker.Random.Int(1, 200), _faker.Random.Int(1, 2000));
            builder.GameDetails = (
                Genre: _faker.Lorem.Word(),
                Platform: configure,
                Tags: _faker.Lorem.Word(),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: "EN-US",
                SupportsDlcs: _faker.Random.Bool());
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldNotBeNull().ShouldNotBeEmpty();
        errors.Count(x => x.Identifier == nameof(GameDetails.Platform)).ShouldBe(1);
        errors.ShouldContain(x =>
            x.Identifier == nameof(GameDetails.Platform) &&
            x.ErrorCode == $"{nameof(GameDetails.Platform)}.ValidPlatform");
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_GameMode_Is_Invalid()
    {
        // Arrange
        var configure = _platforms.ToArray();

        // Act
        var result = DomainGame.Create(builder =>
        {
            builder.Name =
                $"{_faker.Commerce.ProductAdjective()} {_faker.Commerce.ProductMaterial()} {_faker.Commerce.Product()}";
            builder.ReleaseDate = DateOnly.FromDateTime(_faker.Date.Past());
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = _faker.Random.Int(1, 150);
            builder.Price = decimal.Parse(_faker.Commerce.Price(1.0m, 500.0m));
            builder.Playtime = (_faker.Random.Int(1, 200), _faker.Random.Int(1, 2000));
            builder.GameDetails = (
                Genre: _faker.Lorem.Word(),
                Platform: configure,
                Tags: _faker.Lorem.Word(),
                GameMode: "Invalid",
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: "EN-US",
                SupportsDlcs: _faker.Random.Bool());
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldNotBeNull().ShouldNotBeEmpty();
        errors.Count(x => x.Identifier == nameof(GameDetails.GameMode)).ShouldBe(1);
        errors.ShouldContain(x =>
            x.Identifier == nameof(GameDetails.GameMode) &&
            x.ErrorCode == $"{nameof(GameDetails.GameMode)}.ValidGameMode");
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_DistributionFormat_Is_Invalid()
    {
        // Arrange
        var configure = _platforms.ToArray();

        // Act
        var result = DomainGame.Create(builder =>
        {
            builder.Name =
                $"{_faker.Commerce.ProductAdjective()} {_faker.Commerce.ProductMaterial()} {_faker.Commerce.Product()}";
            builder.ReleaseDate = DateOnly.FromDateTime(_faker.Date.Past());
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = _faker.Random.Int(1, 150);
            builder.Price = decimal.Parse(_faker.Commerce.Price(1.0m, 500.0m));
            builder.Playtime = (_faker.Random.Int(1, 200), _faker.Random.Int(1, 2000));
            builder.GameDetails = (
                Genre: _faker.Lorem.Word(),
                Platform: configure,
                Tags: _faker.Lorem.Word(),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: "Invalid",
                AvailableLanguages: "EN-US",
                SupportsDlcs: _faker.Random.Bool());
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldNotBeNull().ShouldNotBeEmpty();
        errors.Count(x => x.Identifier == nameof(GameDetails.DistributionFormat)).ShouldBe(1);
        errors.ShouldContain(x =>
            x.Identifier == nameof(GameDetails.DistributionFormat) &&
            x.ErrorCode == $"{nameof(GameDetails.DistributionFormat)}.ValidDistributionFormat");
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Rating_Is_Negative()
    {
        // Arrange
        var configure = _platforms.ToArray();

        // Act
        var result = DomainGame.Create(builder =>
        {
            builder.Name =
                $"{_faker.Commerce.ProductAdjective()} {_faker.Commerce.ProductMaterial()} {_faker.Commerce.Product()}";
            builder.ReleaseDate = DateOnly.FromDateTime(_faker.Date.Past());
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = _faker.Random.Int(1, 150);
            builder.Price = decimal.Parse(_faker.Commerce.Price(1.0m, 500.0m));
            builder.Playtime = (_faker.Random.Int(1, 200), _faker.Random.Int(1, 2000));
            builder.GameDetails = (
                Genre: _faker.Lorem.Word(),
                Platform: configure,
                Tags: _faker.Lorem.Word(),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: "EN-US",
                SupportsDlcs: _faker.Random.Bool());
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = -1;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldNotBeNull().ShouldNotBeEmpty();
        errors.Count(x => x.Identifier == nameof(DomainGame.Rating)).ShouldBe(1);
        errors.ShouldContain(x =>
            x.Identifier == nameof(DomainGame.Rating) &&
            x.ErrorCode == $"{nameof(DomainGame.Rating)}.GreaterThanOrEqualToZero");
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_GameDetails_Is_Invalid()
    {
        // Arrange
        var configure = _platforms.ToArray();

        // Act
        var result = DomainGame.Create(builder =>
        {
            builder.Name =
                $"{_faker.Commerce.ProductAdjective()} {_faker.Commerce.ProductMaterial()} {_faker.Commerce.Product()}";
            builder.ReleaseDate = DateOnly.FromDateTime(_faker.Date.Past());
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = _faker.Random.Int(1, 150);
            builder.Price = decimal.Parse(_faker.Commerce.Price(1.0m, 500.0m));
            builder.Playtime = (_faker.Random.Int(1, 200), _faker.Random.Int(1, 2000));
            builder.GameDetails = (
                Genre: _faker.Lorem.Word(),
                Platform: configure,
                Tags: _faker.Lorem.Word(),
                GameMode: "Invalid",
                DistributionFormat: "Invalid",
                AvailableLanguages: "EN-US",
                SupportsDlcs: _faker.Random.Bool());
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldNotBeNull().ShouldNotBeEmpty();
        errors.Count(x => x.Identifier == nameof(GameDetails)).ShouldBe(1);
        errors.ShouldContain(x =>
            x.Identifier == nameof(GameDetails) && x.ErrorCode == $"{nameof(GameDetails)}.Required");
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_GameStatus_Is_Invalid()
    {
        // Arrange
        var configure = _platforms.ToArray();

        // Act
        var result = DomainGame.Create(builder =>
        {
            builder.Name =
                $"{_faker.Commerce.ProductAdjective()} {_faker.Commerce.ProductMaterial()} {_faker.Commerce.Product()}";
            builder.ReleaseDate = DateOnly.FromDateTime(_faker.Date.Past());
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = _faker.Random.Int(1, 150);
            builder.Price = decimal.Parse(_faker.Commerce.Price(1.0m, 500.0m));
            builder.Playtime = (_faker.Random.Int(1, 200), _faker.Random.Int(1, 2000));
            builder.GameDetails = (
                Genre: _faker.Lorem.Word(),
                Platform: configure,
                Tags: _faker.Lorem.Word(),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: "EN-US",
                SupportsDlcs: _faker.Random.Bool());
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = "Invalid";
        });

        var errors = result.ValidationErrors;

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldNotBeNull().ShouldNotBeEmpty();
        errors.Count(x => x.Identifier == nameof(DomainGame.GameStatus)).ShouldBe(1);
        errors.ShouldContain(x =>
            x.Identifier == nameof(DomainGame.GameStatus) &&
            x.ErrorCode == $"{nameof(DomainGame.GameStatus)}.ValidGameStatus");
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_OfficialLink_Is_Invalid()
    {
        // Arrange
        var configure = _platforms.ToArray();

        // Act
        var result = DomainGame.Create(builder =>
        {
            builder.Name =
                $"{_faker.Commerce.ProductAdjective()} {_faker.Commerce.ProductMaterial()} {_faker.Commerce.Product()}";
            builder.ReleaseDate = DateOnly.FromDateTime(_faker.Date.Past());
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = _faker.Random.Int(1, 150);
            builder.Price = decimal.Parse(_faker.Commerce.Price(1.0m, 500.0m));
            builder.Playtime = (_faker.Random.Int(1, 200), _faker.Random.Int(1, 2000));
            builder.GameDetails = (
                Genre: _faker.Lorem.Word(),
                Platform: configure,
                Tags: _faker.Lorem.Word(),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: "EN-US",
                SupportsDlcs: _faker.Random.Bool());
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = "Invalid_URL";
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldNotBeNull().ShouldNotBeEmpty();
        errors.Count(x => x.Identifier == nameof(DomainGame.OfficialLink)).ShouldBe(1);
        errors.ShouldContain(x =>
            x.Identifier == nameof(DomainGame.OfficialLink) &&
            x.ErrorCode == $"{nameof(DomainGame.OfficialLink)}.ValidUrl");
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Name_Is_Empty()
    {
        var result = DomainGame.Create(builder =>
        {
            builder.Name = string.Empty;
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (1, 1);
            builder.GameDetails = (
                Genre: _faker.PickRandom(_genres),
                Platform: _faker.PickRandom(_platforms, 1).ToArray(),
                Tags: _faker.PickRandom(_gameTags),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: _faker.PickRandom(_languages),
                SupportsDlcs: true);
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldContain(x =>
            x.Identifier == nameof(DomainGame.Name) && x.ErrorCode == $"{nameof(DomainGame.Name)}.Required");
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_ReleaseDate_Is_Default()
    {
        var result = DomainGame.Create(builder =>
        {
            builder.Name = _faker.Commerce.ProductName();
            builder.ReleaseDate = DateOnly.MinValue;
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (1, 1);
            builder.GameDetails = (
                Genre: _faker.PickRandom(_genres),
                Platform: _faker.PickRandom(_platforms, 1).ToArray(),
                Tags: _faker.PickRandom(_gameTags),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: _faker.PickRandom(_languages),
                SupportsDlcs: true);
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.Count(x => x.Identifier == nameof(DomainGame.ReleaseDate)).ShouldBe(2);
        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x =>
                x.Identifier == nameof(DomainGame.ReleaseDate) &&
                x.ErrorCode == $"{nameof(DomainGame.ReleaseDate)}.Required").ShouldBeTrue();
            errors.Any(x =>
                x.Identifier == nameof(DomainGame.ReleaseDate) &&
                x.ErrorCode == $"{nameof(DomainGame.ReleaseDate)}.ValidDate").ShouldBeTrue();
        });
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_AgeRating_Is_Empty()
    {
        var result = DomainGame.Create(builder =>
        {
            builder.Name = _faker.Commerce.ProductName();
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = string.Empty;
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (1, 1);
            builder.GameDetails = (
                Genre: _faker.PickRandom(_genres),
                Platform: _faker.PickRandom(_platforms, 1).ToArray(),
                Tags: _faker.PickRandom(_gameTags),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: _faker.PickRandom(_languages),
                SupportsDlcs: true);
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.Count(x => x.Identifier == nameof(DomainGame.AgeRating)).ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Developer_Is_Empty()
    {
        var result = DomainGame.Create(builder =>
        {
            builder.Name = _faker.Commerce.ProductName();
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (string.Empty, _faker.Company.CompanyName());
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (1, 1);
            builder.GameDetails = (
                Genre: _faker.PickRandom(_genres),
                Platform: _faker.PickRandom(_platforms, 1).ToArray(),
                Tags: _faker.PickRandom(_gameTags),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: _faker.PickRandom(_languages),
                SupportsDlcs: true);
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldContain(x =>
            x.Identifier == nameof(DeveloperInfo.Developer) &&
            x.ErrorCode == $"{nameof(DeveloperInfo.Developer)}.Required");
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_DiskSize_Is_Zero()
    {
        var result = DomainGame.Create(builder =>
        {
            builder.Name = _faker.Commerce.ProductName();
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = 0;
            builder.Price = 50;
            builder.Playtime = (1, 1);
            builder.GameDetails = (
                Genre: _faker.PickRandom(_genres),
                Platform: _faker.PickRandom(_platforms, 1).ToArray(),
                Tags: _faker.PickRandom(_gameTags),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: _faker.PickRandom(_languages),
                SupportsDlcs: true);
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.Count(x => x.Identifier == nameof(DomainGame.DiskSize)).ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Price_Is_Negative()
    {
        var result = DomainGame.Create(builder =>
        {
            builder.Name = _faker.Commerce.ProductName();
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = 10;
            builder.Price = -5;
            builder.Playtime = (1, 1);
            builder.GameDetails = (
                Genre: _faker.PickRandom(_genres),
                Platform: _faker.PickRandom(_platforms, 1).ToArray(),
                Tags: _faker.PickRandom(_gameTags),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: _faker.PickRandom(_languages),
                SupportsDlcs: true);
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.Count(x => x.Identifier == nameof(DomainGame.Price)).ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Playtime_Is_Invalid()
    {
        var result = DomainGame.Create(builder =>
        {
            builder.Name = _faker.Commerce.ProductName();
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (-1, 0);
            builder.GameDetails = (
                Genre: _faker.PickRandom(_genres),
                Platform: _faker.PickRandom(_platforms, 1).ToArray(),
                Tags: _faker.PickRandom(_gameTags),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: _faker.PickRandom(_languages),
                SupportsDlcs: true);
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldContain(x => x.Identifier == nameof(Playtime.Hours));
        errors.ShouldContain(x => x.Identifier == nameof(Playtime.PlayerCount));
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_GameDetails_Are_Empty()
    {
        var result = DomainGame.Create(builder =>
        {
            builder.Name = _faker.Commerce.ProductName();
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (1, 1);
            builder.GameDetails = (
                Genre: _faker.PickRandom(_genres),
                Platform: [],
                Tags: _faker.PickRandom(_gameTags),
                GameMode: string.Empty,
                DistributionFormat: string.Empty,
                AvailableLanguages: _faker.PickRandom(_languages),
                SupportsDlcs: true);
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldContain(x => x.Identifier == nameof(GameDetails.Platform));
        errors.ShouldContain(x => x.Identifier == nameof(GameDetails.GameMode));
        errors.ShouldContain(x => x.Identifier == nameof(GameDetails.DistributionFormat));
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_SystemRequirements_Minimum_Is_Empty()
    {
        var result = DomainGame.Create(builder =>
        {
            builder.Name = _faker.Commerce.ProductName();
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (1, 1);
            builder.GameDetails = (
                Genre: _faker.PickRandom(_genres),
                Platform: _faker.PickRandom(_platforms, 1).ToArray(),
                Tags: _faker.PickRandom(_gameTags),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: _faker.PickRandom(_languages),
                SupportsDlcs: true);
            builder.SystemRequirements = (string.Empty, _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldContain(x => x.Identifier == nameof(SystemRequirements.Minimum));
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Rating_Is_Out_Of_Range()
    {
        var result = DomainGame.Create(builder =>
        {
            builder.Name = _faker.Commerce.ProductName();
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (1, 1);
            builder.GameDetails = (
                Genre: _faker.PickRandom(_genres),
                Platform: _faker.PickRandom(_platforms, 1).ToArray(),
                Tags: _faker.PickRandom(_gameTags),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: _faker.PickRandom(_languages),
                SupportsDlcs: true);
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 11;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.Rating));
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_GameStatus_Is_Empty()
    {
        var result = DomainGame.Create(builder =>
        {
            builder.Name = _faker.Commerce.ProductName();
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (1, 1);
            builder.GameDetails = (
                Genre: _faker.PickRandom(_genres),
                Platform: _faker.PickRandom(_platforms, 1).ToArray(),
                Tags: _faker.PickRandom(_gameTags),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: _faker.PickRandom(_languages),
                SupportsDlcs: true);
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = string.Empty;
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.GameStatus));
    }


    [Fact]
    public void Create_Game_Using_Builder_Should_Return_Success_When_All_Fields_Are_Valid()
    {
        // Arrange
        var name = _faker.Commerce.ProductName();
        var releaseDate = DateOnly.FromDateTime(DateTime.Now);
        var ageRating = AgeRating.Create(builder => builder.Value = _faker.PickRandom(_ageRatings.ToArray()));
        var description = _faker.Lorem.Paragraph();
        var developerInfo = DeveloperInfo.Create(builder =>
        {
            builder.Developer = _faker.Company.CompanyName();
            builder.Publisher = _faker.Company.CompanyName();
        });
        var diskSize = DiskSize.Create(builder => builder.SizeInGb = _faker.Random.Decimal(1, 100));
        var price = Price.Create(builder => builder.Amount = _faker.Random.Decimal(10, 300));
        var playtime = Playtime.Create(builder =>
        {
            builder.Hours = _faker.Random.Int(1, 10);
            builder.PlayerCount = _faker.Random.Int(10, 100);
        });

        // Act
        var result = DomainGame.CreateFromValueObjects(builder =>
        {
            builder.Name = name;
            builder.ReleaseDate = releaseDate;
            builder.AgeRating = ageRating;
            builder.Description = description;
            builder.DeveloperInfo = developerInfo;
            builder.DiskSize = diskSize;
            builder.Price = price;
            builder.Playtime = playtime;
            builder.GameDetails = GameDetails.Create(builder =>
            {
                builder.Genre = _faker.PickRandom(_genres);
                builder.Platform = [.. _faker.PickRandom(_platforms, 3)];
                builder.Tags = _faker.PickRandom(_gameTags);
                builder.GameMode = _faker.PickRandom(_gameModes);
                builder.DistributionFormat = _faker.PickRandom(_distributionFormats);
                builder.AvailableLanguages = _faker.PickRandom(_languages);
                builder.SupportsDlcs = true;
            });
            builder.SystemRequirements = SystemRequirements.Create(builder =>
            {
                builder.Minimum = _faker.Lorem.Paragraph();
                builder.Recommended = _faker.Lorem.Paragraph();
            });
            builder.Rating = Rating.Create(builder => builder.Average = null);
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldNotBeNull();
        result.Value.Name.ShouldBe(name);
        result.Value.ReleaseDate.ShouldBe(releaseDate);
    }

    [Fact]
    public void Create_Game_Using_Builder_Should_Return_Failure_When_Value_Objects_Are_Invalid()
    {
        // Arrange
        var name = _faker.Commerce.ProductName();
        var releaseDate = DateOnly.FromDateTime(DateTime.Now);
        var ageRating = AgeRating.Create(builder => { });
        var description = _faker.Lorem.Paragraph();
        var developerInfo = DeveloperInfo.Create(builder => { });
        var diskSize = DiskSize.Create(builder => { });
        var price = Price.Create(builder => { builder.Amount = -1; });
        var playtime = Playtime.Create(builder =>
        {
            builder.Hours = -1;
            builder.PlayerCount = -1;
        });
        var gameDetails = GameDetails.Create(builder => { });
        var systemRequirements = SystemRequirements.Create(builder => { });
        var rating = Rating.Create(builder => builder.Average = -1);

        // Act
        var result = DomainGame.CreateFromValueObjects(builder =>
        {
            builder.Name = name;
            builder.ReleaseDate = releaseDate;
            builder.AgeRating = ageRating;
            builder.Description = description;
            builder.DeveloperInfo = developerInfo;
            builder.DiskSize = diskSize;
            builder.Price = price;
            builder.Playtime = playtime;
            builder.GameDetails = gameDetails;
            builder.SystemRequirements = systemRequirements;
            builder.Rating = rating;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_Game_Using_Builder_Result_Should_Return_Failure_When_Value_Objects_Are_Invalid()
    {
        // Arrange
        var name = _faker.Commerce.ProductName();
        var releaseDate = DateOnly.FromDateTime(DateTime.Now);
        var ageRating = AgeRating.Create(builder => { });
        var description = _faker.Lorem.Paragraph();
        var developerInfo = DeveloperInfo.Create(builder => { });
        var diskSize = DiskSize.Create(builder => { });
        var price = Price.Create(builder => { builder.Amount = -1; });
        var playtime = Playtime.Create(builder =>
        {
            builder.Hours = -1;
            builder.PlayerCount = -1;
        });
        var gameDetails = GameDetails.Create(builder => { });
        var systemRequirements = SystemRequirements.Create(builder => { });
        var rating = Rating.Create(builder => builder.Average = -1);

        // Act
        var result = DomainGame.CreateFromResult(builder =>
        {
            builder.Name = name;
            builder.ReleaseDate = releaseDate;
            builder.AgeRating = ageRating;
            builder.Description = description;
            builder.DeveloperInfo = developerInfo;
            builder.DiskSize = diskSize;
            builder.Price = price;
            builder.Playtime = playtime;
            builder.GameDetails = gameDetails;
            builder.SystemRequirements = systemRequirements;
            builder.Rating = rating;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_Game_Using_Builder_Result_Should_Return_Success_When_All_Fields_Are_Valid()
    {
        // Arrange
        var name = _faker.Commerce.ProductName();
        var releaseDate = DateOnly.FromDateTime(DateTime.Now);
        var ageRating = AgeRating.Create(builder => builder.Value = _faker.PickRandom(_ageRatings.ToArray()));
        var description = _faker.Lorem.Paragraph();
        var developerInfo = DeveloperInfo.Create(builder =>
        {
            builder.Developer = _faker.Company.CompanyName();
            builder.Publisher = _faker.Company.CompanyName();
        });
        var diskSize = DiskSize.Create(builder => builder.SizeInGb = _faker.Random.Decimal(1, 100));
        var price = Price.Create(builder => builder.Amount = _faker.Random.Decimal(10, 300));
        var playtime = Playtime.Create(builder =>
        {
            builder.Hours = _faker.Random.Int(1, 10);
            builder.PlayerCount = _faker.Random.Int(10, 100);
        });

        // Act
        var result = DomainGame.CreateFromResult(builder =>
        {
            builder.Name = name;
            builder.ReleaseDate = releaseDate;
            builder.AgeRating = ageRating;
            builder.Description = description;
            builder.DeveloperInfo = developerInfo;
            builder.DiskSize = diskSize;
            builder.Price = price;
            builder.Playtime = playtime;
            builder.GameDetails = GameDetails.Create(builder =>
            {
                builder.Genre = _faker.PickRandom(_genres);
                builder.Platform = [.. _faker.PickRandom(_platforms, 3)];
                builder.Tags = _faker.PickRandom(_gameTags);
                builder.GameMode = _faker.PickRandom(_gameModes);
                builder.DistributionFormat = _faker.PickRandom(_distributionFormats);
                builder.AvailableLanguages = _faker.PickRandom(_languages);
                builder.SupportsDlcs = true;
            });
            builder.SystemRequirements = SystemRequirements.Create(builder =>
            {
                builder.Minimum = _faker.Lorem.Paragraph();
                builder.Recommended = _faker.Lorem.Paragraph();
            });
            builder.Rating = Rating.Create(builder => builder.Average = null);
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldNotBeNull();
        result.Value.Name.ShouldBe(name);
        result.Value.ReleaseDate.ShouldBe(releaseDate);
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
        var result = DomainGame.Create(builder =>
        {
            builder.Name = name;
            builder.ReleaseDate = releaseDate;
            builder.AgeRating = ageRating;
            builder.Description = description;
            builder.DeveloperInfo = developerInfo;
            builder.DiskSize = diskSize;
            builder.Price = price;
            builder.Playtime = playtime;
            builder.GameDetails = (
                Genre: _faker.PickRandom(_genres),
                Platform: _faker.PickRandom(_platforms, 3).ToArray(),
                Tags: _faker.PickRandom(_gameTags),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: _faker.PickRandom(_languages),
                SupportsDlcs: true);
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = null;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldNotBeNull();
        result.Value.Name.ShouldBe(name);
        result.Value.ReleaseDate.ShouldBe(releaseDate);
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Release_Date_Is_Default_Value()
    {
        // Arrange - Act
        var result = DomainGame.Create(builder =>
        {
            builder.Name = _faker.Commerce.ProductName();
            builder.ReleaseDate = DateOnly.MinValue;
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = (decimal)10.5;
            builder.Price = 50;
            builder.Playtime = (_faker.Random.Int(1, 10), _faker.Random.Int(10, 100));
            builder.GameDetails = (
                Genre: _faker.PickRandom(_genres),
                Platform: _faker.PickRandom(_platforms, 3).ToArray(),
                Tags: _faker.PickRandom(_gameTags),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: _faker.PickRandom(_languages),
                SupportsDlcs: true);
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = null;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

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
        var result = DomainGame.Create(builder =>
        {
            builder.Name = _faker.Commerce.ProductName();
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = -1;
            builder.Price = 50;
            builder.Playtime = (10, 2);
            builder.GameDetails = (
                Genre: _faker.PickRandom("RPG", "Ação"),
                Platform: _faker.PickRandom(_platforms, 3).ToArray(),
                Tags: "Indie",
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: "EN-US",
                SupportsDlcs: true
            );
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = "Available";
        });

        var errors = result.ValidationErrors;

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldNotBeNull().ShouldNotBeEmpty();
        errors.ShouldBeOfType<List<ValidationError>>();
        errors.Count().ShouldBe(2);

        errors.Count(x => x.Identifier == nameof(DomainGame.DiskSize)).ShouldBe(2);
        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x =>
                    x.Identifier == nameof(DomainGame.DiskSize) &&
                    x.ErrorCode == $"{nameof(DomainGame.DiskSize)}.Required")
                .ShouldBeTrue();
            errors.Any(x =>
                x.Identifier == nameof(DomainGame.DiskSize) &&
                x.ErrorCode == $"{nameof(DomainGame.DiskSize)}.GreaterThanZero").ShouldBeTrue();
        });
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Name_Exceeds_Max_Length()
    {
        var name = new string('A', 201); // Max 200
        var result = DomainGame.Create(builder =>
        {
            builder.Name = name;
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (10, 2);
            builder.GameDetails = (
                Genre: _faker.PickRandom(_genres),
                Platform: _faker.PickRandom(_platforms, 3).ToArray(),
                Tags: _faker.PickRandom(_gameTags),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: _faker.PickRandom(_languages),
                SupportsDlcs: true
            );
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldContain(x =>
            x.Identifier == nameof(DomainGame.Name) && x.ErrorCode == $"{nameof(DomainGame.Name)}.MaximumLength");
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Description_Exceeds_Max_Length()
    {
        var description = new string('A', 2001); // Max 2000
        var result = DomainGame.Create(builder =>
        {
            builder.Name = _faker.Commerce.ProductName();
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = description;
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (10, 2);
            builder.GameDetails = (
                Genre: _faker.PickRandom(_genres),
                Platform: _faker.PickRandom(_platforms, 3).ToArray(),
                Tags: _faker.PickRandom(_gameTags),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: _faker.PickRandom(_languages),
                SupportsDlcs: true
            );
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldContain(x =>
            x.Identifier == nameof(DomainGame.Description) &&
            x.ErrorCode == $"{nameof(DomainGame.Description)}.MaximumLength");
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_DeveloperInfo_Exceeds_Max_Length()
    {
        var dev = new string('D', 5001); // Max 5000
        var result = DomainGame.Create(builder =>
        {
            builder.Name = _faker.Commerce.ProductName();
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (dev, dev);
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (10, 2);
            builder.GameDetails = (
                Genre: _faker.PickRandom(_genres),
                Platform: _faker.PickRandom(_platforms, 3).ToArray(),
                Tags: _faker.PickRandom(_gameTags),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: _faker.PickRandom(_languages),
                SupportsDlcs: true
            );
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldContain(x =>
            x.Identifier == nameof(DeveloperInfo.Developer) &&
            x.ErrorCode == $"{nameof(DeveloperInfo.Developer)}.MaximumLength");
        errors.ShouldContain(x =>
            x.Identifier == nameof(DeveloperInfo.Publisher) &&
            x.ErrorCode == $"{nameof(DeveloperInfo.Publisher)}.MaximumLength");
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_GameDetails_Fields_Exceed_Max_Length()
    {
        var genre = new string('D', 501); // Max 500
        var tags = new string('D', 501); // Max 500
        var availableLanguages = new string('A', 201); // Max 200
        var result = DomainGame.Create(builder =>
        {
            builder.Name = _faker.Commerce.ProductName();
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (10, 2);
            builder.GameDetails = (
                Genre: genre,
                Platform: _faker.PickRandom(_platforms, 3).ToArray(),
                Tags: tags,
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: availableLanguages,
                SupportsDlcs: true
            );
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldContain(x =>
            x.Identifier == nameof(GameDetails.Genre) && x.ErrorCode == $"{nameof(GameDetails.Genre)}.MaximumLength");
        errors.ShouldContain(x =>
            x.Identifier == nameof(GameDetails.Tags) && x.ErrorCode == $"{nameof(GameDetails.Tags)}.MaximumLength");
        errors.ShouldContain(x =>
            x.Identifier == nameof(GameDetails.AvailableLanguages) &&
            x.ErrorCode == $"{nameof(GameDetails.AvailableLanguages)}.MaximumLength");
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_SystemRequirements_Fields_Exceed_Max_Length()
    {
        var min = new string('M', 2001); // Max 2000
        var rec = new string('R', 2001); // Max 2000
        var result = DomainGame.Create(builder =>
        {
            builder.Name = _faker.Commerce.ProductName();
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (10, 2);
            builder.GameDetails = (
                Genre: _faker.PickRandom(_genres),
                Platform: _faker.PickRandom(_platforms, 3).ToArray(),
                Tags: _faker.PickRandom(_gameTags),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: _faker.PickRandom(_languages),
                SupportsDlcs: true
            );
            builder.SystemRequirements = (min, rec);
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldContain(x =>
            x.Identifier == nameof(SystemRequirements.Minimum) &&
            x.ErrorCode == $"{nameof(SystemRequirements.Minimum)}.MaximumLength");
        errors.ShouldContain(x =>
            x.Identifier == nameof(SystemRequirements.Recommended) &&
            x.ErrorCode == $"{nameof(SystemRequirements.Recommended)}.MaximumLength");
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_OfficialLink_Exceeds_Max_Length()
    {
        var officialLink = $"https://{new string('A', 201)}.com"; // Exceeds max
        var result = DomainGame.Create(builder =>
        {
            builder.Name = _faker.Commerce.ProductName();
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (10, 2);
            builder.GameDetails = (
                Genre: _faker.PickRandom(_genres),
                Platform: _faker.PickRandom(_platforms, 3).ToArray(),
                Tags: _faker.PickRandom(_gameTags),
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: _faker.PickRandom(_languages),
                SupportsDlcs: true
            );
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = 5;
            builder.OfficialLink = officialLink;
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldContain(x =>
            x.Identifier == nameof(DomainGame.OfficialLink) &&
            x.ErrorCode == $"{nameof(DomainGame.OfficialLink)}.MaximumLength");
    }


    private static IEnumerable<(string Identifier, int Count, IEnumerable<string> ErrorCodes)>
        GroupValidationErrorsByIdentifier(IEnumerable<ValidationError> errors)
    {
        return errors
            .GroupBy(e => e.Identifier)
            .Select(g => (
                Identifier: g.Key,
                Count: g.Count(),
                ErrorCodes: g.Select(e => $"{e.ErrorCode} - {e.ErrorMessage}")
            ));
    }
}