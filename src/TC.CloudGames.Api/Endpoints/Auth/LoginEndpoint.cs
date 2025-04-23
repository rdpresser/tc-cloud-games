using Ardalis.Result;
using FastEndpoints;
using System.Net;
using TC.CloudGames.Application.Users.Login;

namespace TC.CloudGames.Api.Endpoints.Login
{
    sealed class LoginEndpoint : Endpoint<LoginUserCommand, LoginUserResponse>
    {
        public override void Configure()
        {
            Post("login");
            RoutePrefixOverride("auth");
            AllowAnonymous();
            Description(
                x => x.Produces<LoginUserResponse>(200)
                      .ProducesProblemDetails()
                      .Produces((int)HttpStatusCode.NotFound));

            Summary(s =>
            {
                s.Summary = "Endpoint responsible for generating a new token to a user";
                s.ExampleRequest = new LoginUserCommand("jhon.smith@gmail.com", "********");
                s.ResponseExamples[200] = new LoginUserResponse("<base64-jwt-token>", "jhon.smith@gmail.com");
                s.Responses[200] = "Occurs when a user successfully creates a login token";
                s.Responses[400] = "Occurs when a badrequest happens";
                s.Responses[404] = "Occurs when an user login is not found";
            });
        }

        public override async Task HandleAsync(LoginUserCommand req, CancellationToken ct)
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
    }
}
