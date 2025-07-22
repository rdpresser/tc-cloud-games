using System.Diagnostics;
using TC.CloudGames.Api.Telemetry;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;

namespace TC.CloudGames.Api.Middleware;

public class TelemetryMiddleware
{
    private readonly RequestDelegate _next;
    private readonly UserMetrics _userMetrics;
    private readonly ILogger<TelemetryMiddleware> _logger;

    public TelemetryMiddleware(
        RequestDelegate next,
        UserMetrics userMetrics,
        ILogger<TelemetryMiddleware> logger)
    {
        _next = next;
        _userMetrics = userMetrics;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var path = context.Request.Path.Value ?? "";

        // Skip telemetry for health checks and metrics endpoints
        if (path.Contains("/health") || path.Contains("/metrics"))
        {
            await _next(context);
            return;
        }

        // Resolve scoped services from request services
        var userContext = context.RequestServices.GetRequiredService<IUserContext>();
        var correlationIdGenerator = context.RequestServices.GetRequiredService<ICorrelationIdGenerator>();

        // Use IUserContext instead of directly accessing HttpContext.User
        string userId;
        bool isAuthenticated;

        try
        {
            userId = userContext.UserId.ToString();
            isAuthenticated = true;
        }
        catch (InvalidOperationException)
        {
            // User context is unavailable (not authenticated)
            userId = TelemetryConstants.AnonymousUser;
            isAuthenticated = false;
        }

        // Use standardized correlation ID header from constants
        var correlationId = correlationIdGenerator.CorrelationId;

        // Start activity for the request
        using var activity = ActivitySources.StartUserOperation("http_request", userId);
        activity?.SetTag("http.method", context.Request.Method);
        activity?.SetTag("http.path", path);
        activity?.SetTag("user.authenticated", isAuthenticated);
        activity?.SetTag("correlation.id", correlationId);

        // Add additional user context tags when authenticated
        if (isAuthenticated)
        {
            try
            {
                activity?.SetTag("user.id", userContext.UserId.ToString());
                activity?.SetTag("user.name", userContext.UserName);
                activity?.SetTag("user.email", userContext.UserEmail);
                activity?.SetTag("user.role", userContext.UserRole);
            }
            catch (InvalidOperationException)
            {
                // Ignore if user context becomes unavailable during request
            }
        }

        try
        {
            // Track user activity
            if (isAuthenticated)
            {
                _userMetrics.RecordUserAction("http_request", userId, $"{context.Request.Method} {path}");
            }

            await _next(context);

            stopwatch.Stop();

            // Log successful request
            activity?.SetTag("http.status_code", context.Response.StatusCode);
            activity?.SetTag("http.duration_ms", stopwatch.ElapsedMilliseconds);
            activity?.SetStatus(ActivityStatusCode.Ok);

            _logger.LogInformation("Request {Method} {Path} completed in {Duration}ms with status {StatusCode} for user {UserId} with correlation {CorrelationId}",
                context.Request.Method, path, stopwatch.ElapsedMilliseconds, context.Response.StatusCode, userId, correlationId);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("error.type", ex.GetType().Name);
            activity?.SetTag("error.message", ex.Message);

            _logger.LogError(ex, "Request {Method} {Path} failed after {Duration}ms for user {UserId} with correlation {CorrelationId}",
                context.Request.Method, path, stopwatch.ElapsedMilliseconds, userId, correlationId);

            // Re-throw the exception
            throw;
        }
    }
}