using ApiAuth.Application.DTOs;
using ApiAuth.Application.Interfaces;
using ApiAuth.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApiAuth.Application.Services;

/// <summary>
/// Service handling authentication operations
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IAuditLogService _auditLogService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenService refreshTokenService,
        IAuditLogService auditLogService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenService = refreshTokenService;
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// Registers a new user with the User role
    /// </summary>
    public async Task<AuthResponseWithRefreshToken> RegisterAsync(RegisterRequest request, string ipAddress, string userAgent)
    {
        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        // Create new user
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create user: {errors}");
        }

        // Assign default User role
        await _userManager.AddToRoleAsync(user, "User");

        // Generate tokens
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenGenerator.GenerateToken(user.Id, user.Email!, roles);
        var refreshToken = _refreshTokenService.GenerateRefreshToken();
        
        await _refreshTokenService.StoreRefreshTokenAsync(user.Id, refreshToken);
        await _auditLogService.LogLoginSuccessAsync(user.Id, user.Email!, ipAddress, userAgent);

        return new AuthResponseWithRefreshToken
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Token = accessToken,
            RefreshToken = refreshToken,
            Roles = roles.ToList()
        };
    }

    /// <summary>
    /// Authenticates user and generates JWT token
    /// </summary>
    public async Task<AuthResponseWithRefreshToken> LoginAsync(LoginRequest request, string ipAddress, string userAgent)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            await _auditLogService.LogLoginFailedAsync(request.Email, ipAddress, userAgent);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            await _auditLogService.LogLoginFailedAsync(request.Email, ipAddress, userAgent);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenGenerator.GenerateToken(user.Id, user.Email!, roles);
        var refreshToken = _refreshTokenService.GenerateRefreshToken();
        
        await _refreshTokenService.StoreRefreshTokenAsync(user.Id, refreshToken);
        await _auditLogService.LogLoginSuccessAsync(user.Id, user.Email!, ipAddress, userAgent);

        return new AuthResponseWithRefreshToken
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Token = accessToken,
            RefreshToken = refreshToken,
            Roles = roles.ToList()
        };
    }

    /// <summary>
    /// Refreshes access token using refresh token
    /// </summary>
    public async Task<AuthResponseWithRefreshToken> RefreshTokenAsync(string refreshToken, string ipAddress, string userAgent)
    {
        var validatedToken = await _refreshTokenService.ValidateRefreshTokenAsync(refreshToken);
        if (validatedToken == null)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        var user = validatedToken.User;
        var roles = await _userManager.GetRolesAsync(user);
        
        var newAccessToken = _jwtTokenGenerator.GenerateToken(user.Id, user.Email!, roles);
        var newRefreshToken = _refreshTokenService.GenerateRefreshToken();
        
        await _refreshTokenService.StoreRefreshTokenAsync(user.Id, newRefreshToken);
        await _auditLogService.LogRefreshTokenAsync(user.Id, user.Email!, ipAddress, userAgent);

        return new AuthResponseWithRefreshToken
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Token = newAccessToken,
            RefreshToken = newRefreshToken,
            Roles = roles.ToList()
        };
    }

    /// <summary>
    /// Logs out user and revokes all refresh tokens
    /// </summary>
    public async Task LogoutAsync(string userId, string ipAddress, string userAgent)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        await _refreshTokenService.RevokeAllUserTokensAsync(userId);
        await _auditLogService.LogLogoutAsync(userId, user.Email!, ipAddress, userAgent);
    }

    /// <summary>
    /// Gets current user information by user ID
    /// </summary>
    public async Task<UserResponse> GetUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new UserResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles.ToList()
        };
    }

    /// <summary>
    /// Gets all users with their roles (Admin only)
    /// </summary>
    public async Task<List<UserResponse>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var userResponses = new List<UserResponse>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userResponses.Add(new UserResponse
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Roles = roles.ToList()
            });
        }

        return userResponses;
    }
}
