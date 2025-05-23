using Ardalis.Result;
using FakeItEasy;
using FastEndpoints;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Application.Games.GetGameById;

namespace TC.CloudGames.Application.Tests.Games;

public class GetGameByIdTests
{
    [Fact]
    public async Task GetGameById_ShouldReturnGame_WhenGameExists()
    {
        // Arrange
        Factory.RegisterTestServices(_ => {});
        
        var gameId = Guid.NewGuid();
        var getGameReq = new GetGameByIdQuery(Id: gameId);
        var expectedGame = GameByIdResponse.Create(builder =>
        {
            builder.Id = gameId;
            builder.Name = "Test Game";
            builder.ReleaseDate = DateOnly.FromDateTime(DateTime.UtcNow);
            builder.AgeRating = "PG-13";
            builder.Description = "Test Description";
            builder.DiskSize = 50.0m;
            builder.Price = 29.99m;
            builder.Rating = 4.5m;
            builder.OfficialLink = "https://example.com/game";
            builder.DeveloperInfo = new("Test Developer", null);
            builder.GameDetails = new("Test Genre", ["PC"], null, "Test Platform", "Test Language", null, false);
            builder.SystemRequirements = new("Test OS", null);
            builder.Playtime = new(null, null);
            builder.GameStatus = "Available";
            builder.OfficialLink = "https://example.com/game";
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