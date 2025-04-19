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
                x => x.Produces<CreateUserResponse>(200) //override swagger response type for 200 ok
                      .Produces<ProblemDetails>(400));
        }

        public override async Task HandleAsync(CreateUserCommand req, CancellationToken ct)
        {
            var response = await req.ExecuteAsync(ct: ct);

            if (response.IsSuccess)
            {
                await SendAsync(response.Value, cancellation: ct);
                return;
            }

            AddError(response.Errors.First());
            await SendErrorsAsync(cancellation: ct);
        }
    }
}
