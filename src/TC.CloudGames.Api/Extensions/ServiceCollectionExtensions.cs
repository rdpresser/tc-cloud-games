using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Newtonsoft.Json.Converters;
using TC.CloudGames.Application.Middleware;
using TC.CloudGames.CrossCutting.Commons.Extensions;
using TC.CloudGames.CrossCutting.IoC;
using TC.CloudGames.Infra.Data.Configurations.Connection;
using TC.CloudGames.Infra.Data.Configurations.Data;

namespace TC.CloudGames.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthenticationJwtBearer(s => s.SigningKey = configuration["JwtSecretKey"])
                    .AddAuthorization();

            return services;
        }

        public static IServiceCollection AddCustomFastEndpoints(this IServiceCollection services)
        {
            services.AddFastEndpoints(dicoveryOptions =>
            {
                dicoveryOptions.Assemblies = [typeof(Application.Abstractions.Messaging.ICommand<>).Assembly];
            })
            .SwaggerDocument(o =>
            {
                o.DocumentSettings = s =>
                {
                    s.Title = "TC.CloudGames API";
                    s.Version = "v1";
                    s.Description = "API for TC.CloudGames";
                    s.MarkNonNullablePropsAsRequired();
                };

                o.RemoveEmptyRequestSchema = true;
                o.NewtonsoftSettings = s =>
                {
                    s.Converters.Add(new StringEnumConverter());
                };
            });

            return services;
        }

        public static IServiceCollection AddCustomMiddleware(this IServiceCollection services)
        {
            services.AddCommandMiddleware(c =>
            {
                c.Register(
                    typeof(CommandLogger<,>),
                    typeof(CommandValidator<,>));
            });

            return services;
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDependencyInjection(configuration)
                    .AddCorrelationIdGenerator()
                    .AddHttpClient();

            return services;
        }

        public static IServiceCollection ConfigureDatabaseSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseSettings>(configuration.GetSection("Database"));

            return services;
        }
    }
}