using Bogus;
using FastEndpoints;
using System.Net;
using TC.CloudGames.Application.Games.GetGameList;
using TC.CloudGames.Application.Middleware;
using TC.CloudGames.Domain.Game;

namespace TC.CloudGames.Api.Endpoints.Games
{
    public sealed class GetGameListEndpoint : Endpoint<GetGameListQuery, IReadOnlyList<GameListResponse>>
    {
        public override void Configure()
        {
            Get("game/list");
            Roles("Admin");
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
            var response = await req.ExecuteAsync(ct: ct).ConfigureAwait(false);

            if (response.IsSuccess)
            {
                await SendAsync(response.Value, cancellation: ct).ConfigureAwait(false);
                return;
            }

            await SendErrorsAsync(cancellation: ct).ConfigureAwait(false);
        }

        private static readonly string[] AvailableLanguagesList = ["English", "Spanish", "French", "German", "Japanese"];

        public static GameListResponse GetGameResponseExample(Faker faker)
        {
            return new GameListResponse
            {
                Id = Guid.NewGuid(),
                Name = $"{faker.Commerce.ProductAdjective()} {faker.Commerce.ProductMaterial()} {faker.Commerce.Product()}",
                ReleaseDate = DateOnly.FromDateTime(faker.Date.Past()),
                AgeRating = faker.PickRandom(AgeRating.ValidRatings.ToArray()),
                Description = faker.Lorem.Paragraph(),
                DeveloperInfo = new Application.Games.GetGame.DeveloperInfo
                {
                    Developer = faker.Company.CompanyName(),
                    Publisher = faker.Company.CompanyName()
                },
                DiskSize = faker.Random.Int(1, 150),
                Price = decimal.Parse(faker.Commerce.Price(1.0m, 100.0m, 2)),
                Playtime = new Application.Games.GetGame.Playtime
                {
                    Hours = faker.Random.Int(1, 100),
                    PlayerCount = faker.Random.Int(1, 2000)
                },
                GameDetails = new Application.Games.GetGame.GameDetails
                {
                    Genre = faker.Commerce.Categories(1)[0],
                    Platform = [.. faker.PickRandom(Domain.Game.GameDetails.ValidPlatforms, faker.Random.Int(1, Domain.Game.GameDetails.ValidPlatforms.Count))],
                    Tags = string.Join(", ", faker.Lorem.Words(5)),
                    GameMode = faker.PickRandom(Domain.Game.GameDetails.ValidGameModes.ToArray()),
                    DistributionFormat = faker.PickRandom(Domain.Game.GameDetails.ValidDistributionFormats.ToArray()),
                    AvailableLanguages = string.Join(", ", faker.Random.ListItems(AvailableLanguagesList, faker.Random.Int(1, AvailableLanguagesList.Length))),
                    SupportsDlcs = faker.Random.Bool()
                },
                SystemRequirements = new Application.Games.GetGame.SystemRequirements
                {
                    Minimum = faker.Lorem.Sentence(),
                    Recommended = faker.Lorem.Sentence()
                },
                Rating = Math.Round(faker.Random.Decimal(1, 10), 2),
                OfficialLink = faker.Internet.Url(),
                GameStatus = faker.PickRandom(Domain.Game.Game.ValidGameStatus.ToArray())
            };
        }
    }
}
