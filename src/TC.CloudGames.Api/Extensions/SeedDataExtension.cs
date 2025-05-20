using Bogus;
using System.Diagnostics.CodeAnalysis;
using TC.CloudGames.Domain.Game;
using TC.CloudGames.Domain.Game.Abstractions;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Domain.User.Abstractions;
using TC.CloudGames.Infra.CrossCutting.Commons.Extensions;

namespace TC.CloudGames.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class SeedDataExtension
{
    private static readonly string[] AvailableLanguagesList = ["English", "Spanish", "French", "German", "Japanese"];

    /// <summary>
    /// Seeds the database with user data if it is empty.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static async Task SeedUserData(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserEfRepository>();

        if (dbContext.Users.Count() >= 100) return;

        var faker = new Faker();

        List<User> users = [];

        //Create default users
        users.Add(await User.CreateAsync(
                "Admin",
                "User",
                "admin@admin.com",
                "Admin@123",
                "Admin",
                userRepository).ConfigureAwait(false));

        users.Add(await User.CreateAsync(
                "Regular",
                "User",
                "user@user.com",
                "User@123",
                "User",
                userRepository).ConfigureAwait(false));

        for (var i = 0; i < 100; i++)
        {
            var newUser = await User.CreateAsync(
                faker.Name.FirstName().OnlyLetters(),
                faker.Name.LastName().OnlyLetters(),
                faker.Internet.Email(),
                PasswordGenerator.GeneratePassword(),
                faker.PickRandom(Role.ValidRoles.ToArray()),
                userRepository).ConfigureAwait(false);

            if (!newUser.IsSuccess) continue;

            users.Add(newUser);
        }

        await userRepository.BulkInsertAsync(users).ConfigureAwait(false);
    }

    /// <summary>
    /// Seeds the database with game data if it is empty.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static async Task SeedGameData(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var gameRepository = scope.ServiceProvider.GetRequiredService<IGameEfRepository>();

        if (dbContext.Games.Count() >= 100) return;

        var faker = new Faker();

        List<Game> games = [];
        for (var i = 0; i < 100; i++)
        {
            var newGame = Game.Create(builder =>
            {
                builder.Name = $"{faker.Commerce.ProductAdjective()} {faker.Commerce.ProductMaterial()} {faker.Commerce.Product()}";
                builder.ReleaseDate = DateOnly.FromDateTime(faker.Date.Past());
                builder.AgeRating = faker.PickRandom(AgeRating.ValidRatings.ToArray());
                builder.Description = faker.Lorem.Paragraph();
                builder.DeveloperInfo = (faker.Company.CompanyName(), faker.Company.CompanyName());
                builder.DiskSize = faker.Random.Int(1, 150);
                builder.Price = decimal.Parse(faker.Commerce.Price(1.0m, 500.0m));
                builder.Playtime = (faker.Random.Int(1, 200), faker.Random.Int(1, 2000));
                builder.GameDetails = (
                    genre: faker.Lorem.Word(),
                    platform: GameDetails.ValidPlatforms.ToArray(),
                    tags: faker.Lorem.Word(),
                    gameMode: faker.PickRandom(GameDetails.ValidGameModes.ToArray()),
                    distributionFormat: faker.PickRandom(GameDetails.ValidDistributionFormats.ToArray()),
                    availableLanguages: string.Join(", ", faker.Random.ListItems(AvailableLanguagesList, faker.Random.Int(1, AvailableLanguagesList.Length))),
                    supportsDlcs: faker.Random.Bool());
                builder.SystemRequirements = (faker.Lorem.Paragraph(), faker.Lorem.Paragraph());
                builder.Rating = Math.Round(faker.Random.Decimal(1, 10), 2);
                builder.OfficialLink = faker.Internet.Url();
                builder.GameStatus = faker.PickRandom(Game.ValidGameStatus.ToArray());
            });

            if (!newGame.IsSuccess) continue;

            games.Add(newGame);
        }

        await gameRepository.BulkInsertAsync(games);
    }
}