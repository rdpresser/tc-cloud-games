using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TC.CloudGames.Application.Games;
using TC.CloudGames.Application.Users;
using TC.CloudGames.CrossCutting.Commons.Clock;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Domain.Exceptions;
using TC.CloudGames.Domain.Game;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Infra.Data;
using TC.CloudGames.Infra.Data.Configurations.Connection;
using TC.CloudGames.Infra.Data.Configurations.Data;
using TC.CloudGames.Infra.Data.Exceptions;
using TC.CloudGames.Infra.Data.Repositories;
using TC.CloudGames.Infra.Data.Repositories.EfCore;
using TC.CloudGames.Infra.Data.Repositories.PostgreSql;

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
            services.AddSingleton<IConnectionStringProvider, ConnectionStringProvider>();
            services.AddSingleton<IPgDbConnectionProvider, PgDbConnectionProvider>();

            services.AddDbContext<ApplicationDbContext>();
            SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        }

        private static void RegisterDomain(IServiceCollection services)
        {
            //
            services.AddSingleton<IDuplicateKeyException, PostgresDuplicateKeyException>();
            services.AddScoped<IUserEfRepository, UserEfRepository>();
            services.AddScoped<IGameEfRepository, GameEfRepository>();
            services.AddScoped<IUnitOfWork, ApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        }

        private static void RegisterApplication(IServiceCollection services)
        {
            services.AddScoped<IUserPgRepository, UserPgRepository>();
            services.AddScoped<IGamePgRepository, GamePgRepository>();
        }
    }
}
