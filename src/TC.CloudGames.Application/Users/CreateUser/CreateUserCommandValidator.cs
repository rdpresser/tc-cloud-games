using FastEndpoints;
using FluentValidation;
using TC.CloudGames.Domain.User;

namespace TC.CloudGames.Application.Users.CreateUser
{
    public sealed class CreateUserCommandValidator : Validator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Email)
                        .EmailAddress()
                        .WithMessage("Invalid email format.");
                });

            RuleFor(x => x.Role)
                .NotEmpty()
                .WithMessage("Role is required.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Role)
                        .Must(role => Role.ValidRoles.Contains(role))
                        .WithMessage($"Invalid role specified. Valid roles are: {string.Join(", ", Role.ValidRoles)}.");
                });


            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Password)
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
                });
        }
    }
}
