namespace ApiAuth.Application.Interfaces;

/// <summary>
/// Interface for login audit logging
/// </summary>
public interface IAuditLogService
{
    /// <summary>
    /// Logs a successful login
    /// </summary>
    Task LogLoginSuccessAsync(string userId, string email, string ipAddress, string userAgent);

    /// <summary>
    /// Logs a failed login attempt
    /// </summary>
    Task LogLoginFailedAsync(string email, string ipAddress, string userAgent);

    /// <summary>
    /// Logs a refresh token usage
    /// </summary>
    Task LogRefreshTokenAsync(string userId, string email, string ipAddress, string userAgent);

    /// <summary>
    /// Logs a logout action
    /// </summary>
    Task LogLogoutAsync(string userId, string email, string ipAddress, string userAgent);
}
