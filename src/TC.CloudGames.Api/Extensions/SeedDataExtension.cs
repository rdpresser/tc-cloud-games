using Bogus;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Infra.Data;

namespace TC.CloudGames.Api.Extensions
{
    public static class SeedDataExtension
    {
        public static async Task SeedData(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var faker = new Faker();

            List<User> users = [];
            for (int i = 0; i < 100; i++)
            {
                users.Add(User.Create(
                    faker.Name.FirstName(),
                    faker.Name.LastName(),
                    Email.Create(faker.Internet.Email()),
                    Password.Create(PasswordGenerator.GeneratePassword(8)),
                    Role.Create(faker.PickRandom(Role.AllowedRoles))));
            }

            dbContext.Users.AddRange(users);

            await dbContext.SaveChangesAsync();
        }
    }
}
