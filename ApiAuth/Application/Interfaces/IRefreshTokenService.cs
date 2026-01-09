using ApiAuth.Domain;

namespace ApiAuth.Application.Interfaces;

/// <summary>
/// Interface for refresh token operations
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    /// Generates a new refresh token
    /// </summary>
    /// <returns>Refresh token string</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Stores refresh token in database and revokes old tokens
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="token">Refresh token</param>
    /// <param name="expiryDays">Token expiry in days</param>
    Task StoreRefreshTokenAsync(string userId, string token, int expiryDays = 7);

    /// <summary>
    /// Validates and retrieves refresh token
    /// </summary>
    /// <param name="token">Refresh token</param>
    /// <returns>RefreshToken entity if valid, null otherwise</returns>
    Task<RefreshToken?> ValidateRefreshTokenAsync(string token);

    /// <summary>
    /// Revokes refresh token
    /// </summary>
    /// <param name="token">Refresh token to revoke</param>
    Task RevokeRefreshTokenAsync(string token);

    /// <summary>
    /// Revokes all refresh tokens for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    Task RevokeAllUserTokensAsync(string userId);
}
