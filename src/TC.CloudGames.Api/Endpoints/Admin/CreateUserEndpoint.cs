using FastEndpoints;
using FluentValidation;

namespace TC.CloudGames.Api.Endpoints.Admin
{
    public sealed class CreateUserEndpoint : Endpoint<RegisterRequest, RegisterResponse>
    {
        public override void Configure()
        {
            Post("identity/register");
            AllowAnonymous();
            Description(
                x => x.Produces<RegisterResponse>(200) //override swagger response type for 200 ok
                      .Produces<ErrorResponse>(400));
        }

        public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
        {
            AddError(r => r.Email, "this email is already in use!");
            AddError(r => r.Name, "you are not eligible for insurance!");
            AddError(r => r.Name, "you are not eligible for insurance222!");

            //ThrowIfAnyErrors(); // If there are errors, execution shouldn't go beyond this point

            //ThrowError(r => r.Email, "creating a user did not go so well!"); // Error response sent here

            if (ValidationFailed)
            {
                await SendErrorsAsync(cancellation: ct);
                return;
            }

            await SendAsync(new(Guid.NewGuid(), req.Name, req.Email, req.Role), cancellation: ct);
        }

        //public override async Task<RegisterResponse> ExecuteAsync(RegisterRequest req, CancellationToken ct)
        //{
        //    AddError(r => r.Email, "this email is already in use!");
        //    AddError(r => r.Name, "you are not eligible for insurance!");
        //    AddError(r => r.Name, "you are not eligible for insurance222!");

        //    if (ValidationFailed)
        //    {
        //        await SendErrorsAsync(cancellation: ct);
        //        return default!; // Explicitly return null with null-forgiving operator to satisfy non-nullable return type
        //    }

        //    return new RegisterResponse(Guid.NewGuid(), req.Name, req.Email, req.Role);
        //}
    }

    public sealed class CreateUser3Validator : Validator<RegisterRequest>
    {
        public CreateUser3Validator()
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

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long.")
                .Matches(@"[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]")
                .WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"\d")
                .WithMessage("Password must contain at least one number.")
                .Matches(@"[\W_]")
                .WithMessage("Password must contain at least one special character.");
        }
    }

    public record RegisterRequest(string Name, string Email, string Password, string Role);

    public record RegisterResponse(Guid Id, string Name, string Email, string Role);
}
