using JasperFx.Events.Projections;
using Marten;
using TC.CloudGames.User.Application.Ports;
using TC.CloudGames.User.Infrastructure.Projections;
using TC.CloudGames.User.Infrastructure.Repositories;

namespace TC.CloudGames.User.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=54320;Database=tc_cloudgames_user;Username=postgres;Password=postgres";

        // Extract maintenance connection string (connects to 'postgres' db for admin tasks)
        var maintenanceConnectionString = connectionString.Replace("Database=tc_cloudgames_user", "Database=postgres");

        services.AddMarten(serviceProvider =>
        {
            var options = new StoreOptions();
            options.Connection(connectionString);

            // Event Store configuration
            options.Events.DatabaseSchemaName = "events";
            // Document database configuration  
            options.DatabaseSchemaName = "documents";
            // Register UserProjection as a document type so its table is created
            options.Schema.For<UserProjection>().DatabaseSchemaName("documents");
            //options.RegisterDocumentType<UserProjection>();
            // Register event projections
            options.Projections.Add<UserProjectionHandler>(ProjectionLifecycle.Inline);

            // Ensure database is created if missing
            options.CreateDatabasesForTenants(c =>
            {
                c.MaintenanceDatabase(maintenanceConnectionString);
                c.ForTenant()
                    .CheckAgainstPgDatabase()
                    .WithOwner("postgres")
                    .WithEncoding("UTF-8")
                    .ConnectionLimit(-1);
            });

            return options;
        })
        .UseLightweightSessions()
        .ApplyAllDatabaseChangesOnStartup();

        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}