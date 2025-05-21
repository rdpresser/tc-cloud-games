using Microsoft.AspNetCore.Http;
using TC.CloudGames.Api.Endpoints.Game;
using TC.CloudGames.Application.Abstractions;
using TC.CloudGames.Application.Games.GetGameList;
using TC.CloudGames.Domain.Game;

namespace TC.CloudGames.Api.Tests.Endpoints.Game
{
    public class GetGameListEndpointTests(App App) : TestBase<App>
    {
        [Fact]
        public async Task GetGameList_ValidFilters_ReturnsGameList()
        {
            // Arrange
            var httpContext = App.GetValidUserContextAccessor(AppConstants.UserRole);
            var userContext = App.GetValidLoggedUser(AppConstants.UserRole);
            var cache = App.GetCache();
            string[] AvailableLanguagesList = ["English", "Spanish", "French", "German", "Japanese"];

            var ep = Factory.Create<GetGameListEndpoint>((httpContext.HttpContext as DefaultHttpContext)!, cache, userContext);
            var getGameReq = new GetGameListQuery(PageNumber: 1, PageSize: 20, SortBy: "FirstName", SortDirection: "ASC", Filter: "");

            var getGameRes = new List<GameListResponse>();
            for (int i = 0; i < 5; i++)
            {
                getGameRes.Add(new()
                {
                    Id = Guid.NewGuid(),
                    Name = $"{App.Fake.Commerce.ProductAdjective()} {App.Fake.Commerce.ProductMaterial()} {App.Fake.Commerce.Product()}",
                    ReleaseDate = DateOnly.FromDateTime(App.Fake.Date.Past()),
                    AgeRating = App.Fake.PickRandom(AgeRating.ValidRatings.ToArray()),
                    Description = App.Fake.Lorem.Paragraph(),
                    DeveloperInfo = new(
                        developer: App.Fake.Company.CompanyName(),
                        publisher: App.Fake.Company.CompanyName()
                    ),
                    DiskSize = App.Fake.Random.Int(1, 150),
                    Price = decimal.Parse(App.Fake.Commerce.Price(1.0m, 100.0m, 2)),
                    Playtime = new(
                        hours: App.Fake.Random.Int(1, 100),
                        playerCount: App.Fake.Random.Int(1, 2000)
                    ),
                    GameDetails = new(
                        genre: App.Fake.Commerce.Categories(1)[0],
                        platform: [.. App.Fake.PickRandom(GameDetails.ValidPlatforms, App.Fake.Random.Int(1, GameDetails.ValidPlatforms.Count))],
                        tags: string.Join(", ", App.Fake.Lorem.Words(5)),
                        gameMode: App.Fake.PickRandom(GameDetails.ValidGameModes.ToArray()),
                        distributionFormat: App.Fake.PickRandom(GameDetails.ValidDistributionFormats.ToArray()),
                        availableLanguages: string.Join(", ", App.Fake.Random.ListItems(AvailableLanguagesList, App.Fake.Random.Int(1, AvailableLanguagesList.Length))),
                        supportsDlcs: App.Fake.Random.Bool()
                    ),
                    SystemRequirements = new(
                        minimum: App.Fake.Lorem.Sentence(),
                        recommended: App.Fake.Lorem.Sentence()
                    ),
                    Rating = Math.Round(App.Fake.Random.Decimal(1, 10), 2),
                    OfficialLink = App.Fake.Internet.Url(),
                    GameStatus = App.Fake.PickRandom(Domain.Game.Game.ValidGameStatus.ToArray())
                });
            }

            var fakeHandler = A.Fake<IAppCommandHandler.QueryHandler<GetGameListQuery, IReadOnlyList<GameListResponse>>>();
            A.CallTo(() => fakeHandler.ExecuteAsync(A<GetGameListQuery>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Result<IReadOnlyList<GameListResponse>>.Success(getGameRes)));

            fakeHandler.RegisterForTesting();

            // Act
            await ep.HandleAsync(getGameReq, TestContext.Current.CancellationToken);

            // Assert
            ep.Response[0].Id.ShouldBe(getGameRes[0].Id);
            ep.Response[0].Name.ShouldBe(getGameRes[0].Name);
            ep.Response[0].AgeRating.ShouldBe(getGameRes[0].AgeRating);
            ep.Response[0].GameStatus.ShouldBe(getGameRes[0].GameStatus);

            // Additional Assertions
            var result = await fakeHandler.ExecuteAsync(getGameReq, CancellationToken.None);
            result.IsSuccess.ShouldBeTrue();
            result.IsInvalid().ShouldBeFalse();
            result.ValidationErrors.Count().ShouldBe(0);
            result.Value.ShouldNotBeNull();
            result.Errors.Count().ShouldBe(0);
        }
    }
}
