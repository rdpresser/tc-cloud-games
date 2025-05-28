namespace TC.CloudGames.Domain.GameAggregate.Abstractions
{
    public class CreateGameValidator : GameEntityValidator
    {
        public CreateGameValidator()
        {
            ValidateName();
            ValidateReleaseDate();
            ValidateAgeRating();
            ValidateDescription();
            ValidateDeveloperInfo();
            ValidateDiskSize();
            ValidatePrice();
            ValidatePlaytime();
            ValidateGameDetails();
            ValidateSystemRequirements();
            //ValidateRating();
            ValidateGameStatus();
            ValidateOfficialLink();
        }
    }
}
