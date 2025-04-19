using FastEndpoints;
using TC.CloudGames.Application.Users.CreateUser;

namespace TC.CloudGames.Api.Endpoints.User
{
    public sealed class CreateUserEndpoint : Endpoint<CreateUserRequest, CreateUserResponse>
    {
        public override void Configure()
        {
            Post("user/register");
            AllowAnonymous();
            Description(
                x => x.Produces<CreateUserResponse>(200) //override swagger response type for 200 ok
                      .Produces<ProblemDetails>(400));
        }

        public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
        {
            var command = new CreateUserCommand(
                req.FirstName,
                req.LastName,
                req.Email,
                req.Password,
                req.Role);

            var response = await command.ExecuteAsync(ct: ct);

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
