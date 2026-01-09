namespace ApiAuth.API.Helpers;

/// <summary>
/// Helper class for extracting client information from HTTP context
/// </summary>
public static class HttpContextHelper
{
    /// <summary>
    /// Gets client IP address from HTTP context
    /// </summary>
    public static string GetIpAddress(HttpContext? context)
    {
        if (context == null)
            return "Unknown";

        var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = context.Connection.RemoteIpAddress?.ToString();
        }

        return ipAddress ?? "Unknown";
    }

    /// <summary>
    /// Gets user agent from HTTP context
    /// </summary>
    public static string GetUserAgent(HttpContext? context)
    {
        if (context == null)
            return "Unknown";

        return context.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";
    }
}
