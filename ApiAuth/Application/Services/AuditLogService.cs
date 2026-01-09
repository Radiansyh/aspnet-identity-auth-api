using ApiAuth.Application.Interfaces;
using ApiAuth.Domain;
using ApiAuth.Infrastructure.Persistence;

namespace ApiAuth.Application.Services;

/// <summary>
/// Service for logging authentication audit events
/// </summary>
public class AuditLogService : IAuditLogService
{
    private readonly ApplicationDbContext _context;

    public AuditLogService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Logs successful login
    /// </summary>
    public async Task LogLoginSuccessAsync(string userId, string email, string ipAddress, string userAgent)
    {
        await CreateLogAsync(userId, email, ipAddress, userAgent, "LoginSuccess");
    }

    /// <summary>
    /// Logs failed login attempt
    /// </summary>
    public async Task LogLoginFailedAsync(string email, string ipAddress, string userAgent)
    {
        await CreateLogAsync(null, email, ipAddress, userAgent, "LoginFailed");
    }

    /// <summary>
    /// Logs refresh token usage
    /// </summary>
    public async Task LogRefreshTokenAsync(string userId, string email, string ipAddress, string userAgent)
    {
        await CreateLogAsync(userId, email, ipAddress, userAgent, "Refresh");
    }

    /// <summary>
    /// Logs logout action
    /// </summary>
    public async Task LogLogoutAsync(string userId, string email, string ipAddress, string userAgent)
    {
        await CreateLogAsync(userId, email, ipAddress, userAgent, "Logout");
    }

    private async Task CreateLogAsync(string? userId, string email, string ipAddress, string userAgent, string action)
    {
        var log = new LoginAuditLog
        {
            UserId = userId,
            Email = email,
            IpAddress = ipAddress,
            UserAgent = userAgent ?? string.Empty,
            Action = action,
            Timestamp = DateTime.UtcNow
        };

        _context.LoginAuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
