using FastEndpoints;
using TC.CloudGames.Application.Users.CreateUser;

namespace TC.CloudGames.Api.Endpoints.Auth
{
    public sealed class CreateUserEndpoint : Endpoint<CreateUserCommand, CreateUserResponse, CreateUserMapper>
    {
        public override void Configure()
        {
            Post("register");
            RoutePrefixOverride("auth");
            AllowAnonymous();
            Description(
                x => x.Produces<CreateUserResponse>(200)
                      .ProducesProblemDetails());

            Summary(s =>
            {
                s.Summary = "Endpoint responsible for creating a new user";
                s.ExampleRequest = new CreateUserCommand("Jhon", "Smith", "jhon.smith@gmail.com", "******", "Admin");
                s.ResponseExamples[200] = new CreateUserResponse(Guid.NewGuid(), "Jhon", "Smith", "jhon.smith@gmail.com", "Admin");
                s.Responses[200] = "Occurs when a new user are created successfully";
                s.Responses[400] = "Occurs when a badrequest happens";
            });
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
