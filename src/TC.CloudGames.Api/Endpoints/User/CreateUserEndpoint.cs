using FastEndpoints;
using TC.CloudGames.Application.Users.CreateUser;

namespace TC.CloudGames.Api.Endpoints.User
{
    public sealed class CreateUserEndpoint : Endpoint<CreateUserCommand, CreateUserResponse, CreateUserMapper>
    {
        //private readonly ICreateUserApp _createUserApp;

        //public CreateUserEndpoint(ICreateUserApp createUserApp)
        //{
        //    _createUserApp = createUserApp;
        //}

        public override void Configure()
        {
            Post("user/register");
            AllowAnonymous();
            Description(
                x => x.Produces<CreateUserResponse>(200) //override swagger response type for 200 ok
                      .Produces<ErrorResponse>(400));
        }

        public override Task HandleAsync(CreateUserCommand req, CancellationToken ct)
        {
            var entity = Map.ToEntity(req);

            var response = Map.FromEntity(entity);
            return SendAsync(response, cancellation: ct);
        }
    }
}
