using FastEndpoints;
using FluentValidation;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Infra.CrossCutting.Commons.Extensions;

namespace TC.CloudGames.Application.Users.CreateUser
{
    public sealed class CreateUserCommandValidator : Validator<CreateUserCommand>
    {
        public CreateUserCommandValidator(IUserPgRepository userPgRepository)
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
                        .WithMessage("Invalid email format.")
                        .MustAsync(async (email, cancellation) =>
                            !await userPgRepository.EmailExistsAsync(email, cancellation))
                        .WithMessage("Email already exists.");
                });

            RuleFor(x => x.Role)
                .NotEmpty()
                .WithMessage("Role is required.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Role)
                        .Must(role => Role.ValidRoles.Contains(role))
                        .WithMessage($"Invalid role specified. Valid roles are: {Role.ValidRoles.JoinWithQuotes()}.");
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
