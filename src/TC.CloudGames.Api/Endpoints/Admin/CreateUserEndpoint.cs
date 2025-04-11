using FastEndpoints;
using FluentValidation;

namespace TC.CloudGames.Api.Endpoints.Admin
{
    public sealed class CreateUserEndpoint : Endpoint<RegisterRequest, RegisterResponse>
    {
        public override void Configure()
        {
            AllowAnonymous();
            Post("identity/register");
        }

        public override Task<RegisterResponse> ExecuteAsync(RegisterRequest req, CancellationToken ct)
        {
            var response = new RegisterResponse(Guid.NewGuid().ToString(), req.Name, req.Email, req.Role);
            return Task.FromResult(response);
        }
    }

    public sealed class CreateUserValidator : Validator<RegisterRequest>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.");

            RuleFor(x => x.Role)
                .NotEmpty()
                .WithMessage("Role is required.")
                .Must(role => role == "Admin" || role == "User")
                .WithMessage("Role must be either 'Admin' or 'User'.");
        }
    }

    public record RegisterRequest(string Name, string Email, string Role);

    public record RegisterResponse(string Id, string Name, string Email, string Role);

    public record UserDto(string Id, string Name, string Email, string Role);
}
