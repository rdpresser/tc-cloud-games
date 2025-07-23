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

            // Set activity tags that are always needed
            activity?.SetTag("http.status_code", context.Response.StatusCode);
            activity?.SetTag("http.duration_ms", stopwatch.ElapsedMilliseconds);

            // Handle different status code ranges with appropriate logging and activity status
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
            {
                // Success responses (2xx)
                activity?.SetStatus(ActivityStatusCode.Ok);
                _logger.LogDebug("Request {Method} {Path} completed successfully in {Duration}ms with status {StatusCode} for user {UserId} with correlation {CorrelationId}",
                    context.Request.Method, path, stopwatch.ElapsedMilliseconds, context.Response.StatusCode, userId, correlationId);
            }
            else if (context.Response.StatusCode >= 300 && context.Response.StatusCode < 400)
            {
                // Redirection responses (3xx) - Usually not errors
                activity?.SetStatus(ActivityStatusCode.Ok);
                _logger.LogInformation("Request {Method} {Path} redirected in {Duration}ms with status {StatusCode} for user {UserId} with correlation {CorrelationId}",
                    context.Request.Method, path, stopwatch.ElapsedMilliseconds, context.Response.StatusCode, userId, correlationId);
            }
            else if (context.Response.StatusCode == 400)
            {
                // Bad Request - Client error but not necessarily an application error
                activity?.SetStatus(ActivityStatusCode.Error, "Bad Request");
                _logger.LogWarning("Request {Method} {Path} completed with bad request in {Duration}ms with status {StatusCode} for user {UserId} with correlation {CorrelationId}",
                    context.Request.Method, path, stopwatch.ElapsedMilliseconds, context.Response.StatusCode, userId, correlationId);
            }
            else if (context.Response.StatusCode == 401)
            {
                // Unauthorized - Security related, should be logged as warning
                activity?.SetStatus(ActivityStatusCode.Error, "Unauthorized");
                _logger.LogWarning("Unauthorized request {Method} {Path} completed in {Duration}ms with status {StatusCode} for user {UserId} with correlation {CorrelationId}",
                    context.Request.Method, path, stopwatch.ElapsedMilliseconds, context.Response.StatusCode, userId, correlationId);
            }
            else if (context.Response.StatusCode == 403)
            {
                // Forbidden - Security related, should be logged as warning
                activity?.SetStatus(ActivityStatusCode.Error, "Forbidden");
                _logger.LogWarning("Forbidden request {Method} {Path} completed in {Duration}ms with status {StatusCode} for user {UserId} with correlation {CorrelationId}",
                    context.Request.Method, path, stopwatch.ElapsedMilliseconds, context.Response.StatusCode, userId, correlationId);
            }
            else if (context.Response.StatusCode == 404)
            {
                // Not Found - Usually not an application error, more informational
                activity?.SetStatus(ActivityStatusCode.Error, "Not Found");
                _logger.LogInformation("Request {Method} {Path} not found in {Duration}ms with status {StatusCode} for user {UserId} with correlation {CorrelationId}",
                    context.Request.Method, path, stopwatch.ElapsedMilliseconds, context.Response.StatusCode, userId, correlationId);
            }
            else if (context.Response.StatusCode == 422)
            {
                // Unprocessable Entity - Validation errors
                activity?.SetStatus(ActivityStatusCode.Error, "Unprocessable Entity");
                _logger.LogWarning("Request {Method} {Path} validation failed in {Duration}ms with status {StatusCode} for user {UserId} with correlation {CorrelationId}",
                    context.Request.Method, path, stopwatch.ElapsedMilliseconds, context.Response.StatusCode, userId, correlationId);
            }
            else if (context.Response.StatusCode >= 400 && context.Response.StatusCode < 500)
            {
                // Other client errors (4xx)
                activity?.SetStatus(ActivityStatusCode.Error, "Client Error");
                _logger.LogWarning("Request {Method} {Path} client error in {Duration}ms with status {StatusCode} for user {UserId} with correlation {CorrelationId}",
                    context.Request.Method, path, stopwatch.ElapsedMilliseconds, context.Response.StatusCode, userId, correlationId);
            }
            else if (context.Response.StatusCode >= 500)
            {
                // Server errors (5xx) - These are actual application errors
                activity?.SetStatus(ActivityStatusCode.Error, "Server Error");
                _logger.LogError("Request {Method} {Path} server error in {Duration}ms with status {StatusCode} for user {UserId} with correlation {CorrelationId}",
                    context.Request.Method, path, stopwatch.ElapsedMilliseconds, context.Response.StatusCode, userId, correlationId);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("error.type", ex.GetType().Name);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetTag("http.status_code", context.Response.StatusCode);
            activity?.SetTag("http.duration_ms", stopwatch.ElapsedMilliseconds);

            _logger.LogError(ex, "Request {Method} {Path} failed after {Duration}ms for user {UserId} with correlation {CorrelationId}. Exception: {ExceptionType}",
                context.Request.Method, path, stopwatch.ElapsedMilliseconds, userId, correlationId, ex.GetType().Name);

            // Re-throw the exception
            throw;
        }
    }
}