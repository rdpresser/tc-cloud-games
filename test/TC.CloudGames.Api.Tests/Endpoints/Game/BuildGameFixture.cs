using TC.CloudGames.Api.Tests.Abstractions;
using TC.CloudGames.Application.Games.CreateGame;

namespace TC.CloudGames.Api.Tests.Endpoints.Game;

public class BuildGameFixture
{
    public static CreateGameCommand BuildCommand(App App)
    {
        return CreateGameCommand.Create(builder =>
        {
            builder.Name = $"{App.Fake.Commerce.ProductAdjective()} {App.Fake.Commerce.ProductMaterial()} {App.Fake.Commerce.Product()}";
            builder.ReleaseDate = DateOnly.FromDateTime(App.Fake.Date.Past());
            builder.AgeRating = App.Fake.PickRandom(App.AgeRatings.ToArray());
            builder.Description = App.Fake.Lorem.Paragraph();
            builder.DeveloperInfo = new(App.Fake.Company.CompanyName(), App.Fake.Company.CompanyName());
            builder.DiskSize = App.Fake.Random.Int(1, 150);
            builder.Price = decimal.Parse(App.Fake.Commerce.Price(1.0m, 500.0m));
            builder.Playtime = new(App.Fake.Random.Int(1, 200), App.Fake.Random.Int(1, 2000));
            builder.GameDetails = new(
                Genre: App.Fake.PickRandom(App.Genres),
                Platform: [.. App.Fake.PickRandom(App.Platforms, 3)],
                Tags: App.Fake.PickRandom(App.GameTags),
                GameMode: App.Fake.PickRandom(App.GameModes),
                DistributionFormat: App.Fake.PickRandom(App.DistributionFormats),
                AvailableLanguages: App.Fake.PickRandom(App.Languages),
                SupportsDlcs: true);
            builder.SystemRequirements = new(App.Fake.Lorem.Paragraph(), App.Fake.Lorem.Paragraph());
            builder.Rating = Math.Round(App.Fake.Random.Decimal(1, 10), 2);
            builder.OfficialLink = App.Fake.Internet.Url();
            builder.GameStatus = App.Fake.PickRandom(App.GameStatus);
        });
    }

    public static CreateGameResponse BuildResponse(CreateGameCommand command)
    {
        return new CreateGameResponse(
            Id: Guid.NewGuid(),
            Name: command.Name,
            ReleaseDate: command.ReleaseDate,
            AgeRating: command.AgeRating,
            Description: command.Description,
            DeveloperInfo: new(command.DeveloperInfo.Developer, command.DeveloperInfo.Publisher),
            DiskSize: command.DiskSize,
            Price: command.Price,
            Playtime: new(command.Playtime!.Hours, command.Playtime.PlayerCount),
            GameDetails: new(
                Genre: command.GameDetails.Genre,
                Platform: command.GameDetails.Platform,
                Tags: command.GameDetails.Tags,
                GameMode: command.GameDetails.GameMode,
                DistributionFormat: command.GameDetails.DistributionFormat,
                AvailableLanguages: command.GameDetails.AvailableLanguages,
                SupportsDlcs: command.GameDetails.SupportsDlcs),
            SystemRequirements: new(command.SystemRequirements.Minimum, command.SystemRequirements.Recommended),
            Rating: command.Rating,
            OfficialLink: command.OfficialLink,
            GameStatus: command.GameStatus
        );
    }
}