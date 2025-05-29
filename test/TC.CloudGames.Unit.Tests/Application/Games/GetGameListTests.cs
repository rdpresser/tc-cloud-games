using TC.CloudGames.Application.Games.GetGameList;
using DomainGameDetails = TC.CloudGames.Domain.GameAggregate.ValueObjects.GameDetails;

namespace TC.CloudGames.Application.Tests.Games;

public class GetGameListTests
{
    private readonly Faker _faker;

    public GetGameListTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public async Task GetGameListTests_ShouldReturnGameList_WhenGameExists()
    {
        // Arrange
        Factory.RegisterTestServices(_ => { });
        string[] AvailableLanguagesList = ["English", "Spanish", "French", "German", "Japanese"];

        var getGameReq = new GetGameListQuery(PageNumber: 1, PageSize: 20, SortBy: "FirstName", SortDirection: "ASC", Filter: "");

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