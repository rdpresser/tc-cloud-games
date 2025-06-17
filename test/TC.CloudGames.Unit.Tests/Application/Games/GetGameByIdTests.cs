using TC.CloudGames.Application.Games.GetGameById;
using TC.CloudGames.Domain.Aggregates.Game.ValueObjects;
using TC.CloudGames.Unit.Tests.Fakes;
using DeveloperInfo = TC.CloudGames.Application.Games.GetGameById.DeveloperInfo;
using DomainGameDetails = TC.CloudGames.Domain.Aggregates.Game.ValueObjects.GameDetails;
using Playtime = TC.CloudGames.Application.Games.GetGameById.Playtime;
using Price = TC.CloudGames.Application.Games.GetGameById.Price;

namespace TC.CloudGames.Unit.Tests.Application.Games;

public class GetGameByIdTests
{

    [Fact]
    public async Task GetGameById_ShouldReturnGame_WhenGameExists()
    {
        // Arrange
        Factory.RegisterTestServices(_ => { });

        string[] AvailableLanguagesList = ["English", "Spanish", "French", "German", "Japanese"];

        var gameValid = FakeGameData.GameValid();
        
        var name = gameValid.Name;
        var releaseDate = DateOnly.FromDateTime(gameValid.ReleaseDate);
        var ageRating = gameValid.AgeRating;
        var description = gameValid.Description;
        var developerInfo = new DeveloperInfo(gameValid.DeveloperName, gameValid.PublisherName);
        var diskSize = gameValid.DiskSize;
        var price = gameValid.Price;
        var playtime = new Playtime(gameValid.PlayersTime, gameValid.PlayersCount);

        var gameId = Guid.NewGuid();
        var getGameReq = new GetGameByIdQuery(Id: gameId);
        var expectedGame = GameByIdResponse.Create(builder =>
        {
            builder.Id = gameId;
            builder.Name = name;
            builder.ReleaseDate = releaseDate;
            builder.AgeRating = ageRating;
            builder.Description = description;
            builder.DiskSize = diskSize;
            builder.Price = price;
            builder.Rating = gameValid.Metacritic;
            builder.OfficialLink = gameValid.URL;
            builder.DeveloperInfo = developerInfo;
            builder.GameDetails = new(
                genre: gameValid.Genre,
                platform: gameValid.Platforms.ToArray(),
                tags: gameValid.Tags,
                gameMode: gameValid.GameMode,
                distributionFormat: gameValid.DistributionFormat,
                availableLanguages: gameValid.Language,
                supportsDlcs: true
            );
            builder.SystemRequirements = new(
                minimum: gameValid.SystemMinimalRequirements,
                recommended: gameValid.SystemRecommendRequirements);
            builder.Playtime = new(null, null);
            builder.GameStatus = "Available";
            builder.OfficialLink = gameValid.URL;
        });

        var fakeHandler = A.Fake<QueryHandler<GetGameByIdQuery, GameByIdResponse>>();
        A.CallTo(() => fakeHandler.ExecuteAsync(A<GetGameByIdQuery>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(Result<GameByIdResponse>.Success(expectedGame)));

        // Act
        var result = await fakeHandler.ExecuteAsync(getGameReq, CancellationToken.None);

        // Assert
        A.CallTo(() => fakeHandler.ExecuteAsync(
            A<GetGameByIdQuery>.That.Matches(q => q.Id == gameId),
            A<CancellationToken>.Ignored
        )).MustHaveHappenedOnceExactly();

        Assert.NotNull(result);
        Assert.Equal(expectedGame.Id, result.Value.Id);
    }

    [Fact]
    public void Constructor_SetsAmount()
    {
        // Arrange
        decimal expectedAmount = 59.99m;

        // Act
        var price = new Price(expectedAmount);

        // Assert
        Assert.Equal(expectedAmount, price.Amount);
    }

    [Fact]
    public void Amount_CanBeZero()
    {
        // Arrange & Act
        var price = new Price(0m);

        // Assert
        Assert.Equal(0m, price.Amount);
    }

    [Fact]
    public void Amount_CanBeNegative()
    {
        // Arrange & Act
        var price = new Price(-10m);

        // Assert
        Assert.Equal(-10m, price.Amount);
    }
}