using FluentValidation;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json.Converters;
using System.Diagnostics.CodeAnalysis;
using TC.CloudGames.Domain.Aggregates.Game.Abstractions;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;
using TC.CloudGames.Infra.CrossCutting.Commons.Extensions;
using TC.CloudGames.Infra.CrossCutting.IoC;
using TC.CloudGames.Infra.Data.Configurations.Connection;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace TC.CloudGames.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    // Authentication and Authorization
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthenticationJwtBearer(s => s.SigningKey = configuration["Jwt:SecretKey"])
                .AddAuthorization()
                .AddHttpContextAccessor();

        return services;
    }

    // FastEndpoints Configuration
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
                o.NewtonsoftSettings = s => { s.Converters.Add(new StringEnumConverter()); };
            });

        return services;
    }

    // Dependency Injection and Caching
    public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDependencyInjection()
                .AddCorrelationIdGenerator()
                .AddHttpClient()
                .AddValidatorsFromAssemblyContaining<CreateGameValidator>()
                .AddFusionCache()
                .WithDefaultEntryOptions(options =>
                {
                    options.Duration = TimeSpan.FromSeconds(20);
                    options.DistributedCacheDuration = TimeSpan.FromSeconds(30);
                })
                .WithDistributedCache(_ =>
                {
                    var connectionString = configuration.GetConnectionString("Cache");
                    var options = new RedisCacheOptions { Configuration = connectionString, InstanceName = "FusionCache" };

                    return new RedisCache(options);
                })
                .WithSerializer(new FusionCacheSystemTextJsonSerializer())
                .AsHybridCache();

        return services;
    }

    // Database Settings Configuration
    public static IServiceCollection ConfigureAppSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseSettings>(configuration.GetSection("Database"));
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

        return services;
    }

    // Health Checks
    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services)
    {
        services.AddHealthChecks()
                .AddNpgSql(sp =>
                    {
                        var connectionProvider = sp.GetRequiredService<IConnectionStringProvider>();
                        return connectionProvider.ConnectionString;
                    },
                    name: "PostgreSQL",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: ["db", "sql", "postgres"]);

        return services;
    }
}
