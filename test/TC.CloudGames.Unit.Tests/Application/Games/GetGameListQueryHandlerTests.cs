using TC.CloudGames.Application.Games.GetGameList;
using TC.CloudGames.Domain.Aggregates.Game;
using TC.CloudGames.Domain.Aggregates.Game.ValueObjects;
using TC.CloudGames.Unit.Tests.Fakes;
using DomainGameDetails = TC.CloudGames.Domain.Aggregates.Game.ValueObjects.GameDetails;

namespace TC.CloudGames.Unit.Tests.Application.Games
{
    public class GetGameListQueryHandlerTests
    {
        [Fact]
        public async Task ExecuteAsync_ReturnsEmptyList_WhenNoGamesFound()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            var gameRepositoryFake = A.Fake<IGamePgRepository>();
            var handler = new GetGameListQueryHandler(gameRepositoryFake);


            var query = new GetGameListQuery();
            A.CallTo(() => gameRepositoryFake.GetGameListAsync(query, A<CancellationToken>._))
                .Returns((IReadOnlyList<GameListResponse>?)null);

            // Act
            var result = await handler.ExecuteAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsGameList_WhenGamesFound()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });
            
            var gameValid = FakeGameData.GameValid();

            string[] AvailableLanguagesList = ["English", "Spanish", "French", "German", "Japanese"];

            var gameRepositoryFake = A.Fake<IGamePgRepository>();
            var handler = new GetGameListQueryHandler(gameRepositoryFake);

            var query = new GetGameListQuery();

            var expectedGame = new List<GameListResponse>();

            for (int i = 0; i < 5; i++)
            {
                expectedGame.Add(new()
                {
                    Id = Guid.NewGuid(),
                    Name = gameValid.Name,
                    ReleaseDate = DateOnly.FromDateTime(gameValid.ReleaseDate),
                    AgeRating = gameValid.AgeRating,
                    Description = gameValid.Description,
                    DeveloperInfo = new(
                            developer: gameValid.DeveloperName,
                            publisher: gameValid.PublisherName
                        ),
                    DiskSize = gameValid.DiskSize,
                    Price = gameValid.Price,
                    Playtime = new(
                            hours: gameValid.PlayersTime,
                            playerCount: gameValid.PlayersCount
                        ),
                    GameDetails = new(
                            genre: gameValid.Genre,
                            platform: gameValid.Platforms.ToArray(),
                            tags: gameValid.Tags,
                            gameMode: gameValid.GameMode,
                            distributionFormat: gameValid.DistributionFormat,
                            availableLanguages: gameValid.Language,
                            supportsDlcs: true
                        ),
                    SystemRequirements = new(
                            minimum: gameValid.SystemMinimalRequirements,
                            recommended: gameValid.SystemRecommendRequirements),
                    Rating = gameValid.Metacritic,
                    OfficialLink = gameValid.URL,
                    GameStatus = gameValid.GameStatus
                });
            }

            A.CallTo(() => gameRepositoryFake.GetGameListAsync(query, A<CancellationToken>._))
                .Returns(expectedGame);

            // Act
            var result = await handler.ExecuteAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(5, result.Value.Count);
        }
    }
}
