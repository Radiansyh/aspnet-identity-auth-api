namespace ApiAuth.Application.DTOs;

/// <summary>
/// Response model including refresh token
/// </summary>
public class AuthResponseWithRefreshToken : AuthResponse
{
    /// <summary>
    /// Refresh token for obtaining new access tokens
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}
