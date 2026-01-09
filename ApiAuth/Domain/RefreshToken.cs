namespace ApiAuth.Domain;

/// <summary>
/// Refresh token entity for maintaining user sessions
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User ID this token belongs to
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token value
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration date
    /// </summary>
    public DateTime ExpiryDate { get; set; }

    /// <summary>
    /// Indicates if token has been revoked
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// Token creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Navigation property to user
    /// </summary>
    public ApplicationUser User { get; set; } = null!;
}
