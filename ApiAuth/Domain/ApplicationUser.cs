using Microsoft.AspNetCore.Identity;

namespace ApiAuth.Domain;

/// <summary>
/// Custom application user extending IdentityUser with additional properties
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Full name of the user
    /// </summary>
    public string FullName { get; set; } = string.Empty;
}
