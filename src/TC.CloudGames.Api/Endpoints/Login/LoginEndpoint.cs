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
            Post("auth/login");
            AllowAnonymous();
            Description(
                x => x.Produces<LoginUserResponse>(200)
                      .ProducesProblemDetails()
                      .Produces((int)HttpStatusCode.NotFound));
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
                await SendNotFoundAsync(cancellation: ct).ConfigureAwait(false);
                return;
            }

            await SendErrorsAsync(cancellation: ct).ConfigureAwait(false);
        }
    }
}
