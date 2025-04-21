using Bogus;
using Dapper;
using TC.CloudGames.Application.Abstractions.Data;

namespace TC.CloudGames.Api.Extensions
{
    public static class SeedDataExtension
    {
        private static readonly string[] items = ["Admin", "User"];

        public static async Task SeedData(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
            using var connection = await sqlConnectionFactory.CreateConnectionAsync();

            var faker = new Faker();

            List<object> users = [];
            for (int i = 0; i < 100; i++)
            {
                users.Add(new
                {
                    Id = Guid.NewGuid(),
                    FirstName = faker.Name.FirstName(),
                    LastName = faker.Name.LastName(),
                    Email = faker.Internet.Email(),
                    Password = PasswordGenerator.GeneratePassword(8),
                    Role = faker.PickRandom(items)
                });
            }

            const string sql = """
                INSERT INTO public.users
                (id, first_name, last_name, email, password, role)
                VALUES(@Id, @FirstName, @LastName, @Email, @Password, @Role);
                """;

            await connection.ExecuteAsync(sql, users);
        }
    }
}
