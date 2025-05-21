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
    public void Create_Game_Should_Return_Invalid_When_Fields_Have_Invalid_Values()
    {
        // Arrange - Act
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
                genre: _faker.Lorem.Word(),
                platform: new[] { "Invalid" },
                tags: _faker.Lorem.Word(),
                gameMode: "Invalid",
                distributionFormat: "Invalid",
                availableLanguages: "EN-US",
                supportsDlcs: _faker.Random.Bool());
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = -1;
            builder.OfficialLink = "Invalid_URL";
            builder.GameStatus = "Invalid";
        });

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
                genre: _faker.PickRandom(_genres),
                platform: _faker.PickRandom(_platforms, 3).ToArray(),
                tags: _faker.PickRandom(_gameTags),
                gameMode: _faker.PickRandom(_gameModes),
                distributionFormat: _faker.PickRandom(_distributionFormats),
                availableLanguages: _faker.PickRandom(_languages),
                supportsDlcs: true);
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
                genre: _faker.PickRandom(_genres),
                platform: [], //Test with empty array platform
                tags: _faker.PickRandom(_gameTags),
                gameMode: string.Empty, //Test with empty game mode
                distributionFormat: string.Empty, //Test with empty distribution format
                availableLanguages: _faker.PickRandom(_languages),
                supportsDlcs: _faker.Random.Bool());
            builder.SystemRequirements = (string.Empty, _faker.Lorem.Paragraph()); //Test with empty system requirements
            builder.Rating = 11; //Test with zero rating
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = string.Empty; //Test with empty game status
        });

        var errors = result.ValidationErrors;
        int errorCount = 15;

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldNotBeNull().ShouldNotBeEmpty();
        errors.ShouldBeOfType<List<ValidationError>>();
        errors.Count().ShouldBe(errorCount);

        errors.Count(x => x.Identifier == nameof(DomainGame.Name)).ShouldBe(0);
        errors.Count(x => x.Identifier == nameof(DomainGame.ReleaseDate)).ShouldBe(0);

        //AgeRating validation errors
        errors.Count(x => x.Identifier == nameof(DomainGame.AgeRating)).ShouldBe(2);
        errorCount -= 2;
        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Any(x => x.Identifier == nameof(DomainGame.AgeRating) && x.ErrorCode == $"{nameof(AgeRating)}.Required").ShouldBeTrue();
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
            builder.GameDetails = DomainGameDetails.Create(builder =>
            {
                builder.Genre = _faker.PickRandom(_genres);
                builder.Platform = _faker.PickRandom(_platforms, 3).ToArray();
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
                genre: _faker.PickRandom(_genres),
                platform: _faker.PickRandom(_platforms, 3).ToArray(),
                tags: _faker.PickRandom(_gameTags),
                gameMode: _faker.PickRandom(_gameModes),
                distributionFormat: _faker.PickRandom(_distributionFormats),
                availableLanguages: _faker.PickRandom(_languages),
                supportsDlcs: true);
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
                genre: _faker.PickRandom(_genres),
                platform: _faker.PickRandom(_platforms, 3).ToArray(),
                tags: _faker.PickRandom(_gameTags),
                gameMode: _faker.PickRandom(_gameModes),
                distributionFormat: _faker.PickRandom(_distributionFormats),
                availableLanguages: _faker.PickRandom(_languages),
                supportsDlcs: true);
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
        errors.Count().ShouldBe(1);
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.Price));
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
                genre: _faker.PickRandom(_genres),
                platform: _faker.PickRandom(_platforms, 3).ToArray(),
                tags: _faker.PickRandom(_gameTags),
                gameMode: _faker.PickRandom(_gameModes),
                distributionFormat: _faker.PickRandom(_distributionFormats),
                availableLanguages: _faker.PickRandom(_languages),
                supportsDlcs: true);
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
                genre: _faker.PickRandom("RPG", "Ação"),
                platform: _faker.PickRandom(_platforms, 3).ToArray(),
                tags: "Indie",
                gameMode: _faker.PickRandom(_gameModes),
                distributionFormat: _faker.PickRandom(_distributionFormats),
                availableLanguages: "EN-US",
                supportsDlcs: true
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
        errors.Count().ShouldBe(1);
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.DiskSize));
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_String_Fields_Exceeds_Max_Length()
    {
        // Arrange - Act
        var result = DomainGame.Create(builder =>
        {
            builder.Name = new string('A', 201); // Max 200
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = _faker.PickRandom(_ageRatings.ToArray());
            builder.Description = new string('D', 2001); // Max 2000
            builder.DeveloperInfo = (new string('D', 5001), new string('D', 5001)); // Max 5000 each
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (10, 2);
            builder.GameDetails = (
                genre: new string('D', 501), // Max 500
                platform: _faker.PickRandom(_platforms, 3).ToArray(),
                tags: new string('D', 501), // Max 500
                gameMode: _faker.PickRandom(_gameModes),
                distributionFormat: _faker.PickRandom(_distributionFormats),
                availableLanguages: new string('A', 201), // Max 200
                supportsDlcs: true
            );
            builder.SystemRequirements = (new string('M', 2001), new string('R', 2001)); // Max 2000 each
            builder.Rating = 5;
            builder.OfficialLink = _faker.Internet.Url();
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

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
                genre: _faker.PickRandom(_genres),
                platform: _faker.PickRandom(_platforms, 3).ToArray(),
                tags: _faker.PickRandom(_gameTags),
                gameMode: _faker.PickRandom(_gameModes),
                distributionFormat: _faker.PickRandom(_distributionFormats),
                availableLanguages: _faker.PickRandom(_languages),
                supportsDlcs: true);
            builder.SystemRequirements = (_faker.Lorem.Paragraph(), _faker.Lorem.Paragraph());
            builder.Rating = null;
            builder.OfficialLink = $"https://{new string('A', 201)}.com";
            builder.GameStatus = _faker.PickRandom(_gameStatus);
        });

        var errors = result.ValidationErrors;
        int errorCount = 3;

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

        // OfficialLink max length
        errors.Count(x => x.Identifier == nameof(DomainGame.OfficialLink)).ShouldBe(1);
        errorCount -= 1;
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.OfficialLink) && x.ErrorCode == $"{nameof(DomainGame.OfficialLink)}.MaximumLength");

        // If counter is 0, it means that all errors were found
        errorCount.ShouldBe(0);
    }
}