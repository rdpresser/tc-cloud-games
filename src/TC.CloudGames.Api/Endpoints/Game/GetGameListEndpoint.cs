using Bogus;
using TC.CloudGames.Application.Games.GetGameList;
using TC.CloudGames.Domain.GameAggregate.ValueObjects;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;
using ZiggyCreatures.Caching.Fusion;

namespace TC.CloudGames.Api.Endpoints.Game
{
    public sealed class GetGameListEndpoint : ApiEndpoint<GetGameListQuery, IReadOnlyList<GameListResponse>>
    {
        private static readonly string[] AvailableLanguagesList = ["English", "Spanish", "French", "German", "Japanese"];

        public GetGameListEndpoint(IFusionCache cache, IUserContext userContext)
            : base(cache, userContext)
        {
        }

        public override void Configure()
        {
            Get("game/list");
            Roles(AppConstants.AdminRole);
            PostProcessor<CommandPostProcessor<GetGameListQuery, IReadOnlyList<GameListResponse>>>();

            Description(
                x => x.Produces<GameListResponse>(200)
                      .ProducesProblemDetails()
                      .Produces((int)HttpStatusCode.Forbidden)
                      .Produces((int)HttpStatusCode.Unauthorized));

            var faker = new Faker();
            List<GameListResponse> gameList = [];
            for (int i = 0; i < 5; i++)
            {
                gameList.Add(GetGameResponseExample(faker));
            }

            Summary(s =>
            {
                s.Summary = "Retrieves a paginated list of games based on the provided filters.";
                s.ExampleRequest = new GetGameListQuery(PageNumber: 1, PageSize: 10, SortBy: "id", SortDirection: "asc", Filter: "<any value/field>");
                s.ResponseExamples[200] = gameList;
                s.Responses[200] = "Returned when the game list is successfully retrieved using the specified filters.";
                s.Responses[400] = "Returned when the request contains invalid parameters.";
                s.Responses[403] = "Returned when the logged-in game lacks the required role to access this endpoint.";
                s.Responses[401] = "Returned when the request is made without a valid game token.";
                s.Responses[404] = "Returned when no Games are found matching the specified filters.";
            });
        }

        public override async Task HandleAsync(GetGameListQuery req, CancellationToken ct)
        {
            // Cache keys for user data and validation failures
            var cacheKey = $"GameList-{req.PageNumber}-{req.PageSize}-{req.SortBy}-{req.SortDirection}-{req.Filter}";
            var validationFailuresCacheKey = $"ValidationFailures-{cacheKey}";

            // Use the helper to handle caching and validation
            var response = await GetOrSetWithValidationAsync
                (
                    cacheKey,
                    validationFailuresCacheKey,
                    req.ExecuteAsync,
                    ct
                ).ConfigureAwait(false);

            // Use the MatchResultAsync method from the base class
            await MatchResultAsync(response, ct).ConfigureAwait(false);
        }

        public static GameListResponse GetGameResponseExample(Faker faker)
        {
            return new GameListResponse
            {
                Id = Guid.NewGuid(),
                Name = $"{faker.Commerce.ProductAdjective()} {faker.Commerce.ProductMaterial()} {faker.Commerce.Product()}",
                ReleaseDate = DateOnly.FromDateTime(faker.Date.Past()),
                AgeRating = faker.PickRandom(AgeRating.ValidRatings.ToArray()),
                Description = faker.Lorem.Paragraph(),
                DeveloperInfo = new(
                    developer: faker.Company.CompanyName(),
                    publisher: faker.Company.CompanyName()
                ),
                DiskSize = faker.Random.Int(1, 150),
                Price = decimal.Parse(faker.Commerce.Price(1.0m, 100.0m, 2)),
                Playtime = new(
                    hours: faker.Random.Int(1, 100),
                    playerCount: faker.Random.Int(1, 2000)
                ),
                GameDetails = new(
                    genre: faker.Commerce.Categories(1)[0],
                    platform: [.. faker.PickRandom(GameDetails.ValidPlatforms, faker.Random.Int(1, GameDetails.ValidPlatforms.Count))],
                    tags: string.Join(", ", faker.Lorem.Words(5)),
                    gameMode: faker.PickRandom(GameDetails.ValidGameModes.ToArray()),
                    distributionFormat: faker.PickRandom(GameDetails.ValidDistributionFormats.ToArray()),
                    availableLanguages: string.Join(", ", faker.Random.ListItems(AvailableLanguagesList, faker.Random.Int(1, AvailableLanguagesList.Length))),
                    supportsDlcs: faker.Random.Bool()
                ),
                SystemRequirements = new(
                    minimum: faker.Lorem.Sentence(),
                    recommended: faker.Lorem.Sentence()
                ),
                Rating = Math.Round(faker.Random.Decimal(1, 10), 2),
                OfficialLink = faker.Internet.Url(),
                GameStatus = faker.PickRandom(Domain.GameAggregate.Game.ValidGameStatus.ToArray())
            };
        }
    }
}
