using FastEndpoints;
using TC.CloudGames.Application.Users.CreateUser;
using TC.CloudGames.Domain.User;

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
                s.Summary = "Endpoint for creating a new user.";
                s.Description = "This endpoint allows for the registration of a new user by providing their first name, last name, email, password, and role. Upon successful registration, a new user is created in the system.";
                s.ExampleRequest = new CreateUserCommand("John", "Smith", "john.smith@gmail.com", "******", Role.Create("Admin").Value.Value);
                s.ResponseExamples[200] = new CreateUserResponse(Guid.NewGuid(), "John", "Smith", "john.smith@gmail.com", Role.Create("Admin").Value.Value);
                s.Responses[200] = "Returned when a new user is successfully created.";
                s.Responses[400] = "Returned when a bad request occurs.";
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
