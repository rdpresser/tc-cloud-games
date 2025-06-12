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

        _genres = [ "Action", "Adventure", "RPG", "Strategy", "Simulation", "Racing", "Sport", "Puzzle",
            "Fighter", "Platform", "FPS", "TPS", "Survival", "Horror", "Stealth", "Open World", "MMORPG", "Roguelike",
            "Visual Novel", "Beat 'em up", "Battle Royale", "Musical", "Party Game", "Metroidvania", "Idle/Incremental",
            "Tower Defense", "MOBA", "Sandbox", "Tycoon" ];

        _platforms = [.. GameDetails.ValidPlatforms];

        _gameTags = [ "Indie", "Multiplayer", "Singleplayer", "Co-op", "PvP", "PvE", "Online Co-op",
            "Local Multiplayer", "Story Rich", "Difficult", "Casual", "Anime", "Pixel Graphics", "Retro", "Funny", "Atmospheric",
            "Horror", "Sci-fi", "Fantasy", "Cyberpunk", "Steampunk", "Post-apocalyptic", "Choices Matter", "Narration",
            "Character Customization", "Exploration", "Loot", "Crafting", "Building", "Resource Management", "Base Building",
            "Turn-Based", "Real Time", "Fast-Paced", "Third Person", "First Person", "Top-Down", "Isometric", "Stylized",
            "Realistic", "Female Protagonist", "Controller Support", "VR Support", "Moddable", "Replay Value", "Open World",
            "Procedural Generation", "Sandbox", "Nonlinear", "Mystery", "Psychological", "Dark", "Gore", "Violent" ];

        _gameModes = [.. GameDetails.ValidGameModes];

        _distributionFormats = [.. GameDetails.ValidDistributionFormats];

        _languages = ["PT-BR", "EN-US", "ES-ES", "FR-FR", "ZH-CN", "JA-JP", "RU-RU", "KO-KR"];

        _gameStatus = [.. DomainGame.ValidGameStatus];

        _ageRatings = [.. AgeRating.ValidRatings];
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Fields_Have_Invalid_Values()
    {
        // Arrange
        string[] configure = ["Invalid"];

        // Act
        var result = DomainGame.Create(builder =>
        {
            builder.Name = $"{_faker.Commerce.ProductAdjective()} {_faker.Commerce.ProductMaterial()} {_faker.Commerce.Product()}";
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
                GameMode: "Invalid",
                DistributionFormat: "Invalid",
                AvailableLanguages: "EN-US",
                SupportsDlcs: _faker.Random.Bool());
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = -1;
            builder.OfficialLink = "Invalid_URL";
            builder.GameStatus = "Invalid";
        });

        var errors = result.ValidationErrors;
        int errorCount = 9;

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldNotBeNull().ShouldNotBeEmpty();
        errors.ShouldBeOfType<List<ValidationError>>();
        errors.Count().ShouldBe(errorCount);

        // AgeRating validation errors
        errors.Count(x => x.Identifier == nameof(DomainGame.AgeRating)).ShouldBe(2);
        errorCount -= 2;
        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x => x.Identifier == nameof(DomainGame.AgeRating) && x.ErrorCode == $"{nameof(AgeRating)}.Required").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(DomainGame.AgeRating) && x.ErrorCode == $"{nameof(AgeRating)}.ValidRating").ShouldBeTrue();
        });

        // Platform validation errors
        errors.Count(x => x.Identifier == nameof(GameDetails.Platform)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(GameDetails.Platform) && x.ErrorCode == $"{nameof(GameDetails.Platform)}.ValidPlatform");

        // GameMode validation errors
        errors.Count(x => x.Identifier == nameof(GameDetails.GameMode)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(GameDetails.GameMode) && x.ErrorCode == $"{nameof(GameDetails.GameMode)}.ValidGameMode");

        // DistributionFormat validation errors
        errors.Count(x => x.Identifier == nameof(GameDetails.DistributionFormat)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(GameDetails.DistributionFormat) && x.ErrorCode == $"{nameof(GameDetails.DistributionFormat)}.ValidDistributionFormat");

        // Rating validation errors
        errors.Count(x => x.Identifier == nameof(DomainGame.Rating)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.Rating) && x.ErrorCode == $"{nameof(DomainGame.Rating)}.GreaterThanOrEqualToZero");

        // GameDetails validation errors
        errors.Count(x => x.Identifier == nameof(GameDetails)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(GameDetails) && x.ErrorCode == $"{nameof(GameDetails)}.Required");

        // GameStatus validation errors
        errors.Count(x => x.Identifier == nameof(DomainGame.GameStatus)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.GameStatus) && x.ErrorCode == $"{nameof(DomainGame.GameStatus)}.ValidGameStatus");

        // OfficialLink validation error
        errors.Count(x => x.Identifier == nameof(DomainGame.OfficialLink)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.OfficialLink) && x.ErrorCode == $"{nameof(DomainGame.OfficialLink)}.ValidUrl");

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
            builder.OfficialLink = "Invalid_URL";
            builder.GameStatus = "Invalid";
        });

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
        var result = DomainGame.Create(builder =>
        {
            builder.Name = string.Empty; //Test with empty name
            builder.ReleaseDate = DateOnly.MinValue; //Test with default date
            builder.AgeRating = string.Empty; //Test with empty age rating
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (string.Empty, _faker.Company.CompanyName()); //Test with empty developer name
            builder.DiskSize = 0; //Test with zero disk size
            builder.Price = -5; //Test with negative price
            builder.Playtime = (-1, 0); //Test with invalid playtime
            builder.GameDetails = (
                Genre: _faker.PickRandom(_genres),
                Platform: [], //Test with empty array platform
                Tags: _faker.PickRandom(_gameTags),
                GameMode: string.Empty, //Test with empty game mode
                DistributionFormat: string.Empty, //Test with empty distribution format
                AvailableLanguages: _faker.PickRandom(_languages),
                SupportsDlcs: _faker.Random.Bool());
            builder.SystemRequirements = (string.Empty, _faker.Lorem.Paragraph()); //Test with empty system requirements
            builder.Rating = 11; //Test with invalid range rating
            builder.OfficialLink = "Invalid_URL"; //Test with empty url
            builder.GameStatus = string.Empty; //Test with empty game status
        });

        var errors = result.ValidationErrors;
        int errorCount = 26;

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldNotBeNull().ShouldNotBeEmpty();
        errors.ShouldBeOfType<List<ValidationError>>();
        errors.Count().ShouldBe(errorCount);

        // Name validation errors
        errors.Count(x => x.Identifier == nameof(DomainGame.Name)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.Name) && x.ErrorCode == $"{nameof(DomainGame.Name)}.Required");

        //ReleaseDate validation errors
        errors.Count(x => x.Identifier == nameof(DomainGame.ReleaseDate)).ShouldBe(2);
        errorCount -= 2;
        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x => x.Identifier == nameof(DomainGame.ReleaseDate) && x.ErrorCode == $"{nameof(DomainGame.ReleaseDate)}.Required").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(DomainGame.ReleaseDate) && x.ErrorCode == $"{nameof(DomainGame.ReleaseDate)}.ValidDate").ShouldBeTrue();
        });

        //AgeRating validation errors
        errors.Count(x => x.Identifier == nameof(DomainGame.AgeRating)).ShouldBe(3);
        errors.Count(x => x.Identifier == nameof(DomainGame.AgeRating) && x.ErrorCode == $"{nameof(AgeRating)}.Required").ShouldBe(2);
        errorCount -= 3;
        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x => x.Identifier == nameof(DomainGame.AgeRating) && x.ErrorCode == $"{nameof(AgeRating)}.Required").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(DomainGame.AgeRating) && x.ErrorCode == $"{nameof(AgeRating)}.ValidRating").ShouldBeTrue();
        });

        //Developer validation errors
        errors.Count(x => x.Identifier == nameof(DeveloperInfo.Developer)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DeveloperInfo.Developer) && x.ErrorCode == $"{nameof(DeveloperInfo.Developer)}.Required");

        //DeveloperInfo validation errors
        errors.Count(x => x.Identifier == nameof(DeveloperInfo)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DeveloperInfo) && x.ErrorCode == $"{nameof(DeveloperInfo)}.Required");

        //GameDetails validation errors
        errors.Count(x => x.Identifier == nameof(GameDetails)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(GameDetails) && x.ErrorCode == $"{nameof(GameDetails)}.Required");

        //DiskSize validation errors
        errors.Count(x => x.Identifier == nameof(DomainGame.DiskSize)).ShouldBe(3);
        errors.Count(x => x.Identifier == nameof(DomainGame.DiskSize) && x.ErrorCode == $"{nameof(DomainGame.DiskSize)}.Required").ShouldBe(2);
        errorCount -= 3;
        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x => x.Identifier == nameof(DomainGame.DiskSize) && x.ErrorCode == $"{nameof(DomainGame.DiskSize)}.Required").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(DomainGame.DiskSize) && x.ErrorCode == $"{nameof(DomainGame.DiskSize)}.GreaterThanZero").ShouldBeTrue();
        });

        //Price validation errors
        errors.Count(x => x.Identifier == nameof(DomainGame.Price)).ShouldBe(2);
        errorCount -= 2;
        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x => x.Identifier == nameof(DomainGame.Price) && x.ErrorCode == $"{nameof(DomainGame.Price)}.Required").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(DomainGame.Price) && x.ErrorCode == $"{nameof(DomainGame.Price)}.GreaterThanOrEqualToZero").ShouldBeTrue();
        });

        //Platform validation errors
        errors.Count(x => x.Identifier == nameof(GameDetails.Platform)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(errors => errors.Identifier == nameof(GameDetails.Platform) && errors.ErrorCode == $"{nameof(GameDetails.Platform)}.Required");

        //GameMode validation errors
        errors.Count(x => x.Identifier == nameof(GameDetails.GameMode)).ShouldBe(2);
        errorCount -= 2;
        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x => x.Identifier == nameof(GameDetails.GameMode) && x.ErrorCode == $"{nameof(GameDetails.GameMode)}.Required").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(GameDetails.GameMode) && x.ErrorCode == $"{nameof(GameDetails.GameMode)}.ValidGameMode").ShouldBeTrue();
        });

        //DistributionFormat validation errors
        errors.Count(x => x.Identifier == nameof(GameDetails.DistributionFormat)).ShouldBe(2);
        errorCount -= 2;
        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x => x.Identifier == nameof(GameDetails.DistributionFormat) && x.ErrorCode == $"{nameof(GameDetails.DistributionFormat)}.Required").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(GameDetails.DistributionFormat) && x.ErrorCode == $"{nameof(GameDetails.DistributionFormat)}.ValidDistributionFormat").ShouldBeTrue();
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

        //SystemRequirements validation errors
        errors.Count(x => x.Identifier == nameof(SystemRequirements)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(SystemRequirements) && x.ErrorCode == $"{nameof(SystemRequirements)}.Required");

        //Rating validation erros
        errors.Count(x => x.Identifier == nameof(DomainGame.Rating)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.Rating) && x.ErrorCode == $"{nameof(DomainGame.Rating)}.LessThanOrEqualToTen");

        //GameStatus validation errors
        errors.Count(x => x.Identifier == nameof(DomainGame.GameStatus)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.GameStatus) && x.ErrorCode == $"{nameof(DomainGame.GameStatus)}.ValidGameStatus");

        //OfficialLink validation errors
        errors.Count(x => x.Identifier == nameof(DomainGame.OfficialLink)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.OfficialLink) && x.ErrorCode == $"{nameof(DomainGame.OfficialLink)}.ValidUrl");

        //If counter is 0, it means that all errors were found
        errorCount.ShouldBe(0);
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
        var playtime = Playtime.Create(builder => { builder.Hours = -1; builder.PlayerCount = -1; });
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
        var playtime = Playtime.Create(builder => { builder.Hours = -1; builder.PlayerCount = -1; });
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
    public void Create_Game_Should_Return_Invalid_When_Price_Is_Negative()
    {
        // Arrange - Act
        var result = DomainGame.Create(builder =>
        {
            builder.Name = _faker.Commerce.ProductName();
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = _faker.Lorem.Paragraph();
            builder.DeveloperInfo = (_faker.Company.CompanyName(), _faker.Company.CompanyName());
            builder.DiskSize = (decimal)10.5;
            builder.Price = -50;
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
        errors.Count(x => x.Identifier == nameof(DomainGame.Price)).ShouldBe(2);
        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x => x.Identifier == nameof(DomainGame.Price) && x.ErrorCode == $"{nameof(DomainGame.Price)}.Required").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(DomainGame.Price) && x.ErrorCode == $"{nameof(DomainGame.Price)}.GreaterThanOrEqualToZero").ShouldBeTrue();
        });
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
            errors.Any(x => x.Identifier == nameof(DomainGame.DiskSize) && x.ErrorCode == $"{nameof(DomainGame.DiskSize)}.Required").ShouldBeTrue();
            errors.Any(x => x.Identifier == nameof(DomainGame.DiskSize) && x.ErrorCode == $"{nameof(DomainGame.DiskSize)}.GreaterThanZero").ShouldBeTrue();
        });
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_String_Fields_Exceeds_Max_Length()
    {
        // Arrange
        var name = new string('A', 201); // Max 200
        var releaseDate = DateOnly.FromDateTime(DateTime.Now);
        var ageRating = _faker.PickRandom(_ageRatings.ToArray());
        var description = new string('A', 2001); // Max 2000

        // Act
        var result = DomainGame.Create(builder =>
        {
            builder.Name = name;
            builder.ReleaseDate = releaseDate;
            builder.AgeRating = ageRating;
            builder.Description = description;
            builder.DeveloperInfo = (new string('D', 5001), new string('D', 5001)); // Max 5000 each
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (10, 2);
            builder.GameDetails = (
                Genre: new string('D', 501), // Max 500
                Platform: _faker.PickRandom(_platforms, 3).ToArray(),
                Tags: new string('D', 501), // Max 500
                GameMode: _faker.PickRandom(_gameModes),
                DistributionFormat: _faker.PickRandom(_distributionFormats),
                AvailableLanguages: new string('A', 201), // Max 200
                SupportsDlcs: true
            );
            builder.SystemRequirements = (new string('M', 2001), new string('R', 2001)); // Max 2000 each
            builder.Rating = 5;
            builder.OfficialLink = $"https://{new string('A', 201)}.com";
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;
        int errorCount = 13;

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

        // DeveloperInfo max length
        errors.Count(x => x.Identifier == nameof(DeveloperInfo)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DeveloperInfo) && x.ErrorCode == $"{nameof(DeveloperInfo)}.Required");

        // DeveloperInfo.Developer max length
        errors.Count(x => x.Identifier == nameof(DeveloperInfo.Developer)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DeveloperInfo.Developer) && x.ErrorCode == $"{nameof(DeveloperInfo.Developer)}.MaximumLength");

        // DeveloperInfo.Publisher max length
        errors.Count(x => x.Identifier == nameof(DeveloperInfo.Publisher)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DeveloperInfo.Publisher) && x.ErrorCode == $"{nameof(DeveloperInfo.Publisher)}.MaximumLength");

        // GameDetails max length
        errors.Count(x => x.Identifier == nameof(GameDetails)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(GameDetails) && x.ErrorCode == $"{nameof(GameDetails)}.Required");

        // GameDetails.Genre max length
        errors.Count(x => x.Identifier == nameof(GameDetails.Genre)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(GameDetails.Genre) && x.ErrorCode == $"{nameof(GameDetails.Genre)}.MaximumLength");

        // GameDetails.Tags max length
        errors.Count(x => x.Identifier == nameof(GameDetails.Tags)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(GameDetails.Tags) && x.ErrorCode == $"{nameof(GameDetails.Tags)}.MaximumLength");

        // GameDetails.AvailableLanguages max length
        errors.Count(x => x.Identifier == nameof(GameDetails.AvailableLanguages)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(GameDetails.AvailableLanguages) && x.ErrorCode == $"{nameof(GameDetails.AvailableLanguages)}.MaximumLength");

        // SystemRequirements max length
        errors.Count(x => x.Identifier == nameof(SystemRequirements)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(SystemRequirements) && x.ErrorCode == $"{nameof(SystemRequirements)}.Required");

        // SystemRequirements.Minimum max length
        errors.Count(x => x.Identifier == nameof(SystemRequirements.Minimum)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(SystemRequirements.Minimum) && x.ErrorCode == $"{nameof(SystemRequirements.Minimum)}.MaximumLength");

        // SystemRequirements.Recommended max length
        errors.Count(x => x.Identifier == nameof(SystemRequirements.Recommended)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(SystemRequirements.Recommended) && x.ErrorCode == $"{nameof(SystemRequirements.Recommended)}.MaximumLength");

        // OfficialLink max length
        errors.Count(x => x.Identifier == nameof(DomainGame.OfficialLink)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.OfficialLink) && x.ErrorCode == $"{nameof(DomainGame.OfficialLink)}.MaximumLength");

        // If counter is 0, it means that all errors were found
        errorCount.ShouldBe(0);
    }

    private static IEnumerable<(string Identifier, int Count, IEnumerable<string> ErrorCodes)> GroupValidationErrorsByIdentifier(IEnumerable<ValidationError> errors)
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