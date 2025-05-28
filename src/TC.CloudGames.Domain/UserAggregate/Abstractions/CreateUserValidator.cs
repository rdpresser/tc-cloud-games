namespace TC.CloudGames.Domain.UserAggregate.Abstractions
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
