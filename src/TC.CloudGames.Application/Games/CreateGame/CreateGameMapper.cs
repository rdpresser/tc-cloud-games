using TC.CloudGames.Domain.GameAggregate;

namespace TC.CloudGames.Application.Games.CreateGame
{
    public static class CreateGameMapper
    {
        public static Result<Game> ToEntity(CreateGameCommand command)
        {
            var gameResult = Game.Create(builder =>
            {
                builder.Name = command.Name;
                builder.ReleaseDate = command.ReleaseDate;
                builder.AgeRating = command.AgeRating;
                builder.Description = command.Description;
                builder.DeveloperInfo = (command.DeveloperInfo.Developer, command.DeveloperInfo.Publisher);
                builder.DiskSize = command.DiskSize;
                builder.Price = command.Price;
                builder.Playtime = command.Playtime != null ? (command.Playtime.Hours, command.Playtime.PlayerCount) : null;
                builder.GameDetails = (
                    command.GameDetails.Genre,
                    command.GameDetails.Platform,
                    command.GameDetails.Tags,
                    command.GameDetails.GameMode,
                    command.GameDetails.DistributionFormat,
                    command.GameDetails.AvailableLanguages,
                    command.GameDetails.SupportsDlcs
                );
                builder.SystemRequirements = (command.SystemRequirements.Minimum, command.SystemRequirements.Recommended);
                builder.Rating = command.Rating;
                builder.OfficialLink = command.OfficialLink;
                builder.GameStatus = command.GameStatus;
            });

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
