using FakeItEasy;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Application.Games.CreateGame;
using TC.CloudGames.Domain.Game.Abstractions;
using DeveloperInfo = TC.CloudGames.Application.Games.CreateGame.DeveloperInfo;
using GameDetails = TC.CloudGames.Application.Games.CreateGame.GameDetails;
using Playtime = TC.CloudGames.Application.Games.CreateGame.Playtime;
using SystemRequirements = TC.CloudGames.Application.Games.CreateGame.SystemRequirements;

namespace TC.CloudGames.Application.Tests.Games;

public class CreateGameTests
{
    
    [Fact]
    public async Task Handle_CreateGame()
    {
        
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var gameRepository = A.Fake<IGameEfRepository>();
        
        var command = new CreateGameCommand(
            Name: "Test Game",
            ReleaseDate: DateOnly.FromDateTime(DateTime.UtcNow),
            AgeRating: "18+",
            Description: "Test Description",
            DeveloperInfo: new DeveloperInfo("Test Developer", "Test Publisher"),
            DiskSize: 50.0m,
            Price: 59.99m,
            Playtime: new Playtime(10, 1),
            GameDetails: new GameDetails(
                Genre: "Test Genre",
                Platform: new[] { "PC" },
                Tags: "Action,Adventure",
                GameMode: "Singleplayer",
                DistributionFormat: "Digital",
                AvailableLanguages: "EN,PT",
                SupportsDlcs: true
            ),
            SystemRequirements: new SystemRequirements("Min Spec", "Rec Spec"),
            Rating: 4.5m,
            OfficialLink: "https://testgame.com",
            GameStatus: "Released"
        );
        
        var expectedResponse = new CreateGameResponse(
            Id: Guid.NewGuid(),
            Name: command.Name,
            ReleaseDate: command.ReleaseDate,
            AgeRating: command.AgeRating,
            Description: command.Description,
            DeveloperInfo: command.DeveloperInfo,
            DiskSize: command.DiskSize,
            Price: command.Price,
            Playtime: command.Playtime,
            GameDetails: command.GameDetails,
            SystemRequirements: command.SystemRequirements,
            Rating: command.Rating,
            OfficialLink: command.OfficialLink,
            GameStatus: command.GameStatus
        );
        
        var fakerHandler = A.Fake<ICommandHandler<CreateGameCommand, CreateGameResponse>>();

        A.CallTo(() => fakerHandler.ExecuteAsync(command, A<CancellationToken>.Ignored))
            .Returns(expectedResponse);
        
        // Act
        var result = await fakerHandler.ExecuteAsync(command, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Value.Id);

        A.CallTo(() => fakerHandler.ExecuteAsync(command, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}