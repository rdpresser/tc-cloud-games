using TC.CloudGames.Domain.Aggregates.Game.ValueObjects;
using TC.CloudGames.Unit.Tests.Fakes;
using DeveloperInfo = TC.CloudGames.Domain.Aggregates.Game.ValueObjects.DeveloperInfo;
using DomainGame = TC.CloudGames.Domain.Aggregates.Game.Game;
using GameDetails = TC.CloudGames.Domain.Aggregates.Game.ValueObjects.GameDetails;
using Playtime = TC.CloudGames.Domain.Aggregates.Game.ValueObjects.Playtime;
using Price = TC.CloudGames.Domain.Aggregates.Game.ValueObjects.Price;
using SystemRequirements = TC.CloudGames.Domain.Aggregates.Game.ValueObjects.SystemRequirements;

namespace TC.CloudGames.Unit.Tests.Domain.Game;

public class GameTests
{
    [Fact]
    public void Create_Game_Should_Return_Invalid_When_AgeRating_Is_Invalid()
    {
        // Arrange
        var configure = new[] { "Invalid" };
        
        var gameValid = FakeGameData.GameValid();

        // Act
        var result = DomainGame.Create(builder =>
        {
            builder.Name = gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(gameValid.ReleaseDate);
            builder.AgeRating = "Invalid";
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = gameValid.DiskSize;
            builder.Price = gameValid.Price;
            builder.Playtime = (gameValid.PlayersTime, gameValid.PlayersCount);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: configure,
                Tags: gameValid.Tags,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: gameValid.Language,
                SupportsDlcs: true
                );
            builder.SystemRequirements = (gameValid.SystemMinimalRequirements, gameValid.SystemRecommendRequirements);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
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

        var gameValid = FakeGameData.GameValid();
        
        // Act
        var result = DomainGame.Create(builder =>
        {
            builder.Name = gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(gameValid.ReleaseDate);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = gameValid.DiskSize;
            builder.Price = gameValid.Price;
            builder.Playtime = (gameValid.PlayersTime, gameValid.PlayersCount);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: configure,
                Tags: gameValid.Genre,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: "EN-US",
                SupportsDlcs: true);
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
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
        var gameValid = FakeGameData.GameValid();
        var configure = gameValid.Platforms.ToArray();
        
        // Act
        var result = DomainGame.Create(builder =>
        {
            builder.Name =
                gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(gameValid.ReleaseDate);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = gameValid.DiskSize;
            builder.Price = gameValid.Price;
            builder.Playtime = (gameValid.PlayersTime, gameValid.PlayersCount);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: configure,
                Tags: gameValid.Genre,
                GameMode: "Invalid",
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: "EN-US",
                SupportsDlcs: true);
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
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
        var gameValid = FakeGameData.GameValid();
        var configure = gameValid.Platforms.ToArray();
        
        // Act
        var result = DomainGame.Create(builder =>
        {
            builder.Name =
                gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(gameValid.ReleaseDate);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = gameValid.DiskSize;
            builder.Price = gameValid.Price;
            builder.Playtime = (gameValid.PlayersTime, gameValid.PlayersCount);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: configure,
                Tags: gameValid.Genre,
                GameMode: gameValid.GameMode,
                DistributionFormat: "Invalid",
                AvailableLanguages: "EN-US",
                SupportsDlcs: true);
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
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
        var gameValid = FakeGameData.GameValid();

        var configure = gameValid.Platforms.ToArray();

        
        // Act
        var result = DomainGame.Create(builder =>
        {
            builder.Name =
                gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(gameValid.ReleaseDate);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = gameValid.DiskSize;
            builder.Price = gameValid.Price;
            builder.Playtime = (gameValid.PlayersTime, gameValid.PlayersCount);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: configure,
                Tags: gameValid.Genre,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: "EN-US",
                SupportsDlcs: true);
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = -1;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
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
        var gameValid = FakeGameData.GameValid();

        var configure = gameValid.Platforms.ToArray();

        
        // Act
        var result = DomainGame.Create(builder =>
        {
            builder.Name =
                gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(gameValid.ReleaseDate);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = gameValid.DiskSize;
            builder.Price = gameValid.Price;
            builder.Playtime = (gameValid.PlayersTime, gameValid.PlayersCount);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: configure,
                Tags: gameValid.Genre,
                GameMode: "Invalid",
                DistributionFormat: "Invalid",
                AvailableLanguages: "EN-US",
                SupportsDlcs: true);
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
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
        var gameValid = FakeGameData.GameValid();

        var configure = gameValid.Platforms.ToArray();

        
        // Act
        var result = DomainGame.Create(builder =>
        {
            builder.Name =
                gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(gameValid.ReleaseDate);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = gameValid.DiskSize;
            builder.Price = gameValid.Price;
            builder.Playtime = (gameValid.PlayersTime, gameValid.PlayersCount);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: configure,
                Tags: gameValid.Genre,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: "EN-US",
                SupportsDlcs: true);
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
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
        var gameValid = FakeGameData.GameValid();

        var configure = gameValid.Platforms.ToArray();
        
        
        // Act
        var result = DomainGame.Create(builder =>
        {
            builder.Name =
                gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(gameValid.ReleaseDate);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = gameValid.DiskSize;
            builder.Price = gameValid.Price;
            builder.Playtime = (gameValid.PlayersTime, gameValid.PlayersCount);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: configure,
                Tags: gameValid.Genre,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: "EN-US",
                SupportsDlcs: true);
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = "Invalid_URL";
            builder.GameStatus = gameValid.GameStatus;
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
        var gameValid = FakeGameData.GameValid();
        
        var result = DomainGame.Create(builder =>
        {
            builder.Name = string.Empty;
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (1, 1);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: gameValid.Platforms.ToArray(),
                Tags: gameValid.Tags,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: gameValid.Language,
                SupportsDlcs: true);
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldContain(x =>
            x.Identifier == nameof(DomainGame.Name) && x.ErrorCode == $"{nameof(DomainGame.Name)}.Required");
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_ReleaseDate_Is_Default()
    {
        var gameValid = FakeGameData.GameValid();
        
        var result = DomainGame.Create(builder =>
        {
            builder.Name = gameValid.Name;
            builder.ReleaseDate = DateOnly.MinValue;
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (1, 1);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: gameValid.Platforms.ToArray(),
                Tags: gameValid.Tags,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: gameValid.Language,
                SupportsDlcs: true);
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
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
        var gameValid = FakeGameData.GameValid();
        
        var result = DomainGame.Create(builder =>
        {
            builder.Name = gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = string.Empty;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (1, 1);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: gameValid.Platforms.ToArray(),
                Tags: gameValid.Tags,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: gameValid.Language,
                SupportsDlcs: true);
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.Count(x => x.Identifier == nameof(DomainGame.AgeRating)).ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Developer_Is_Empty()
    {
        var gameValid = FakeGameData.GameValid();
        
        var result = DomainGame.Create(builder =>
        {
            builder.Name = gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (string.Empty, gameValid.PublisherName);
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (1, 1);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: gameValid.Platforms.ToArray(),
                Tags: gameValid.Tags,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: gameValid.Language,
                SupportsDlcs: true);
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
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
        var gameValid = FakeGameData.GameValid();
        
        var result = DomainGame.Create(builder =>
        {
            builder.Name = gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = 0;
            builder.Price = 50;
            builder.Playtime = (1, 1);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: gameValid.Platforms.ToArray(),
                Tags: gameValid.Tags,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: gameValid.Language,
                SupportsDlcs: true);
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.Count(x => x.Identifier == nameof(DomainGame.DiskSize)).ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Price_Is_Negative()
    {
        var gameValid = FakeGameData.GameValid();
        
        var result = DomainGame.Create(builder =>
        {
            builder.Name = gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = 10;
            builder.Price = -5;
            builder.Playtime = (1, 1);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: gameValid.Platforms.ToArray(),
                Tags: gameValid.Tags,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: gameValid.Language,
                SupportsDlcs: true);
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.Count(x => x.Identifier == nameof(DomainGame.Price)).ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Playtime_Is_Invalid()
    {
        var gameValid = FakeGameData.GameValid();
        
        var result = DomainGame.Create(builder =>
        {
            builder.Name = gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (-1, 0);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: gameValid.Platforms.ToArray(),
                Tags: gameValid.Tags,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: gameValid.Language,
                SupportsDlcs: true);
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldContain(x => x.Identifier == nameof(Playtime.Hours));
        errors.ShouldContain(x => x.Identifier == nameof(Playtime.PlayerCount));
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_GameDetails_Are_Empty()
    {
        var gameValid = FakeGameData.GameValid();
        
        var result = DomainGame.Create(builder =>
        {
            builder.Name = gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (1, 1);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: [],
                Tags: gameValid.Tags,
                GameMode: string.Empty,
                DistributionFormat: string.Empty,
                AvailableLanguages: gameValid.Language,
                SupportsDlcs: true);
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
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
        var gameValid = FakeGameData.GameValid();
        
        var result = DomainGame.Create(builder =>
        {
            builder.Name = gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (1, 1);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: gameValid.Platforms.ToArray(),
                Tags: gameValid.Tags,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: gameValid.Language,
                SupportsDlcs: true);
            builder.SystemRequirements = (string.Empty, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldContain(x => x.Identifier == nameof(SystemRequirements.Minimum));
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Rating_Is_Out_Of_Range()
    {
        var gameValid = FakeGameData.GameValid();
        
        var result = DomainGame.Create(builder =>
        {
            builder.Name = gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (1, 1);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: gameValid.Platforms.ToArray(),
                Tags: gameValid.Tags,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: gameValid.Language,
                SupportsDlcs: true);
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 11;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.Rating));
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_GameStatus_Is_Empty()
    {
        var gameValid = FakeGameData.GameValid();
        
        var result = DomainGame.Create(builder =>
        {
            builder.Name = gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (1, 1);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: gameValid.Platforms.ToArray(),
                Tags: gameValid.Tags,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: gameValid.Language,
                SupportsDlcs: true);
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = string.Empty;
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldContain(x => x.Identifier == nameof(DomainGame.GameStatus));
    }


    [Fact]
    public void Create_Game_Using_Builder_Should_Return_Success_When_All_Fields_Are_Valid()
    {
        var gameValid = FakeGameData.GameValid();
        
        // Arrange
        var name = gameValid.Name;
        var releaseDate = DateOnly.FromDateTime(DateTime.Now);
        var ageRating = AgeRating.Create(builder => builder.Value = gameValid.AgeRating);
        var description = gameValid.Description;
        var developerInfo = DeveloperInfo.Create(builder =>
        {
            builder.Developer = gameValid.PublisherName;
            builder.Publisher = gameValid.PublisherName;
        });
        var diskSize = DiskSize.Create(builder => builder.SizeInGb = gameValid.DiskSize);
        var price = Price.Create(builder => builder.Amount = gameValid.Price);
        var playtime = Playtime.Create(builder =>
        {
            builder.Hours = gameValid.PlayersTime;
            builder.PlayerCount = gameValid.PlayersCount;
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
                builder.Genre = gameValid.Genre;
                builder.Platform = gameValid.Platforms.ToArray();
                builder.Tags = gameValid.Tags;
                builder.GameMode = gameValid.GameMode;
                builder.DistributionFormat = gameValid.DistributionFormat;
                builder.AvailableLanguages = gameValid.Language;
                builder.SupportsDlcs = true;
            });
            builder.SystemRequirements = SystemRequirements.Create(builder =>
            {
                builder.Minimum = gameValid.Description;
                builder.Recommended = gameValid.Description;
            });
            builder.Rating = Rating.Create(builder => builder.Average = null);
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
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
        var gameValid = FakeGameData.GameValid();
        
        // Arrange
        var name = gameValid.Name;
        var releaseDate = DateOnly.FromDateTime(DateTime.Now);
        var ageRating = AgeRating.Create(builder => { });
        var description = gameValid.Description;
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
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
        });

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_Game_Using_Builder_Result_Should_Return_Failure_When_Value_Objects_Are_Invalid()
    {
        var gameValid = FakeGameData.GameValid();
        
        // Arrange
        var name = gameValid.Name;
        var releaseDate = DateOnly.FromDateTime(DateTime.Now);
        var ageRating = AgeRating.Create(builder => { });
        var description = gameValid.Description;
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
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
        });

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_Game_Using_Builder_Result_Should_Return_Success_When_All_Fields_Are_Valid()
    {
        var gameValid = FakeGameData.GameValid();
        
        // Arrange
        var name = gameValid.Name;
        var releaseDate = DateOnly.FromDateTime(DateTime.Now);
        var ageRating = AgeRating.Create(builder => builder.Value = gameValid.AgeRating);
        var description = gameValid.Description;
        var developerInfo = DeveloperInfo.Create(builder =>
        {
            builder.Developer = gameValid.PublisherName;
            builder.Publisher = gameValid.PublisherName;
        });
        var diskSize = DiskSize.Create(builder => builder.SizeInGb = gameValid.DiskSize);
        var price = Price.Create(builder => builder.Amount = gameValid.Price);
        var playtime = Playtime.Create(builder =>
        {
            builder.Hours = gameValid.PlayersTime;
            builder.PlayerCount = gameValid.PlayersCount;
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
                builder.Genre = gameValid.Genre;
                builder.Platform = gameValid.Platforms.ToArray();
                builder.Tags = gameValid.Tags;
                builder.GameMode = gameValid.GameMode;
                builder.DistributionFormat = gameValid.DistributionFormat;
                builder.AvailableLanguages = gameValid.Language;
                builder.SupportsDlcs = true;
            });
            builder.SystemRequirements = SystemRequirements.Create(builder =>
            {
                builder.Minimum = gameValid.Description;
                builder.Recommended = gameValid.Description;
            });
            builder.Rating = Rating.Create(builder => builder.Average = null);
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
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
        var gameValid = FakeGameData.GameValid();
        
        // Arrange
        var name = gameValid.Name;
        var releaseDate = DateOnly.FromDateTime(DateTime.Now);
        var ageRating = gameValid.AgeRating;
        var description = gameValid.Description;
        var developerInfo = (gameValid.DeveloperName, gameValid.PublisherName);
        var diskSize = gameValid.DiskSize;
        var price = gameValid.Price;
        var playtime = (gameValid.PlayersTime, gameValid.PlayersCount);

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
                Genre: gameValid.Genre,
                Platform: gameValid.Platforms.ToArray(),
                Tags: gameValid.Tags,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: gameValid.Language,
                SupportsDlcs: true);
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = null;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
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
        var gameValid = FakeGameData.GameValid();

        var result = DomainGame.Create(builder =>
        {
            builder.Name = gameValid.Name;
            builder.ReleaseDate = DateOnly.MinValue;
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = (decimal)10.5;
            builder.Price = 50;
            builder.Playtime = (gameValid.PlayersTime, gameValid.PlayersCount);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: gameValid.Platforms.ToArray(),
                Tags: gameValid.Tags,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: gameValid.Language,
                SupportsDlcs: true);
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = null;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
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
        var gameValid = FakeGameData.GameValid();
        
        var result = DomainGame.Create(builder =>
        {
            builder.Name = gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = -1;
            builder.Price = 50;
            builder.Playtime = (10, 2);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: gameValid.Platforms.ToArray(),
                Tags: "Indie",
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: "EN-US",
                SupportsDlcs: true
            );
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
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
        var gameValid = FakeGameData.GameValid();
        
        var name = new string('A', 201); // Max 200
        var result = DomainGame.Create(builder =>
        {
            builder.Name = name;
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (10, 2);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: gameValid.Platforms.ToArray(),
                Tags: gameValid.Tags,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: gameValid.Language,
                SupportsDlcs: true
            );
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
        });

        var errors = result.ValidationErrors;
        result.Status.ShouldBe(ResultStatus.Invalid);
        errors.ShouldContain(x =>
            x.Identifier == nameof(DomainGame.Name) && x.ErrorCode == $"{nameof(DomainGame.Name)}.MaximumLength");
    }

    [Fact]
    public void Create_Game_Should_Return_Invalid_When_Description_Exceeds_Max_Length()
    {
        var gameValid = FakeGameData.GameValid();

        var description = new string('A', 2001); // Max 2000
        var result = DomainGame.Create(builder =>
        {
            builder.Name = gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (10, 2);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: gameValid.Platforms.ToArray(),
                Tags: gameValid.Tags,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: gameValid.Language,
                SupportsDlcs: true
            );
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
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
        var gameValid = FakeGameData.GameValid();

        var dev = new string('D', 5001); // Max 5000
        var result = DomainGame.Create(builder =>
        {
            builder.Name = gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (dev, dev);
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (10, 2);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: gameValid.Platforms.ToArray(),
                Tags: gameValid.Tags,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: gameValid.Language,
                SupportsDlcs: true
            );
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
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
        var gameValid = FakeGameData.GameValid();
        
        var genre = new string('D', 501); // Max 500
        var tags = new string('D', 501); // Max 500
        var availableLanguages = new string('A', 201); // Max 200
        var result = DomainGame.Create(builder =>
        {
            builder.Name = gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (10, 2);
            builder.GameDetails = (
                Genre: genre,
                Platform: gameValid.Platforms.ToArray(),
                Tags: tags,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: availableLanguages,
                SupportsDlcs: true
            );
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
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
        var gameValid = FakeGameData.GameValid();
        
        var min = new string('M', 2001); // Max 2000
        var rec = new string('R', 2001); // Max 2000
        var result = DomainGame.Create(builder =>
        {
            builder.Name = gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (10, 2);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: gameValid.Platforms.ToArray(),
                Tags: gameValid.Tags,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: gameValid.Language,
                SupportsDlcs: true
            );
            builder.SystemRequirements = (min, rec);
            builder.Rating = 5;
            builder.OfficialLink = gameValid.URL;
            builder.GameStatus = gameValid.GameStatus;
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
        var gameValid = FakeGameData.GameValid();

        var officialLink = $"https://{new string('A', 201)}.com"; // Exceeds max
        var result = DomainGame.Create(builder =>
        {
            builder.Name = gameValid.Name;
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            builder.AgeRating = gameValid.AgeRating;
            builder.Description = gameValid.Description;
            builder.DeveloperInfo = (gameValid.DeveloperName, gameValid.PublisherName);
            builder.DiskSize = 10;
            builder.Price = 50;
            builder.Playtime = (10, 2);
            builder.GameDetails = (
                Genre: gameValid.Genre,
                Platform: gameValid.Platforms.ToArray(),
                Tags: gameValid.Tags,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: gameValid.Language,
                SupportsDlcs: true
            );
            builder.SystemRequirements = (gameValid.Description, gameValid.Description);
            builder.Rating = 5;
            builder.OfficialLink = officialLink;
            builder.GameStatus = gameValid.GameStatus;
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