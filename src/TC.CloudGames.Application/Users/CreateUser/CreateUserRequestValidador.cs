using FastEndpoints;
using FluentValidation;

namespace TC.CloudGames.Application.Users.CreateUser
{
    public sealed class CreateUserRequestValidador : Validator<CreateUserRequest>
    {
        public CreateUserRequestValidador()
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
}
