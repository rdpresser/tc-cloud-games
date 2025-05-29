using TC.CloudGames.Application.Games.GetGameList;
using DomainGameDetails = TC.CloudGames.Domain.GameAggregate.ValueObjects.GameDetails;

namespace TC.CloudGames.Application.Tests.Games
{
    public class GetGameListQueryHandlerTests
    {
        private readonly Faker _faker;

        public GetGameListQueryHandlerTests()
        {
            _faker = new Faker();
        }

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
                    Name = $"{_faker.Commerce.ProductAdjective()} {_faker.Commerce.ProductMaterial()} {_faker.Commerce.Product()}",
                    ReleaseDate = DateOnly.FromDateTime(_faker.Date.Past()),
                    AgeRating = _faker.PickRandom(AgeRating.ValidRatings.ToArray()),
                    Description = _faker.Lorem.Paragraph(),
                    DeveloperInfo = new(
                            developer: _faker.Company.CompanyName(),
                            publisher: _faker.Company.CompanyName()
                        ),
                    DiskSize = _faker.Random.Int(1, 150),
                    Price = decimal.Parse(_faker.Commerce.Price(1.0m, 100.0m, 2)),
                    Playtime = new(
                            hours: _faker.Random.Int(1, 100),
                            playerCount: _faker.Random.Int(1, 2000)
                        ),
                    GameDetails = new(
                            genre: _faker.Commerce.Categories(1)[0],
                            platform: [.. _faker.PickRandom(DomainGameDetails.ValidPlatforms, _faker.Random.Int(1, DomainGameDetails.ValidPlatforms.Count))],
                            tags: string.Join(", ", _faker.Lorem.Words(5)),
                            gameMode: _faker.PickRandom(DomainGameDetails.ValidGameModes.ToArray()),
                            distributionFormat: _faker.PickRandom(DomainGameDetails.ValidDistributionFormats.ToArray()),
                            availableLanguages: string.Join(", ", _faker.Random.ListItems(AvailableLanguagesList, _faker.Random.Int(1, AvailableLanguagesList.Length))),
                            supportsDlcs: _faker.Random.Bool()
                        ),
                    SystemRequirements = new(
                            minimum: _faker.Lorem.Sentence(),
                            recommended: _faker.Lorem.Sentence()
                        ),
                    Rating = Math.Round(_faker.Random.Decimal(1, 10), 2),
                    OfficialLink = _faker.Internet.Url(),
                    GameStatus = _faker.PickRandom(Game.ValidGameStatus.ToArray())
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
