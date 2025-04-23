using FastEndpoints;
using System.Net;
using TC.CloudGames.Application.Users.GetUserList;

namespace TC.CloudGames.Api.Endpoints.User
{
    public sealed class GetUserListEndpoint : Endpoint<GetUserListQuery, IReadOnlyList<UserListResponse>>
    {
        public override void Configure()
        {
            Get("user/list");
            Roles("Admin");
            Description(
                x => x.Produces<UserListResponse>(200)
                      .ProducesProblemDetails()
                      .Produces((int)HttpStatusCode.Forbidden)
                      .Produces((int)HttpStatusCode.Unauthorized));
        }

        public override async Task HandleAsync(GetUserListQuery req, CancellationToken ct)
        {
            var response = await req.ExecuteAsync(ct: ct).ConfigureAwait(false);

            if (response.IsSuccess)
            {
                await SendAsync(response.Value, cancellation: ct).ConfigureAwait(false);
                return;
            }

            response.Errors.ToList().ForEach(e => AddError(e));
            await SendErrorsAsync(cancellation: ct).ConfigureAwait(false);
        }
    }
}
