using FastEndpoints;
using TC.CloudGames.Application.Games.CreateGame;
using TC.CloudGames.Application.Middleware;
using TC.CloudGames.CrossCutting.Commons.Extensions;

namespace TC.CloudGames.Api.Endpoints.Games
{
    public sealed class CreateGameEndpoint : Endpoint<CreateGameCommand, CreateGameResponse>
    {
        public override void Configure()
        {
            Post("game");
            Roles("Admin");
            PostProcessor<CommandPostProcessor<CreateGameCommand, CreateGameResponse>>();

            Description(
                x => x.Produces<CreateGameResponse>(200)
                      .ProducesProblemDetails());

            Summary(s =>
            {
                s.Summary = "Endpoint for creating a new game.";
                s.Description = "This endpoint allows for the creation of a new game by providing its name, description, and other relevant details. Upon successful creation, a new game is added to the system.";
                s.ExampleRequest = CreateGameCommandExample();
                s.ResponseExamples[200] = CreateGameResponseExample();
                s.Responses[200] = "Returned when a new game is successfully created.";
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
                await SendAsync(response.Value, cancellation: ct).ConfigureAwait(false);
                return;
            }

            await SendErrorsAsync(cancellation: ct).ConfigureAwait(false);
        }

        public static CreateGameCommand CreateGameCommandExample()
        {
            return new CreateGameCommand(
                Name: "Game Name",
                ReleaseDate: DateOnly.FromDateTime(DateTime.UtcNow),
                AgeRating: $"Choose one of valid age rate: {Domain.Game.AgeRating.ValidRatings.JoinWithQuotes()}",
                Description: "Game Description",
                DeveloperInfo: new DeveloperInfo("Developer Name", "Publisher Name"),
                DiskSize: 50.0m,
                Price: 59.99m,
                Playtime: new Playtime(10, 1),
                GameDetails: new GameDetails
                (
                    Genre: "Genre",
                    Platform: [.. Domain.Game.GameDetails.ValidPlatforms],
                    Tags: "Tags",
                    GameMode: $"Choose one of valid game modes: {Domain.Game.GameDetails.ValidGameModes.JoinWithQuotes()}",
                    DistributionFormat: $"Choose one of valid distribution format: {Domain.Game.GameDetails.ValidDistributionFormats.JoinWithQuotes()}",
                    AvailableLanguages: "Available Languages",
                    SupportsDlcs: true
                ),
                SystemRequirements: new SystemRequirements("Minimum Requirements", "Recommended Requirements"),
                Rating: 4.5m,
                OfficialLink: "https://example.com",
                GameStatus: $"Choose one of valid game status: {Domain.Game.Game.ValidGameStatus.JoinWithQuotes()}"
            );
        }

        public static CreateGameResponse CreateGameResponseExample()
        {
            return new CreateGameResponse(
                Id: Guid.NewGuid(),
                Name: "Game Name",
                ReleaseDate: DateOnly.FromDateTime(DateTime.UtcNow),
                AgeRating: Domain.Game.AgeRating.ValidRatings.First(),
                Description: "Game Description",
                DeveloperInfo: new DeveloperInfo("Developer Name", "Publisher Name"),
                DiskSize: 50.0m,
                Price: 59.99m,
                Playtime: new Playtime(10, 1),
                GameDetails: new GameDetails(
                    Genre: "Genre",
                    Platform: [.. Domain.Game.GameDetails.ValidPlatforms],
                    Tags: "Tags",
                    GameMode: Domain.Game.GameDetails.ValidGameModes.First(),
                    DistributionFormat: Domain.Game.GameDetails.ValidDistributionFormats.First(),
                    AvailableLanguages: "Available Languages",
                    SupportsDlcs: true),
                SystemRequirements: new SystemRequirements("Minimum Requirements", "Recommended Requirements"),
                Rating: 4.5m,
                OfficialLink: "https://example.com",
                GameStatus: Domain.Game.Game.ValidGameStatus.First()
            );
        }
    }
}
