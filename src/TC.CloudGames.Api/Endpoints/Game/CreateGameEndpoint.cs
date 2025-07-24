using TC.CloudGames.Application.Games.CreateGame;
using TC.CloudGames.Domain.Aggregates.Game.ValueObjects;
using TC.CloudGames.Infra.CrossCutting.Commons.Extensions;
using CreateGame = TC.CloudGames.Application.Games.CreateGame;

namespace TC.CloudGames.Api.Endpoints.Game
{
    public sealed class CreateGameEndpoint : Endpoint<CreateGameCommand, CreateGameResponse>
    {
        public override void Configure()
        {
            Post("game");
            Roles(AppConstants.AdminRole);
            PostProcessor<CommandPostProcessor<CreateGameCommand, CreateGameResponse>>();

            Description(
                x => x.Produces<CreateGameResponse>(201)
                      .ProducesProblemDetails());

            Summary(s =>
            {
                s.Summary = "Endpoint for creating a new game.";
                s.Description = "This endpoint allows for the creation of a new game by providing its name, description, and other relevant details. Upon successful creation, a new game is added to the system.";
                s.ExampleRequest = CreateGameCommandExample();
                s.ResponseExamples[201] = CreateGameResponseExample();
                s.Responses[201] = "Returned when a new game is successfully created.";
                s.Responses[400] = "Returned when a bad request occurs.";
                s.Responses[403] = "Returned when the caller lacks the required role to access this endpoint.";
                s.Responses[401] = "Returned when the request is made without a valid user token.";
            });
        }
        public override async Task HandleAsync(CreateGameCommand req, CancellationToken ct)
        {
            var response = await req.ExecuteAsync(ct: ct).ConfigureAwait(false);

            if (response.IsSuccess)
            {
                string location = $"{BaseURL}api/game/";
                object routeValues = new { id = response.Value.Id };
                await Send.CreatedAtAsync(location, routeValues, response.Value, cancellation: ct).ConfigureAwait(false);
                return;
            }

            await Send.ErrorsAsync(cancellation: ct).ConfigureAwait(false);
        }

        public static CreateGameCommand CreateGameCommandExample()
        {
            return new CreateGameCommand(
                Name: "Game Name",
                ReleaseDate: DateOnly.FromDateTime(DateTime.UtcNow),
                AgeRating: $"Choose one of valid age rate: {AgeRating.ValidRatings.JoinWithQuotes()}",
                Description: "Game Description",
                DeveloperInfo: new CreateGame.DeveloperInfo("Developer Name", "Publisher Name"),
                DiskSize: 50.0m,
                Price: 59.99m,
                Playtime: new CreateGame.Playtime(10, 1),
                GameDetails: new CreateGame.GameDetails
                (
                    Genre: "Genre",
                    Platform: [.. Domain.Aggregates.Game.ValueObjects.GameDetails.ValidPlatforms],
                    Tags: "Tags",
                    GameMode: $"Choose one of valid game modes: {Domain.Aggregates.Game.ValueObjects.GameDetails.ValidGameModes.JoinWithQuotes()}",
                    DistributionFormat: $"Choose one of valid distribution format: {Domain.Aggregates.Game.ValueObjects.GameDetails.ValidDistributionFormats.JoinWithQuotes()}",
                    AvailableLanguages: "Available Languages",
                    SupportsDlcs: true
                ),
                SystemRequirements: new CreateGame.SystemRequirements("Minimum Requirements", "Recommended Requirements"),
                Rating: 4.5m,
                OfficialLink: "https://example.com",
                GameStatus: $"Choose one of valid game status: {Domain.Aggregates.Game.Game.ValidGameStatus.JoinWithQuotes()}"
            );
        }

        public static CreateGameResponse CreateGameResponseExample()
        {
            return new CreateGameResponse(
                Id: Guid.NewGuid(),
                Name: "Game Name",
                ReleaseDate: DateOnly.FromDateTime(DateTime.UtcNow),
                AgeRating: AgeRating.ValidRatings.First(),
                Description: "Game Description",
                DeveloperInfo: new CreateGame.DeveloperInfo("Developer Name", "Publisher Name"),
                DiskSize: 50.0m,
                Price: 59.99m,
                Playtime: new CreateGame.Playtime(10, 1),
                GameDetails: new CreateGame.GameDetails(
                    Genre: "Genre",
                    Platform: [.. Domain.Aggregates.Game.ValueObjects.GameDetails.ValidPlatforms],
                    Tags: "Tags",
                    GameMode: Domain.Aggregates.Game.ValueObjects.GameDetails.ValidGameModes.First(),
                    DistributionFormat: Domain.Aggregates.Game.ValueObjects.GameDetails.ValidDistributionFormats.First(),
                    AvailableLanguages: "Available Languages",
                    SupportsDlcs: true),
                SystemRequirements: new CreateGame.SystemRequirements("Minimum Requirements", "Recommended Requirements"),
                Rating: 4.5m,
                OfficialLink: "https://example.com",
                GameStatus: Domain.Aggregates.Game.Game.ValidGameStatus.First()
            );
        }
    }
}
