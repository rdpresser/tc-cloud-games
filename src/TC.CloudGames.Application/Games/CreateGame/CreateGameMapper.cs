using Ardalis.Result;
using TC.CloudGames.Domain.Game;

namespace TC.CloudGames.Application.Games.CreateGame
{
    public static class CreateGameMapper
    {
        public static Result<Game> ToEntity(CreateGameCommand command)
        {
            var gameDetailsResult = Domain.Game.GameDetails.Create(
                command.GameDetails.Genre,
                command.GameDetails.Platform,
                command.GameDetails.Tags,
                command.GameDetails.GameMode,
                command.GameDetails.DistributionFormat,
                command.GameDetails.AvailableLanguages,
                command.GameDetails.SupportsDlcs
            );

            if (!gameDetailsResult.IsSuccess)
            {
                return Result<Game>.Error(new ErrorList(gameDetailsResult.Errors));
            }

            var ageRatingResult = AgeRating.Create(command.AgeRating);
            if (!ageRatingResult.IsSuccess)
            {
                return Result<Game>.Error(new ErrorList(ageRatingResult.Errors));
            }

            var ratingResult = Rating.Create(command.Rating);
            if (!ratingResult.IsSuccess)
            {
                return Result<Game>.Error(new ErrorList(ratingResult.Errors));
            }

            return Game.Create(
                command.Name,
                command.ReleaseDate,
                ageRatingResult.Value,
                command.Description,
                new Domain.Game.DeveloperInfo(command.DeveloperInfo.Developer, command.DeveloperInfo.Publisher),
                new DiskSize(command.DiskSize),
                new Domain.Game.Price(command.Price),
                new Domain.Game.Playtime(command.Playtime.Hours, command.Playtime.PlayerCount),
                gameDetailsResult.Value,
                new Domain.Game.SystemRequirements(command.SystemRequirements.Minimum, command.SystemRequirements.Recommended),
                ratingResult.Value,
                command.OfficialLink,
                command.GameStatus
            );
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
                GameDetails: new GameDetails(game.GameDetails.Genre, [.. game.GameDetails.PlatformList], game.GameDetails.Tags, game.GameDetails.GameMode, game.GameDetails.DistributionFormat, game.GameDetails.AvailableLanguages, game.GameDetails.SupportsDlcs),
                SystemRequirements: new SystemRequirements(game.SystemRequirements.Minimum, game.SystemRequirements.Recommended),
                Rating: game.Rating?.Average ?? 0, // Handle null Rating by providing a default value
                OfficialLink: game.OfficialLink,
                GameStatus: game.GameStatus
            );
        }
    }
}
