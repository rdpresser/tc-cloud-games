using FakeItEasy;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Application.Games.GetGameById;

namespace TC.CloudGames.Application.Tests.Games;

public class GetGameByIdTests
{
    [Fact]
    public async Task GetGameByIdQuery_When_Game_Exists_Should_Return_Game()
    {
        // Arrange
        var fakeRepository = A.Fake<IGamePgRepository>();
        var gameId = Guid.NewGuid();
        var expectedGame = new GameByIdResponse { Id = gameId };
        
        A.CallTo(() => fakeRepository.GetByIdAsync(gameId, CancellationToken.None))
            .Returns(expectedGame);

        var handler = new GetGameByIdQueryHandler(fakeRepository);
        
        var query = new GetGameByIdQuery(gameId);

        // Act
        var result = await handler.ExecuteAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedGame.Id, result.Value.Id);

        A.CallTo(() => fakeRepository.GetByIdAsync(gameId, CancellationToken.None))
            .MustHaveHappenedOnceExactly();
    }
}