namespace ApiAuth.Application.DTOs;

/// <summary>
/// Response model for authentication operations
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// User ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// User's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// JWT access token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// List of user roles
    /// </summary>
    public List<string> Roles { get; set; } = new();
}
