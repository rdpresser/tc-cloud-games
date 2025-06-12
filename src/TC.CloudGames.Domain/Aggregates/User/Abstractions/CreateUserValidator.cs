namespace TC.CloudGames.Domain.Aggregates.User.Abstractions
{
    public class CreateUserValidator : UserEntityValidator
    {
        public CreateUserValidator()
        {
            ValidateId();
            ValidateFirstName();
            ValidateLastName();
            ValidateEmail();
            ValidatePassword();
            ValidateRole();
        }
    }
}
