using ApiAuth.Application.DTOs;

namespace ApiAuth.Application.Interfaces;

/// <summary>
/// Interface for authentication service operations
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="request">Registration request containing user details</param>
    /// <param name="ipAddress">Client IP address</param>
    /// <param name="userAgent">Client user agent</param>
    /// <returns>Authentication response with JWT token and refresh token</returns>
    Task<AuthResponseWithRefreshToken> RegisterAsync(RegisterRequest request, string ipAddress, string userAgent);

    /// <summary>
    /// Authenticates a user and generates JWT token
    /// </summary>
    /// <param name="request">Login request containing credentials</param>
    /// <param name="ipAddress">Client IP address</param>
    /// <param name="userAgent">Client user agent</param>
    /// <returns>Authentication response with JWT token and refresh token</returns>
    Task<AuthResponseWithRefreshToken> LoginAsync(LoginRequest request, string ipAddress, string userAgent);

    /// <summary>
    /// Refreshes access token using refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <param name="ipAddress">Client IP address</param>
    /// <param name="userAgent">Client user agent</param>
    /// <returns>New authentication response with tokens</returns>
    Task<AuthResponseWithRefreshToken> RefreshTokenAsync(string refreshToken, string ipAddress, string userAgent);

    /// <summary>
    /// Logs out user and revokes refresh tokens
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="ipAddress">Client IP address</param>
    /// <param name="userAgent">Client user agent</param>
    Task LogoutAsync(string userId, string ipAddress, string userAgent);

    /// <summary>
    /// Gets current user information
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User information</returns>
    Task<UserResponse> GetUserAsync(string userId);

    /// <summary>
    /// Gets all users with their roles (Admin only)
    /// </summary>
    /// <returns>List of all users</returns>
    Task<List<UserResponse>> GetAllUsersAsync();
}
