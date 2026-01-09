using ApiAuth.Application.Interfaces;
using ApiAuth.Domain;
using ApiAuth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ApiAuth.Application.Services;

/// <summary>
/// Service for managing refresh tokens
/// </summary>
public class RefreshTokenService : IRefreshTokenService
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Generates a cryptographically secure refresh token
    /// </summary>
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    /// <summary>
    /// Stores refresh token and revokes all existing tokens for the user
    /// </summary>
    public async Task StoreRefreshTokenAsync(string userId, string token, int expiryDays = 7)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Revoke all existing tokens for this user
            var existingTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var existingToken in existingTokens)
            {
                existingToken.IsRevoked = true;
            }

            // Create new refresh token
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = token,
                ExpiryDate = DateTime.UtcNow.AddDays(expiryDays),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Validates refresh token and returns it if valid
    /// </summary>
    public async Task<RefreshToken?> ValidateRefreshTokenAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (refreshToken == null)
            return null;

        if (refreshToken.IsRevoked)
            return null;

        if (refreshToken.ExpiryDate < DateTime.UtcNow)
        {
            refreshToken.IsRevoked = true;
            await _context.SaveChangesAsync();
            return null;
        }

        return refreshToken;
    }

    /// <summary>
    /// Revokes a specific refresh token
    /// </summary>
    public async Task RevokeRefreshTokenAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Revokes all refresh tokens for a user
    /// </summary>
    public async Task RevokeAllUserTokensAsync(string userId)
    {
        var userTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync();

        foreach (var token in userTokens)
        {
            token.IsRevoked = true;
        }

        await _context.SaveChangesAsync();
    }
}
