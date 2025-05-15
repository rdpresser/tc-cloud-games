namespace TC.CloudGames.Domain.Game.Abstractions
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
            ValidateRating();
            ValidateGameStatus();
            ValidateOfficialLink();
        }
    }
}
