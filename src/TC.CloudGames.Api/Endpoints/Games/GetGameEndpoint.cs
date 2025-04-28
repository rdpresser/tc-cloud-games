using Ardalis.Result;
using FastEndpoints;
using System.Net;
using TC.CloudGames.Application.Games.GetGame;

namespace TC.CloudGames.Api.Endpoints.Games
{
    public sealed class GetGameEndpoint : Endpoint<GetGameQuery, GameResponse>
    {
        public override void Configure()
        {
            Get("game/{Id}");
            Roles("Admin");

            Description(
                x => x.Produces<GameResponse>(200)
                      .ProducesProblemDetails()
                      .Produces((int)HttpStatusCode.NotFound)
                      .Produces((int)HttpStatusCode.Forbidden)
                      .Produces((int)HttpStatusCode.Unauthorized));

            Summary(s =>
            {
                s.Summary = "Retrieve game details by its unique identifier.";
                s.Description = "This endpoint retrieves detailed information about a game by its unique Id. Access is restricted to users with the appropriate roles.";
                s.ExampleRequest = new GetGameQuery(Guid.NewGuid());
                s.ResponseExamples[200] = GetGameResponseExample();
                s.Responses[200] = "Returned when game information is successfully retrieved.";
                s.Responses[400] = "Returned when the request is invalid.";
                s.Responses[404] = "Returned when no game is found with the provided Id.";
                s.Responses[403] = "Returned when the caller lacks the required role to access this endpoint.";
                s.Responses[401] = "Returned when the request is made without a valid user token.";
            });
        }

        public override async Task HandleAsync(GetGameQuery req, CancellationToken ct)
        {
            var response = await req.ExecuteAsync(ct: ct).ConfigureAwait(false);

            if (response.IsSuccess)
            {
                await SendAsync(response.Value, cancellation: ct).ConfigureAwait(false);
                return;
            }

            response.Errors.ToList().ForEach(e => AddError(e));

            if (response.IsNotFound())
            {
                await SendErrorsAsync((int)HttpStatusCode.NotFound, cancellation: ct).ConfigureAwait(false);
                return;
            }

            await SendErrorsAsync(cancellation: ct).ConfigureAwait(false);
        }

        public static GameResponse GetGameResponseExample()
        {
            return new GameResponse
            {
                Id = Guid.NewGuid(),
                Name = "Game Name",
                ReleaseDate = DateOnly.FromDateTime(DateTime.UtcNow),
                AgeRating = Domain.Game.AgeRating.ValidRatings.First(),
                Description = "Game Description",
                DeveloperInfo = new DeveloperInfo { Developer = "Developer Name", Publisher = "Publisher Name" },
                DiskSize = 50.0m,
                Price = 59.99m,
                Playtime = new Playtime { Hours = 10, PlayerCount = 1 },
                GameDetails = new GameDetails
                {
                    Genre = "Genre",
                    Platform = [.. Domain.Game.GameDetails.ValidPlatforms],
                    Tags = "Tags",
                    GameMode = Domain.Game.GameDetails.ValidGameModes.First(),
                    DistributionFormat = Domain.Game.GameDetails.ValidDistributionFormats.First(),
                    AvailableLanguages = "Available Languages",
                    SupportsDlcs = true
                },
                SystemRequirements = new SystemRequirements
                {
                    Minimum = "Minimum Requirements",
                    Recommended = "Recommended Requirements"
                },
                Rating = 4.5m,
                OfficialLink = "https://example.com",
                GameStatus = Domain.Game.Game.ValidGameStatus.First()
            };
        }
    }
}
