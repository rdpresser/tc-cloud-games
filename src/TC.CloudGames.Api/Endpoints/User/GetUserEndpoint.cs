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
            AllowAnonymous();
            Description(
                x => x.Produces<UserResponse>(200)
                      .ProducesProblemDetails()
                      .Produces((int)HttpStatusCode.NotFound));
        }

        public override async Task HandleAsync(GetUserQuery req, CancellationToken ct)
        {
            var response = await req.ExecuteAsync(ct: ct);

            if (response.IsSuccess)
            {
                await SendAsync(response.Value, cancellation: ct);
                return;
            }

            response.Errors.ToList().ForEach(e => AddError(e));

            if (response.IsNotFound())
            {
                await SendNotFoundAsync(cancellation: ct);
                return;
            }

            await SendErrorsAsync(cancellation: ct);
        }
    }
}
