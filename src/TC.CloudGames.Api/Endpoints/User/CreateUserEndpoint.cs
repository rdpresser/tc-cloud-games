using FastEndpoints;
using TC.CloudGames.Application.Users.CreateUser;

namespace TC.CloudGames.Api.Endpoints.User
{
    public sealed class CreateUserEndpoint : Endpoint<CreateUserCommand, CreateUserResponse, CreateUserMapper>
    {
        public override void Configure()
        {
            Post("user/register");
            AllowAnonymous();
            Description(
                x => x.Produces<CreateUserResponse>(200)
                      .ProducesProblemDetails());
        }

        public override async Task HandleAsync(CreateUserCommand req, CancellationToken ct)
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
