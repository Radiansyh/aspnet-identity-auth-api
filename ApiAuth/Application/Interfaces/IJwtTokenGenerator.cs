using System.Security.Claims;

namespace ApiAuth.Application.Interfaces;

/// <summary>
/// Interface for JWT token generation
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a JWT token for the user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="email">User email</param>
    /// <param name="roles">User roles</param>
    /// <returns>JWT token string</returns>
    string GenerateToken(string userId, string email, IList<string> roles);
}
