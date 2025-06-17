using TC.CloudGames.Application.Games.GetGameList;
using TC.CloudGames.Domain.Aggregates.Game;
using TC.CloudGames.Domain.Aggregates.Game.ValueObjects;
using TC.CloudGames.Unit.Tests.Fakes;
using DomainGameDetails = TC.CloudGames.Domain.Aggregates.Game.ValueObjects.GameDetails;

namespace TC.CloudGames.Unit.Tests.Application.Games;

public class GetGameListTests
{
    [Fact]
    public async Task GetGameListTests_ShouldReturnGameList_WhenGameExists()
    {
        // Arrange
        Factory.RegisterTestServices(_ => { });

        var gameValid = FakeGameData.GameValid();
        
        var getGameReq = new GetGameListQuery(PageNumber: 1, PageSize: 20, SortBy: "FirstName", SortDirection: "ASC", Filter: "");

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
                        recommended: gameValid.SystemRecommendRequirements
                    ),
                Rating = gameValid.Metacritic,
                OfficialLink = gameValid.URL,
                GameStatus = gameValid.GameStatus
            });
        }

        var fakeHandler = A.Fake<QueryHandler<GetGameListQuery, IReadOnlyList<GameListResponse>>>();
        A.CallTo(() => fakeHandler.ExecuteAsync(A<GetGameListQuery>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(Result<IReadOnlyList<GameListResponse>>.Success(expectedGame)));

        // Act
        var result = await fakeHandler.ExecuteAsync(getGameReq, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(expectedGame.Count, result.Value.Count);
        for (int i = 0; i < expectedGame.Count; i++)
        {
            Assert.Equal(expectedGame[i].Id, result.Value[i].Id);
            Assert.Equal(expectedGame[i].Name, result.Value[i].Name);
            Assert.Equal(expectedGame[i].AgeRating, result.Value[i].AgeRating);
            Assert.Equal(expectedGame[i].GameStatus, result.Value[i].GameStatus);
        }
    }
}