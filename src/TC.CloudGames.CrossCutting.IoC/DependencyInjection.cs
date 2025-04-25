using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.CrossCutting.Commons.Clock;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Domain.Exceptions;
using TC.CloudGames.Domain.Game;
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
            services.AddSingleton<IDatabaseConnectionProvider, DatabaseConnectionProvider>();
            services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

            services.AddDbContext<ApplicationDbContext>();

            SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        }

        private static void RegisterDomain(IServiceCollection services)
        {
            //
            services.AddSingleton<IDuplicateKeyException, PostgresDuplicateKeyException>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<IUnitOfWork, ApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        }

        private static void RegisterApplication(IServiceCollection services)
        {

        }
    }
}
