using FastEndpoints;
using TC.CloudGames.Api.Endpoints.Admin;
using TC.CloudGames.Application.Users.CreateUser;

namespace TC.CloudGames.Api.Endpoints
{
    public sealed class CreateUserEndpoint : Endpoint<CreateUserRequest, CreateUserResponse>
    {
        public override void Configure()
        {
            Post("user/register");
            AllowAnonymous();
            Description(
                x => x.Produces<RegisterResponse>(200) //override swagger response type for 200 ok
                      .Produces<ErrorResponse>(400));
        }

        public override Task HandleAsync(CreateUserRequest req, CancellationToken ct)
        {

        }
    }
}
