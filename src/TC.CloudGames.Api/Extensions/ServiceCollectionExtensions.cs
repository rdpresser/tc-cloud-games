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
using TC.CloudGames.Api.Telemetry;
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
        builder.Logging.ClearProviders();

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeScopes = true;
            options.IncludeFormattedMessage = true;

            // Enhanced resource configuration for logs using centralized constants
            options.SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(TelemetryConstants.ServiceName,
                               serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString() ?? TelemetryConstants.Version)
                    .AddAttributes(new Dictionary<string, object>
                    {
                        ["deployment.environment"] = builder.Configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development",
                        ["service.namespace"] = TelemetryConstants.ServiceNamespace,
                        ["cloud.provider"] = "azure",
                        ["cloud.platform"] = "azure_container_apps"
                    }));

            options.AddOtlpExporter();
        });

        // Add structured JSON logging for better parsing in Grafana
        builder.Logging.AddJsonConsole(options =>
        {
            options.IncludeScopes = true;
            options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff ";
            options.JsonWriterOptions = new System.Text.Json.JsonWriterOptions
            {
                Indented = false
            };
        });

        return builder;
    }

    public static IServiceCollection AddCustomOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? TelemetryConstants.Version;
        var environment = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development";
        var instanceId = Environment.MachineName;

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(TelemetryConstants.ServiceName, serviceVersion: serviceVersion, serviceInstanceId: instanceId)
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment"] = environment,
                    ["service.namespace"] = TelemetryConstants.ServiceNamespace,
                    ["service.instance.id"] = instanceId,
                    ["container.name"] = Environment.GetEnvironmentVariable("HOSTNAME") ?? instanceId,
                    ["cloud.provider"] = "azure",
                    ["cloud.platform"] = "azure_container_apps",
                    ["service.team"] = "engineering",
                    ["service.owner"] = "devops"
                }))
            .WithMetrics(metricsBuilder =>
                metricsBuilder
                    // ASP.NET Core and system instrumentation
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation() // CPU, Memory, GC metrics
                    .AddFusionCacheInstrumentation()
                    .AddNpgsqlInstrumentation()
                    // Built-in meters for system metrics
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddMeter("System.Net.Http")
                    .AddMeter("System.Runtime") // .NET runtime metrics
                                                // Custom application meters
                    .AddMeter(TelemetryConstants.GameMeterName) // Custom game metrics
                    .AddMeter(TelemetryConstants.UserMeterName) // Custom user metrics
                                                                // Export to both OTLP (Grafana Cloud) and Prometheus endpoint
                    .AddOtlpExporter()
                    .AddPrometheusExporter()) // Prometheus scraping endpoint
            .WithTracing(tracingBuilder =>
                tracingBuilder
                    .AddHttpClientInstrumentation(options =>
                    {
                        options.FilterHttpRequestMessage = request =>
                        {
                            // Filter out health check and metrics requests
                            var path = request.RequestUri?.AbsolutePath ?? "";
                            return !path.Contains("/health") && !path.Contains("/metrics") && !path.Contains("/prometheus");
                        };
                        options.EnrichWithHttpRequestMessage = (activity, request) =>
                        {
                            activity.SetTag("http.request.method", request.Method.ToString());
                            activity.SetTag("http.request.body.size", request.Content?.Headers?.ContentLength);
                            activity.SetTag("user_agent", request.Headers.UserAgent?.ToString());
                        };
                        options.EnrichWithHttpResponseMessage = (activity, response) =>
                        {
                            activity.SetTag("http.response.status_code", (int)response.StatusCode);
                            activity.SetTag("http.response.body.size", response.Content?.Headers?.ContentLength);
                        };
                    })
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.Filter = httpContext =>
                        {
                            // Filter out health check, metrics, and prometheus requests
                            var path = httpContext.Request.Path.Value ?? "";
                            return !path.Contains("/health") && !path.Contains("/metrics") && !path.Contains("/prometheus");
                        };
                        options.EnrichWithHttpRequest = (activity, request) =>
                        {
                            activity.SetTag("http.method", request.Method);
                            activity.SetTag("http.scheme", request.Scheme);
                            activity.SetTag("http.host", request.Host.Value);
                            activity.SetTag("http.target", request.Path);
                            if (request.ContentLength.HasValue)
                                activity.SetTag("http.request_content_length", request.ContentLength.Value);

                            activity.SetTag("http.request.size", request.ContentLength);
                            activity.SetTag("user.id", request.HttpContext.User?.Identity?.Name);
                            activity.SetTag("user.authenticated", request.HttpContext.User?.Identity?.IsAuthenticated);
                            activity.SetTag("http.route", request.HttpContext.GetRouteValue("action")?.ToString());
                            activity.SetTag("http.client_ip", request.HttpContext.Connection.RemoteIpAddress?.ToString());

                            if (request.Headers.TryGetValue(TelemetryConstants.CorrelationIdHeader, out var correlationId))
                            {
                                activity.SetTag("correlation.id", correlationId.FirstOrDefault());
                            }
                        };
                        options.EnrichWithHttpResponse = (activity, response) =>
                        {
                            activity.SetTag("http.status_code", response.StatusCode);
                            if (response.ContentLength.HasValue)
                                activity.SetTag("http.response_content_length", response.ContentLength.Value);

                            activity.SetTag("http.response.size", response.ContentLength);
                        };

                        options.EnrichWithException = (activity, exception) =>
                        {
                            activity.SetTag("exception.type", exception.GetType().Name);
                            activity.SetTag("exception.message", exception.Message);
                            activity.SetTag("exception.stacktrace", exception.StackTrace);
                        };
                    })
                    .AddEntityFrameworkCoreInstrumentation(options =>
                    {
                        options.SetDbStatementForText = true;
                        options.SetDbStatementForStoredProcedure = true;
                        options.EnrichWithIDbCommand = (activity, dbCommand) =>
                        {
                            activity.SetTag("db.operation", dbCommand.CommandText?.Split(' ').FirstOrDefault()?.ToUpper());

                            // RESTORED: Enhanced DB enrichment from previous version
                            activity.SetTag("db.statement", dbCommand.CommandText);
                            activity.SetTag("db.operation", dbCommand.CommandType.ToString());
                            activity.SetTag("db.row_count", dbCommand.Parameters?.Count);
                        };
                    })
                    // RESTORED: Missing instrumentation from previous version
                    .AddFusionCacheInstrumentation()
                    .AddNpgsql()
                    .AddRedisInstrumentation()
                    .AddSource(TelemetryConstants.GameActivitySource)
                    .AddSource(TelemetryConstants.UserActivitySource)
                    .AddSource(TelemetryConstants.DatabaseActivitySource)
                    .AddSource(TelemetryConstants.CacheActivitySource)
                    .AddOtlpExporter());

        // Register custom metrics classes
        services.AddSingleton<GameMetrics>();
        services.AddSingleton<UserMetrics>();
        services.AddSingleton<SystemMetrics>();

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
                // Add custom telemetry services
                .AddSingleton<GameMetrics>()
                .AddSingleton<UserMetrics>()
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

    // Health Checks with Enhanced Telemetry
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
                    tags: ["db", "sql", "postgres"])
                .AddTypeActivatedCheck<RedisHealthCheck>("Redis",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: ["cache", "redis"])
                .AddCheck("Memory", () =>
                    {
                        var allocated = GC.GetTotalMemory(false);
                        var mb = allocated / 1024 / 1024;

                        return mb < 1024
                            ? HealthCheckResult.Healthy($"Memory usage: {mb} MB")
                            : HealthCheckResult.Degraded($"High memory usage: {mb} MB");
                    },
                    tags: ["memory", "system"])
                .AddCheck("Custom-Metrics", () =>
                    {
                        // Add any custom health logic for your metrics system
                        return HealthCheckResult.Healthy("Custom metrics are functioning");
                    },
                    tags: ["metrics", "telemetry"]);

        return services;
    }
}
