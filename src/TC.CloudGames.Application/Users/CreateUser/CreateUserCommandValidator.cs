using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Domain.Aggregates.User.ValueObjects;
using TC.CloudGames.Infra.CrossCutting.Commons.Extensions;

namespace TC.CloudGames.Application.Users.CreateUser
{
    public sealed class CreateUserCommandValidator : Validator<CreateUserCommand>
    {
        public CreateUserCommandValidator(IUserPgRepository userPgRepository)
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .WithErrorCode($"{nameof(CreateUserCommand.FirstName)}.Required").DependentRules(() =>
                {
                    RuleFor(x => x.FirstName)
                        .MinimumLength(3).WithMessage("First name must be at least 3 characters long.")
                            .WithErrorCode($"{nameof(CreateUserCommand.FirstName)}.MinimumLength")
                        .Matches(@"^[a-zA-Z]+$")
                            .WithMessage("First name can only contain letters.")
                            .WithErrorCode($"{nameof(CreateUserCommand.FirstName)}.InvalidCharacters");
                });

            RuleFor(x => x.LastName)
                .NotEmpty()
                    .WithMessage("Last name is required.")
                    .WithErrorCode($"{nameof(CreateUserCommand.LastName)}.Required")
                .DependentRules(() =>
                {
                    RuleFor(x => x.LastName)
                        .MinimumLength(3)
                            .WithMessage("Last name must be at least 3 characters long.")
                            .WithErrorCode($"{nameof(CreateUserCommand.LastName)}.MinimumLength")
                        .Matches(@"^[a-zA-Z]+$")
                            .WithMessage("Last name can only contain letters.")
                            .WithErrorCode($"{nameof(CreateUserCommand.LastName)}.InvalidCharacters");
                });

            RuleFor(x => x.Email)
                .NotEmpty()
                    .WithMessage("Email is required.")
                    .WithErrorCode($"{nameof(CreateUserCommand.Email)}.Required")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Email)
                        .EmailAddress()
                            .WithMessage("Invalid email format.")
                            .WithErrorCode($"{nameof(CreateUserCommand.Email)}.InvalidFormat")
                        .MustAsync(async (email, cancellation) => !await userPgRepository.EmailExistsAsync(email, cancellation).ConfigureAwait(false))
                            .WithMessage("Email already exists.")
                            .WithErrorCode($"{nameof(CreateUserCommand.Email)}.AlreadyExists");
                });

            RuleFor(x => x.Role)
                .NotEmpty()
                .WithMessage("Role is required.")
                .WithErrorCode($"{nameof(CreateUserCommand.Role)}.Required")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Role)
                        .Must(role => Role.ValidRoles.Contains(role))
                        .WithMessage($"Invalid role specified. Valid roles are: {Role.ValidRoles.JoinWithQuotes()}.")
                        .WithErrorCode($"{nameof(CreateUserCommand.Role)}.InvalidRole");
                });


            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .WithErrorCode($"{nameof(CreateUserCommand.Password)}.Required")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Password)
                        .MinimumLength(8)
                            .WithMessage("Password must be at least 8 characters long.")
                            .WithErrorCode($"{nameof(CreateUserCommand.Password)}.MinimumLength")
                        .Matches(@"[A-Z]")
                            .WithMessage("Password must contain at least one uppercase letter.")
                            .WithErrorCode($"{nameof(CreateUserCommand.Password)}.Uppercase")
                        .Matches(@"[a-z]")
                            .WithMessage("Password must contain at least one lowercase letter.")
                            .WithErrorCode($"{nameof(CreateUserCommand.Password)}.Lowercase")
                        .Matches(@"\d")
                            .WithMessage("Password must contain at least one number.")
                            .WithErrorCode($"{nameof(CreateUserCommand.Password)}.Digit")
                        .Matches(@"[\W_]")
                            .WithMessage("Password must contain at least one special character.")
                            .WithErrorCode($"{nameof(CreateUserCommand.Password)}.SpecialCharacter");
                });
        }
    }
}
