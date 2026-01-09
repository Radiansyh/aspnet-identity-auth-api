using ApiAuth.API.Helpers;
using ApiAuth.Application.DTOs;
using ApiAuth.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace ApiAuth.API.Controllers;

/// <summary>
/// Authentication and user management controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <returns>Authentication response with JWT token and refresh token</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseWithRefreshToken), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseWithRefreshToken>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var ipAddress = HttpContextHelper.GetIpAddress(HttpContext);
        var userAgent = HttpContextHelper.GetUserAgent(HttpContext);

        var response = await _authService.RegisterAsync(request, ipAddress, userAgent);
        return Ok(response);
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication response with JWT token and refresh token</returns>
    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(AuthResponseWithRefreshToken), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<AuthResponseWithRefreshToken>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var ipAddress = HttpContextHelper.GetIpAddress(HttpContext);
        var userAgent = HttpContextHelper.GetUserAgent(HttpContext);

        var response = await _authService.LoginAsync(request, ipAddress, userAgent);
        return Ok(response);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>New authentication response with tokens</returns>
    [HttpPost("refresh")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(AuthResponseWithRefreshToken), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<AuthResponseWithRefreshToken>> Refresh([FromBody] RefreshTokenRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var ipAddress = HttpContextHelper.GetIpAddress(HttpContext);
        var userAgent = HttpContextHelper.GetUserAgent(HttpContext);

        var response = await _authService.RefreshTokenAsync(request.RefreshToken, ipAddress, userAgent);
        return Ok(response);
    }

    /// <summary>
    /// Logout and revoke refresh tokens
    /// </summary>
    /// <returns>Success message</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Logout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID not found in token" });
        }

        var ipAddress = HttpContextHelper.GetIpAddress(HttpContext);
        var userAgent = HttpContextHelper.GetUserAgent(HttpContext);

        await _authService.LogoutAsync(userId, ipAddress, userAgent);
        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Get current authenticated user information
    /// </summary>
    /// <returns>User information</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> GetMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID not found in token" });
        }

        var user = await _authService.GetUserAsync(userId);
        return Ok(user);
    }

    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    /// <returns>List of all users</returns>
    [HttpGet("admin/users")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<UserResponse>>> GetAllUsers()
    {
        var users = await _authService.GetAllUsersAsync();
        return Ok(users);
    }
}
