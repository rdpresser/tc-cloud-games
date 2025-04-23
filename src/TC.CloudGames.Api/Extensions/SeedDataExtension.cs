using Bogus;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Infra.Data;

namespace TC.CloudGames.Api.Extensions
{
    public static class SeedDataExtension
    {
        private static readonly string[] items = ["Admin", "User"];

        public static async Task SeedData(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var faker = new Faker();

            List<User> users = [];
            for (int i = 0; i < 100; i++)
            {
                users.Add(User.Create(
                    new FirstName(faker.Name.FirstName()),
                    new LastName(faker.Name.LastName()),
                    new Email(faker.Internet.Email()),
                    new Password(PasswordGenerator.GeneratePassword(8)),
                    new Role(faker.PickRandom(items))));
            }

            dbContext.Users.AddRange(users);

            await dbContext.SaveChangesAsync();
        }
    }
}
