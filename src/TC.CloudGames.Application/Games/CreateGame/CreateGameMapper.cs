using Ardalis.Result;
using TC.CloudGames.Domain.Game;
using TC.CloudGames.Infra.CrossCutting.Commons.Clock;

namespace TC.CloudGames.Application.Games.CreateGame
{
    public static class CreateGameMapper
    {
        public static Result<Game> ToEntity(CreateGameCommand command, IDateTimeProvider dateTimeProvider)
        {
            List<ValidationError> validation = [];

            var gameDetailsResult = Domain.Game.GameDetails.Create(
                genre: command.GameDetails.Genre,
                platform: command.GameDetails.Platform,
                tags: command.GameDetails.Tags,
                gameMode: command.GameDetails.GameMode,
                distributionFormat: command.GameDetails.DistributionFormat,
                availableLanguages: command.GameDetails.AvailableLanguages,
                supportsDlcs: command.GameDetails.SupportsDlcs
            );

            if (!gameDetailsResult.IsSuccess)
            {
                validation.AddRange(gameDetailsResult.ValidationErrors);
            }

            var ageRatingResult = AgeRating.Create(command.AgeRating);
            if (!ageRatingResult.IsSuccess)
            {
                validation.AddRange(ageRatingResult.ValidationErrors);
            }

            var ratingResult = Rating.Create(command.Rating);
            if (!ratingResult.IsSuccess)
            {
                validation.AddRange(ratingResult.ValidationErrors);
            }

            var gameResult = Game.Create(
                name: command.Name,
                releaseDate: command.ReleaseDate,
                ageRating: ageRatingResult.Value,
                description: command.Description,
                developerInfo: new Domain.Game.DeveloperInfo(command.DeveloperInfo.Developer, command.DeveloperInfo.Publisher),
                diskSize: new DiskSize(command.DiskSize),
                price: new Domain.Game.Price(command.Price),
                playtime: command.Playtime != null ? new Domain.Game.Playtime(command.Playtime.Hours, command.Playtime.PlayerCount) : null,
                gameDetails: gameDetailsResult.Value,
                systemRequirements: new Domain.Game.SystemRequirements(command.SystemRequirements.Minimum, command.SystemRequirements.Recommended),
                rating: ratingResult.Value,
                officialLink: command.OfficialLink,
                gameStatus: command.GameStatus,
                createdOnUtc: dateTimeProvider.UtcNow
            );

            if (!gameResult.IsSuccess)
            {
                validation.AddRange(gameResult.ValidationErrors);
            }

            if (validation.Count != 0)
            {
                return Result<Game>.Invalid(validation);
            }

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
