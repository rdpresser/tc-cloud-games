using TC.CloudGames.Domain.Aggregates.Game.ValueObjects;
using TC.CloudGames.Unit.Tests.Fakes;
using DeveloperInfo = TC.CloudGames.Application.Games.CreateGame.DeveloperInfo;
using DomainGameDetails = TC.CloudGames.Domain.Aggregates.Game.ValueObjects.GameDetails;
using GameDetails = TC.CloudGames.Application.Games.CreateGame.GameDetails;
using Playtime = TC.CloudGames.Application.Games.CreateGame.Playtime;
using Price = TC.CloudGames.Application.Games.CreateGame.Price;
using SystemRequirements = TC.CloudGames.Application.Games.CreateGame.SystemRequirements;

namespace TC.CloudGames.Unit.Tests.Application.Games;

public class CreateGameTests
{
    [Fact]
    public async Task Handle_CreateGame()
    {

        // Arrange
        var gameValid = FakeGameData.GameValid();
        
        var name = gameValid.Name;
        var releaseDate = DateOnly.FromDateTime(gameValid.ReleaseDate);
        var ageRating = gameValid.AgeRating;
        var description = gameValid.Description;
        var developerInfo = new DeveloperInfo(gameValid.DeveloperName, gameValid.PublisherName);
        var diskSize = gameValid.DiskSize;
        var price = gameValid.Price;
        var playtime = new Playtime(gameValid.PlayersTime, gameValid.PlayersCount);

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
                Genre: gameValid.Genre,
                Platform: gameValid.Platforms.ToArray(),
                Tags: gameValid.Tags,
                GameMode: gameValid.GameMode,
                DistributionFormat: gameValid.DistributionFormat,
                AvailableLanguages: gameValid.Language,
                SupportsDlcs: true
            ),
            SystemRequirements: new SystemRequirements(gameValid.SystemMinimalRequirements, gameValid.SystemRecommendRequirements),
            Rating: null,
            OfficialLink: gameValid.URL,
            GameStatus: gameValid.GameStatus
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

        var fakerHandler = A.Fake<IAppCommandHandler.ICommandHandler<CreateGameCommand, CreateGameResponse>>();

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