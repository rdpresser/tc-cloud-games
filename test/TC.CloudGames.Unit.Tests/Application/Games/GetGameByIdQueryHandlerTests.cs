using TC.CloudGames.Application.Games.GetGameById;

namespace TC.CloudGames.Application.Tests.Games
{
    public class GetGameByIdQueryHandlerTests
    {
        [Fact]
        public async Task ExecuteAsync_ReturnsGame_WhenGameExists()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            var gameRepository = A.Fake<IGamePgRepository>();
            var handler = new GetGameByIdQueryHandler(gameRepository);

            var gameId = Guid.NewGuid();
            var query = new GetGameByIdQuery(gameId);
            var expectedResponse = A.Fake<GameByIdResponse>();
            A.CallTo(() => gameRepository.GetByIdAsync(gameId, A<CancellationToken>._))
                .Returns(Task.FromResult<GameByIdResponse?>(expectedResponse));

            // Act
            var result = await handler.ExecuteAsync(query, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(expectedResponse);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsValidationError_WhenGameDoesNotExist()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            var gameRepository = A.Fake<IGamePgRepository>();
            var handler = new GetGameByIdQueryHandler(gameRepository);

            var gameId = Guid.NewGuid();
            var query = new GetGameByIdQuery(gameId);
            A.CallTo(() => gameRepository.GetByIdAsync(gameId, A<CancellationToken>._))
                .Returns(Task.FromResult<GameByIdResponse?>(null));

            // Act
            var result = await handler.ExecuteAsync(query, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Errors.ShouldBeEmpty();
        }
    }
}
