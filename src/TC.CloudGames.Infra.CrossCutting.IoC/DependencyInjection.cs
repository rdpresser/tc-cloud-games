using Dapper;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Domain.Abstractions;
using TC.CloudGames.Domain.Exceptions;
using TC.CloudGames.Domain.GameAggregate;
using TC.CloudGames.Domain.GameAggregate.Abstractions;
using TC.CloudGames.Domain.UserAggregate.Abstractions;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;
using TC.CloudGames.Infra.CrossCutting.Commons.Clock;
using TC.CloudGames.Infra.Data;
using TC.CloudGames.Infra.Data.Configurations.Connection;
using TC.CloudGames.Infra.Data.Configurations.Data;
using TC.CloudGames.Infra.Data.Exceptions;
using TC.CloudGames.Infra.Data.Repositories.EfCore;
using TC.CloudGames.Infra.Data.Repositories.PostgreSql;

namespace TC.CloudGames.Infra.CrossCutting.IoC
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
        {
            return services
                .RegisterInfra()
                .RegisterDomain()
                .RegisterApplication();
        }

        private static IServiceCollection RegisterInfra(this IServiceCollection services)
        {
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<IConnectionStringProvider, ConnectionStringProvider>();
            services.AddSingleton<IPgDbConnectionProvider, PgDbConnectionProvider>();
            services.AddSingleton<ITokenProvider, TokenProvider>();
            services.AddScoped<IUserContext, UserContext>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            services.AddDbContext<ApplicationDbContext>(contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Scoped);
            SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

            return services;
        }

        private static IServiceCollection RegisterDomain(this IServiceCollection services)
        {
            services.AddSingleton<IDuplicateKeyViolation, PostgresDuplicateKeyException>();
            services.AddScoped<IUserEfRepository, UserEfRepository>();
            services.AddScoped<IGameEfRepository, GameEfRepository>();
            services.AddScoped<IUnitOfWork, ApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IBaseValidator<Game>, CreateGameValidator>();

            return services;
        }

        private static IServiceCollection RegisterApplication(this IServiceCollection services)
        {
            services.AddScoped<IUserPgRepository, UserPgRepository>();
            services.AddScoped<IGamePgRepository, GamePgRepository>();

            return services;
        }
    }
}
