using TC.CloudGames.Application.Users.CreateUser;
using TC.CloudGames.Domain.Aggregates.User.ValueObjects;

namespace TC.CloudGames.Api.Endpoints.Auth
{
    public sealed class CreateUserEndpoint : Endpoint<CreateUserCommand, CreateUserResponse>
    {
        public override void Configure()
        {
            Post("register");
            RoutePrefixOverride("auth");
            PostProcessor<CommandPostProcessor<CreateUserCommand, CreateUserResponse>>();

            AllowAnonymous();
            Description(
                x => x.Produces<CreateUserResponse>(201)
                      .ProducesProblemDetails());

            Summary(s =>
            {
                s.Summary = "Endpoint for creating a new user.";
                s.Description = "This endpoint allows for the registration of a new user by providing their first name, last name, email, password, and role. Upon successful registration, a new user is created in the system.";
                s.ExampleRequest = new CreateUserCommand("John", "Smith", "john.smith@gmail.com", "******", Role.Create(builder => builder.Value = "Admin").Value.Value);
                s.ResponseExamples[201] = new CreateUserResponse(Guid.NewGuid(), "John", "Smith", "john.smith@gmail.com", Role.Create(builder => builder.Value = "Admin").Value.Value);
                s.Responses[201] = "Returned when a new user is successfully created.";
                s.Responses[400] = "Returned when a bad request occurs.";
            });
        }

        public override async Task HandleAsync(CreateUserCommand req, CancellationToken ct)
        {
            var response = await req.ExecuteAsync(ct: ct).ConfigureAwait(false);

            if (response.IsSuccess)
            {
                string location = $"{BaseURL}api/user/";
                object routeValues = new { id = response.Value.Id };
                await Send.CreatedAtAsync(location, routeValues, response.Value, cancellation: ct).ConfigureAwait(false);
                return;
            }

            await Send.ErrorsAsync(cancellation: ct).ConfigureAwait(false);
        }
    }
}