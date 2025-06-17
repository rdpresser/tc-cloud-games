using TC.CloudGames.Domain.Aggregates.Game;
using TC.CloudGames.Domain.Aggregates.Game.Abstractions;
using TC.CloudGames.Unit.Tests.Fakes;
using DeveloperInfo = TC.CloudGames.Application.Games.CreateGame.DeveloperInfo;
using GameDetails = TC.CloudGames.Application.Games.CreateGame.GameDetails;
using Playtime = TC.CloudGames.Application.Games.CreateGame.Playtime;
using SystemRequirements = TC.CloudGames.Application.Games.CreateGame.SystemRequirements;

namespace TC.CloudGames.Unit.Tests.Application.Games
{
    public class CreateGameCommandHandlerTests
    {
        [Fact]
        public async Task ExecuteAsync_ValidCommand_AddsGameAndSavesChanges()
        {
            // Arrange
            Factory.RegisterTestServices(_ => { });

            var gameValid = FakeGameData.GameValid();

            var name = gameValid.Name;
            var releaseDate = DateOnly.FromDateTime(gameValid.ReleaseDate);
            var ageRating = gameValid.AgeRating;
            var description = gameValid.Description;
            var developerInfo = new DeveloperInfo(gameValid.DeveloperName, gameValid.PublisherName);
            var diskSize = gameValid.DiskSize;
            var price = gameValid.Price;
            var playtime = new Playtime(gameValid.PlayersTime, gameValid.PlayersCount);

            var unitOfWorkFake = A.Fake<IUnitOfWork>();
            var repoFake = A.Fake<IGameEfRepository>();

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

            var handler = new CreateGameCommandHandler(unitOfWorkFake, repoFake);

            // Act
            var result = await handler.ExecuteAsync(command, TestContext.Current.CancellationToken);

            // Assert
            A.CallTo(() => repoFake.Add(A<Game>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => unitOfWorkFake.SaveChangesAsync(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.True(result.IsSuccess);
        }
    }
}
