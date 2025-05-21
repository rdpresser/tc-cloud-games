namespace TC.CloudGames.Domain.User.Abstractions
{
    public class UserEntityValidator : BaseValidator<User>
    {
        protected void ValidateId()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                    .WithMessage("User ID is required.")
                    .WithErrorCode($"{nameof(User.Id)}.Required")
                .Must(id => id != Guid.Empty)
                    .WithMessage("User ID cannot be empty.")
                    .WithErrorCode($"{nameof(User.Id)}.Empty");
        }

        protected void ValidateFirstName()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                    .WithMessage("First name is required.")
                    .WithErrorCode($"{nameof(User.FirstName)}.Required")
                .MinimumLength(3)
                    .WithMessage("First name must be at least 3 characters long.")
                    .WithErrorCode($"{nameof(User.FirstName)}.MinimumLength")
                .MaximumLength(200)
                    .WithMessage("First name must be at most 200 characters long.")
                    .WithErrorCode($"{nameof(User.FirstName)}.MaximumLength")
                .Matches(@"^[a-zA-Z]+$")
                    .WithMessage("First name can only contain letters.")
                    .WithErrorCode($"{nameof(User.FirstName)}.InvalidCharacters");
        }

        protected void ValidateLastName()
        {
            RuleFor(x => x.LastName)
                .NotEmpty()
                    .WithMessage("Last name is required.")
                    .WithErrorCode($"{nameof(User.LastName)}.Required")
                .MinimumLength(3)
                    .WithMessage("Last name must be at least 3 characters long.")
                    .WithErrorCode($"{nameof(User.LastName)}.MinimumLength")
                .MaximumLength(200)
                    .WithMessage("Last name must be at most 200 characters long.")
                    .WithErrorCode($"{nameof(User.LastName)}.MaximumLength")
                .Matches(@"^[a-zA-Z]+$")
                    .WithMessage("Last name can only contain letters.")
                    .WithErrorCode($"{nameof(User.LastName)}.InvalidCharacters");
        }

        protected void ValidateEmail()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                    .WithMessage("Email object is required.")
                    .WithErrorCode($"{nameof(User.Email)}.Required");
        }

        protected void ValidatePassword()
        {
            RuleFor(x => x.Password)
                .NotEmpty()
                    .WithMessage("Password object is required.")
                    .WithErrorCode($"{nameof(User.Password)}.Required");
        }

        protected void ValidateRole()
        {
            RuleFor(x => x.Role)
                .NotEmpty()
                    .WithMessage("Role object is required.")
                    .WithErrorCode($"{nameof(User.Role)}.Required");
        }
    }
}
