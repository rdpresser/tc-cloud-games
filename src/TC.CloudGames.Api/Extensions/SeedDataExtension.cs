using Bogus;
using TC.CloudGames.Domain.Game;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Infra.Data;

namespace TC.CloudGames.Api.Extensions
{
    public static class SeedDataExtension
    {
        public static async Task SeedUserData(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (dbContext.Users.Count() >= 100)
            {
                return;
            }

            var faker = new Faker();

            List<User> users = [];
            for (int i = 0; i < 100; i++)
            {
                users.Add(User.Create(
                    faker.Name.FirstName(),
                    faker.Name.LastName(),
                    Email.Create(faker.Internet.Email()),
                    Password.Create(PasswordGenerator.GeneratePassword(8)),
                    Role.Create(faker.PickRandom(Role.ValidRoles.ToArray()))));
            }

            dbContext.Users.AddRange(users);

            await dbContext.SaveChangesAsync();
        }

        public static async Task SeedGameData(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (dbContext.Games.Count() >= 100)
            {
                return;
            }

            var faker = new Faker();

            List<Game> games = [];
            for (int i = 0; i < 100; i++)
            {
                games.Add(
                    Game.Create(
                        name: $"{faker.Commerce.ProductAdjective()} {faker.Commerce.ProductMaterial()} {faker.Commerce.Product()}",
                        releaseDate: DateOnly.FromDateTime(faker.Date.Past()),
                        ageRating: AgeRating.Create(faker.PickRandom(AgeRating.ValidRatings.ToArray())),
                        description: faker.Lorem.Paragraph(),
                        developerInfo: new DeveloperInfo(faker.Company.CompanyName(), faker.Company.CompanyName()),
                        diskSize: new DiskSize(faker.Random.Int(1, 150)),
                        price: new Price(Math.Round(faker.Random.Decimal(1, 500), 2)),
                        playtime: new Playtime(faker.Random.Int(1, 200), faker.Random.Int(1, 2000)),
                        gameDetails:
                            GameDetails.Create(
                                genre: faker.Lorem.Word(),
                                platform: GameDetails.ValidPlatforms.ToArray(),
                                tags: faker.Lorem.Word(),
                                gameMode: faker.PickRandom(GameDetails.ValidGameModes.ToArray()),
                                distributionFormat: faker.PickRandom(GameDetails.ValidDistributionFormats.ToArray()),
                                availableLanguages: faker.Lorem.Word(),
                                supportsDlcs: faker.Random.Bool()),
                        systemRequirements: new SystemRequirements(faker.Lorem.Paragraph(), faker.Lorem.Paragraph()),
                        rating: Rating.Create(Math.Round(faker.Random.Decimal(1, 10), 2)),
                        officialLink: faker.Internet.Url(),
                        gameStatus: faker.PickRandom(Game.ValidGameStatus.ToArray())));
            }
            dbContext.Games.AddRange(games);
            await dbContext.SaveChangesAsync();
        }
    }
}
