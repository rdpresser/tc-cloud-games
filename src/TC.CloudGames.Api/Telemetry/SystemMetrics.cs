using System.Diagnostics.Metrics;
using System.Diagnostics.CodeAnalysis;

namespace TC.CloudGames.Api.Telemetry;

/// <summary>
/// System-level metrics for infrastructure monitoring
/// </summary>
[ExcludeFromCodeCoverage]
public class SystemMetrics
{
    // System counters
    private readonly Counter<long> _requestsTotal;
    private readonly Counter<long> _errorsTotal;
    
    // System histograms
    private readonly Histogram<double> _requestDuration;
    private readonly Histogram<double> _databaseQueryDuration;
    
    // System gauges (using UpDownCounter)
    private readonly UpDownCounter<long> _activeConnections;

    public SystemMetrics()
    {
        var meter = new Meter("TC.CloudGames.System.Metrics", TelemetryConstants.Version);
        
        _requestsTotal = meter.CreateCounter<long>(
            "http_requests_total",
            description: "Total number of HTTP requests");
            
        _errorsTotal = meter.CreateCounter<long>(
            "http_errors_total",
            description: "Total number of HTTP errors");
            
        _requestDuration = meter.CreateHistogram<double>(
            "http_request_duration_seconds",
            unit: "s",
            description: "Duration of HTTP requests");
            
        _databaseQueryDuration = meter.CreateHistogram<double>(
            "database_query_duration_seconds",
            unit: "s", 
            description: "Duration of database queries");
            
        _activeConnections = meter.CreateUpDownCounter<long>(
            "active_connections",
            description: "Number of active connections");
    }

    /// <summary>
    /// Records HTTP request
    /// </summary>
    public void RecordHttpRequest(string method, string path, int statusCode, double durationSeconds)
    {
        _requestsTotal.Add(1, new KeyValuePair<string, object?>[]
        {
            new("method", method),
            new("path", path),
            new("status_code", statusCode.ToString())
        });
        
        _requestDuration.Record(durationSeconds, new KeyValuePair<string, object?>[]
        {
            new("method", method),
            new("status_code", statusCode.ToString())
        });
        
        if (statusCode >= 400)
        {
            _errorsTotal.Add(1, new KeyValuePair<string, object?>[]
            {
                new("method", method),
                new("status_code", statusCode.ToString()),
                new("error_type", statusCode >= 500 ? "server_error" : "client_error")
            });
        }
    }

    /// <summary>
    /// Records database query duration
    /// </summary>
    public void RecordDatabaseQuery(string operation, double durationSeconds) =>
        _databaseQueryDuration.Record(durationSeconds, new KeyValuePair<string, object?>[]
        {
            new("operation", operation)
        });

    /// <summary>
    /// Connection opened
    /// </summary>
    public void ConnectionOpened() => _activeConnections.Add(1);

    /// <summary>
    /// Connection closed
    /// </summary>
    public void ConnectionClosed() => _activeConnections.Add(-1);
}