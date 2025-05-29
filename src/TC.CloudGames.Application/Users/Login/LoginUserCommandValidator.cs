namespace TC.CloudGames.Application.Users.Login
{
    public sealed class LoginUserCommandValidator : Validator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .WithErrorCode($"{nameof(LoginUserCommand.Email)}.Required")
                .EmailAddress()
                .WithMessage("Invalid email format.")
                .WithErrorCode($"{nameof(LoginUserCommand.Email)}.InvalidEmailFormat");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .WithErrorCode($"{nameof(LoginUserCommand.Password)}.Required")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long.")
                .WithErrorCode($"{nameof(LoginUserCommand.Password)}.MinimumLength")
                .Matches(@"[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter.")
                .WithErrorCode($"{nameof(LoginUserCommand.Password)}.UppercaseLetter")
                .Matches(@"[a-z]")
                .WithMessage("Password must contain at least one lowercase letter.")
                .WithErrorCode($"{nameof(LoginUserCommand.Password)}.LowercaseLetter")
                .Matches(@"\d")
                .WithMessage("Password must contain at least one number.")
                .WithErrorCode($"{nameof(LoginUserCommand.Password)}.ContainNumber")
                .Matches(@"[\W_]")
                .WithMessage("Password must contain at least one special character.")
                .WithErrorCode($"{nameof(LoginUserCommand.Password)}.SpecialCharacter");
        }
    }
}
