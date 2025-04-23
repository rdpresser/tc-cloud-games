using Ardalis.Result;
using FastEndpoints;
using System.Net;
using TC.CloudGames.Application.Users.GetUser;

namespace TC.CloudGames.Api.Endpoints.User
{
    public sealed class GetUserEndpoint : Endpoint<GetUserQuery, UserResponse>
    {
        public override void Configure()
        {

            Get("user/{Id}");
            Roles("Admin");
            Description(
                x => x.Produces<UserResponse>(200)
                      .ProducesProblemDetails()
                      .Produces((int)HttpStatusCode.NotFound)
                      .Produces((int)HttpStatusCode.Forbidden)
                      .Produces((int)HttpStatusCode.Unauthorized));
        }

        public override async Task HandleAsync(GetUserQuery req, CancellationToken ct)
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
