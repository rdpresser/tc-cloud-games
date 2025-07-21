using System.Diagnostics.Metrics;
using System.Diagnostics.CodeAnalysis;

namespace TC.CloudGames.Api.Telemetry;

[ExcludeFromCodeCoverage]
public class UserMetrics
{
    // Counters for user actions
    private readonly Counter<long> _userLogins;
    private readonly Counter<long> _userLogouts;
    private readonly Counter<long> _userRegistrations;
    private readonly Counter<long> _userActions;
    
    // Histograms for user behavior
    private readonly Histogram<double> _sessionDuration;
    
    // Up-down counters for current state
    private readonly UpDownCounter<long> _activeUsers;

    public UserMetrics()
    {
        var meter = new Meter(TelemetryConstants.UserMeterName, TelemetryConstants.Version);
        
        _userLogins = meter.CreateCounter<long>(
            "user_logins_total",
            description: "Total number of user logins");
            
        _userLogouts = meter.CreateCounter<long>(
            "user_logouts_total",
            description: "Total number of user logouts");
            
        _userRegistrations = meter.CreateCounter<long>(
            "user_registrations_total",
            description: "Total number of user registrations");
            
        _userActions = meter.CreateCounter<long>(
            "user_actions_total",
            description: "Total number of user actions");
            
        _sessionDuration = meter.CreateHistogram<double>(
            "user_session_duration_seconds",
            unit: "s",
            description: "Duration of user sessions");
            
        _activeUsers = meter.CreateUpDownCounter<long>(
            "active_users",
            description: "Number of currently active users");
    }

    /// <summary>
    /// Records when a user logs in
    /// </summary>
    public void RecordUserLogin(string userId, string sessionId) =>
        _userLogins.Add(1, new KeyValuePair<string, object?>[]
        {
            new(TelemetryConstants.UserId, userId),
            new(TelemetryConstants.SessionId, sessionId)
        });

    /// <summary>
    /// Records when a user logs out
    /// </summary>
    public void RecordUserLogout(string userId, string sessionId, double sessionDurationSeconds)
    {
        _userLogouts.Add(1, new KeyValuePair<string, object?>[]
        {
            new(TelemetryConstants.UserId, userId),
            new(TelemetryConstants.SessionId, sessionId)
        });
        
        _sessionDuration.Record(sessionDurationSeconds, new KeyValuePair<string, object?>[]
        {
            new(TelemetryConstants.UserId, userId)
        });
    }

    /// <summary>
    /// Records user registration
    /// </summary>
    public void RecordUserRegistration(string userId, string registrationMethod = "email") =>
        _userRegistrations.Add(1, new KeyValuePair<string, object?>[]
        {
            new(TelemetryConstants.UserId, userId),
            new("registration_method", registrationMethod)
        });

    /// <summary>
    /// Records general user actions
    /// </summary>
    public void RecordUserAction(string action, string userId, string details = "") =>
        _userActions.Add(1, new KeyValuePair<string, object?>[]
        {
            new(TelemetryConstants.UserAction, action),
            new(TelemetryConstants.UserId, userId),
            new("action_details", details)
        });

    /// <summary>
    /// Increments active users counter
    /// </summary>
    public void UserConnected(string userId) =>
        _activeUsers.Add(1, new KeyValuePair<string, object?>[]
        {
            new(TelemetryConstants.UserId, userId)
        });

    /// <summary>
    /// Decrements active users counter
    /// </summary>
    public void UserDisconnected(string userId) =>
        _activeUsers.Add(-1, new KeyValuePair<string, object?>[]
        {
            new(TelemetryConstants.UserId, userId)
        });
}