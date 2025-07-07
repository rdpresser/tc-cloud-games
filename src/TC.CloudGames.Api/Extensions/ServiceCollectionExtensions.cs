using FluentValidation;
using FluentValidation.Resources;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json.Converters;
using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics.CodeAnalysis;
using TC.CloudGames.Domain.Aggregates.Game.Abstractions;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;
using TC.CloudGames.Infra.CrossCutting.Commons.Caching;
using TC.CloudGames.Infra.CrossCutting.Commons.Extensions;
using TC.CloudGames.Infra.CrossCutting.IoC;
using TC.CloudGames.Infra.Data.Configurations.Connection;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace TC.CloudGames.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    // FluentValidation Global Setup
    public static void ConfigureFluentValidationGlobals()
    {
        ValidatorOptions.Global.PropertyNameResolver = (type, memberInfo, expression) => memberInfo?.Name;
        ValidatorOptions.Global.DisplayNameResolver = (type, memberInfo, expression) => memberInfo?.Name;
        ValidatorOptions.Global.ErrorCodeResolver = validator => validator.Name;
        ValidatorOptions.Global.LanguageManager = new LanguageManager
        {
            Enabled = true,
            Culture = new System.Globalization.CultureInfo("en")
        };
    }

    public static WebApplicationBuilder AddCustomLoggingTelemetry(this WebApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeScopes = true;
            options.ParseStateValues = true;
            options.IncludeFormattedMessage = true;
            options.AddOtlpExporter();
        });

        return builder;
    }

    public static IServiceCollection AddCustomOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
                .ConfigureResource(r => r.AddService("tccloudgames-app"))
                .WithTracing(tracingBuilder =>
                {
                    tracingBuilder.AddHttpClientInstrumentation()
                                  .AddAspNetCoreInstrumentation()
                                  .AddEntityFrameworkCoreInstrumentation()
                                  .AddRedisInstrumentation()
                                  .AddFusionCacheInstrumentation(configure =>
                                  {
                                      configure.IncludeDistributedLevel = true;
                                      configure.IncludeMemoryLevel = true;
                                      configure.IncludeBackplane = true;
                                  })
                                  .AddSource("tccloudgames-app")
                                  //.AddSource("TC.CloudGames.Application")
                                  //.AddSource("TC.CloudGames.Domain")
                                  //.AddSource("TC.CloudGames.Infra.CrossCutting.Commons")
                                  //.AddSource("TC.CloudGames.Infra.CrossCutting.IoC")
                                  //.AddSource("TC.CloudGames.Infra.Data")
                                  .AddNpgsql();

                    tracingBuilder.AddOtlpExporter();
                })
                .WithMetrics(metricsBuilder =>
                {
                    metricsBuilder.AddAspNetCoreInstrumentation()
                                  .AddHttpClientInstrumentation()
                                  .AddNpgsqlInstrumentation()
                                  .AddFusionCacheInstrumentation(options =>
                                  {
                                      options.IncludeDistributedLevel = true;
                                      options.IncludeMemoryLevel = true;
                                      options.IncludeBackplane = true;
                                  });

                    metricsBuilder.AddOtlpExporter();
                });

        return services;
    }

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
                .WithDistributedCache(sp =>
                {
                    var cacheProvider = sp.GetRequiredService<ICacheProvider>();

                    var options = new RedisCacheOptions { Configuration = cacheProvider.ConnectionString, InstanceName = cacheProvider.InstanceName };

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
        services.Configure<CacheSettings>(configuration.GetSection("Cache"));

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
