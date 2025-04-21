using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Application.Users.CreateUser;
using TC.CloudGames.CrossCutting.Commons.Clock;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Domain.Exceptions;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Infra.Data;
using TC.CloudGames.Infra.Data.Configurations.Data;
using TC.CloudGames.Infra.Data.Exceptions;
using TC.CloudGames.Infra.Data.Repositories;

namespace TC.CloudGames.CrossCutting.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterInfra(services, configuration);
            RegisterDomain(services);
            RegisterApplication(services);

            return services;
        }

        private static void RegisterInfra(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();

            var connectionString =
                configuration.GetConnectionString("Database") ??
                throw new ArgumentNullException(nameof(configuration), "Connection string 'Database' not found.");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
                options.EnableSensitiveDataLogging(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development");
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, ApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
            services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionString));
            SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        }

        private static void RegisterDomain(IServiceCollection services)
        {
            //
            services.AddSingleton<IDuplicateKeyException, PostgresDuplicateKeyException>();
        }

        private static void RegisterApplication(IServiceCollection services)
        {
            services.AddSingleton(sp => new CreateUserMapper());
        }
    }
}
