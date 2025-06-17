using TC.CloudGames.Domain.Aggregates.Game.ValueObjects;
using TC.CloudGames.Unit.Tests.Fakes;
using DeveloperInfo = TC.CloudGames.Application.Games.CreateGame.DeveloperInfo;
using DomainGameDetails = TC.CloudGames.Domain.Aggregates.Game.ValueObjects.GameDetails;
using GameDetails = TC.CloudGames.Application.Games.CreateGame.GameDetails;
using Playtime = TC.CloudGames.Application.Games.CreateGame.Playtime;
using SystemRequirements = TC.CloudGames.Application.Games.CreateGame.SystemRequirements;

namespace TC.CloudGames.Unit.Tests.Application.Games
{
    public class CreateGameCommandValidatorTests
    {
        [Fact]
        public void Should_Return_Error_When_Name_Is_Empty()
        {
            // Arrange
            var gameValid = FakeGameData.GameValid();
            
            var releaseDate = DateOnly.FromDateTime(gameValid.ReleaseDate);
            var ageRating = gameValid.AgeRating;
            var description = gameValid.Description;
            var developerInfo = new DeveloperInfo(gameValid.DeveloperName, gameValid.PublisherName);
            var diskSize = gameValid.DiskSize;
            var price = gameValid.Price;
            var playtime = new Playtime(gameValid.PlayersTime, gameValid.PlayersCount);

            var validator = new CreateGameCommandValidator();
            var command = new CreateGameCommand(
                Name: "",
                ReleaseDate: releaseDate,
                AgeRating: ageRating,
                Description: description,
                DeveloperInfo: developerInfo,
                DiskSize: diskSize,
                Price: price,
                Playtime: playtime,
                GameDetails: new GameDetails(
                    Genre: gameValid.Genre,
                    Platform: gameValid.Platforms.ToArray(),
                    Tags: gameValid.Tags,
                    GameMode: gameValid.GameMode,
                    DistributionFormat: gameValid.DistributionFormat,
                    AvailableLanguages: gameValid.Language,
                    SupportsDlcs: true
                ),
                SystemRequirements: new SystemRequirements(gameValid.SystemMinimalRequirements, gameValid.SystemRecommendRequirements),
                Rating: null,
                OfficialLink: gameValid.URL,
                GameStatus: gameValid.GameStatus
            );

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Name");
        }

        [Fact]
        public void Should_Pass_When_Command_Is_Valid()
        {
            // Arrange
            var gameValid = FakeGameData.GameValid();
            
            var name = gameValid.Name;
            var releaseDate = DateOnly.FromDateTime(gameValid.ReleaseDate);
            var ageRating = gameValid.AgeRating;
            var description = gameValid.Description;
            var developerInfo = new DeveloperInfo(gameValid.DeveloperName, gameValid.PublisherName);
            var diskSize = gameValid.DiskSize;
            var price = gameValid.Price;
            var playtime = new Playtime(gameValid.PlayersTime, gameValid.PlayersCount);

            var validator = new CreateGameCommandValidator();
            var command = new CreateGameCommand(
                Name: name,
                ReleaseDate: releaseDate,
                AgeRating: ageRating,
                Description: description,
                DeveloperInfo: developerInfo,
                DiskSize: diskSize,
                Price: price,
                Playtime: playtime,
                GameDetails: new GameDetails(
                    Genre: gameValid.Genre,
                    Platform: gameValid.Platforms.ToArray(),
                    Tags: gameValid.Tags,
                    GameMode: gameValid.GameMode,
                    DistributionFormat: gameValid.DistributionFormat,
                    AvailableLanguages: gameValid.Language,
                    SupportsDlcs: true
                ),
                SystemRequirements: new SystemRequirements(gameValid.SystemMinimalRequirements, gameValid.SystemRecommendRequirements),
                Rating: null,
                OfficialLink: gameValid.URL,
                GameStatus: gameValid.GameStatus
            );

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
        }
    }
}
