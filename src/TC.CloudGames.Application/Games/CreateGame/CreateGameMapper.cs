using Ardalis.Result;
using TC.CloudGames.Domain.Game;

namespace TC.CloudGames.Application.Games.CreateGame
{
    public static class CreateGameMapper
    {
        public static Result<Game> ToEntity(CreateGameCommand command)
        {
            var gameResult = Game.Create(
                name: command.Name,
                releaseDate: command.ReleaseDate,
                ageRating: command.AgeRating,
                description: command.Description,
                developerInfo: (command.DeveloperInfo.Developer, command.DeveloperInfo.Publisher),
                diskSize: command.DiskSize,
                price: command.Price,
                playtime: command.Playtime != null ? (command.Playtime.Hours, command.Playtime.PlayerCount) : null,
                gameDetails: (
                    genre: command.GameDetails.Genre,
                    platform: command.GameDetails.Platform,
                    tags: command.GameDetails.Tags,
                    gameMode: command.GameDetails.GameMode,
                    distributionFormat: command.GameDetails.DistributionFormat,
                    availableLanguages: command.GameDetails.AvailableLanguages,
                    supportsDlcs: command.GameDetails.SupportsDlcs
                ),
                systemRequirements: (command.SystemRequirements.Minimum, command.SystemRequirements.Recommended),
                rating: command.Rating,
                officialLink: command.OfficialLink,
                gameStatus: command.GameStatus
            );

            return gameResult;
        }

        public static CreateGameResponse FromEntity(Game game)
        {
            return new CreateGameResponse(
                Id: game.Id,
                Name: game.Name,
                ReleaseDate: game.ReleaseDate,
                AgeRating: game.AgeRating.ToString(),
                Description: game.Description,
                DeveloperInfo: new DeveloperInfo(game.DeveloperInfo.Developer, game.DeveloperInfo.Publisher),
                DiskSize: game.DiskSize.SizeInGb,
                Price: game.Price.Amount,
                Playtime: game.Playtime != null ? new Playtime(game.Playtime.Hours, game.Playtime.PlayerCount) : null,
                GameDetails:
                    new GameDetails(
                        Genre: game.GameDetails.Genre,
                        Platform: [.. game.GameDetails.PlatformList],
                        Tags: game.GameDetails.Tags,
                        GameMode: game.GameDetails.GameMode,
                        DistributionFormat: game.GameDetails.DistributionFormat,
                        AvailableLanguages: game.GameDetails.AvailableLanguages,
                        SupportsDlcs: game.GameDetails.SupportsDlcs),
                SystemRequirements: new SystemRequirements(game.SystemRequirements.Minimum, game.SystemRequirements.Recommended),
                Rating: game.Rating?.Average ?? 0, // Handle null Rating by providing a default value
                OfficialLink: game.OfficialLink,
                GameStatus: game.GameStatus
            );
        }
    }
}
