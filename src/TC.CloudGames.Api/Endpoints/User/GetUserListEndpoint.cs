using FastEndpoints;
using TC.CloudGames.Application.Users.GetUserList;

namespace TC.CloudGames.Api.Endpoints.User
{
    public sealed class GetUserListEndpoint : Endpoint<GetUserListQuery, IReadOnlyList<UserListResponse>>
    {
        public override void Configure()
        {
            Get("user/list");
            AllowAnonymous();
            Description(
                x => x.Produces<UserListResponse>(200)
                      .ProducesProblemDetails());
        }

        public override async Task HandleAsync(GetUserListQuery req, CancellationToken ct)
        {
            var response = await req.ExecuteAsync(ct: ct);

            if (response.IsSuccess)
            {
                await SendAsync(response.Value, cancellation: ct);
                return;
            }

            response.Errors.ToList().ForEach(e => AddError(e));
            await SendErrorsAsync(cancellation: ct);
        }
    }
}
