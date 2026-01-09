namespace ApiAuth.Domain;

/// <summary>
/// Login audit log entity for tracking authentication events
/// </summary>
public class LoginAuditLog
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User ID (nullable for failed login attempts)
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Email address used in login attempt
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// IP address of the request
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// User agent string from request
    /// </summary>
    public string UserAgent { get; set; } = string.Empty;

    /// <summary>
    /// Action performed (LoginSuccess, LoginFailed, Refresh, Logout)
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp of the event
    /// </summary>
    public DateTime Timestamp { get; set; }
}
