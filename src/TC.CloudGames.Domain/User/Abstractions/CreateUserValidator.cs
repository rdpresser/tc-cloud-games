namespace TC.CloudGames.Domain.User.Abstractions
{
    public class CreateUserValidator : UserEntityValidator
    {
        public CreateUserValidator()
        {
            ValidateId();
            ValidateFirstName();
            ValidateLastName();
        }
    }
}
