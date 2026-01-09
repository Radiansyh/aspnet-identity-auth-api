namespace ApiAuth.Application.DTOs;

/// <summary>
/// Response model for user information
/// </summary>
public class UserResponse
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
    /// List of user roles
    /// </summary>
    public List<string> Roles { get; set; } = new();
}
