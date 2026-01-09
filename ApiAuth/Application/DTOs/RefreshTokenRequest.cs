using System.ComponentModel.DataAnnotations;

namespace ApiAuth.Application.DTOs;

/// <summary>
/// Request model for refresh token
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    /// Refresh token value
    /// </summary>
    [Required(ErrorMessage = "Refresh token is required")]
    public string RefreshToken { get; set; } = string.Empty;
}
