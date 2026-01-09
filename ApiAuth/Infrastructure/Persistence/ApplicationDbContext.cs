using ApiAuth.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiAuth.Infrastructure.Persistence;

/// <summary>
/// Application database context for Identity and application data
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Refresh tokens
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    /// <summary>
    /// Login audit logs
    /// </summary>
    public DbSet<LoginAuditLog> LoginAuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure RefreshToken
        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.Property(rt => rt.Token).IsRequired().HasMaxLength(500);
            entity.Property(rt => rt.UserId).IsRequired();
            entity.HasIndex(rt => rt.Token).IsUnique();
            entity.HasIndex(rt => new { rt.UserId, rt.IsRevoked });
            
            entity.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure LoginAuditLog
        builder.Entity<LoginAuditLog>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Email).IsRequired().HasMaxLength(256);
            entity.Property(l => l.IpAddress).IsRequired().HasMaxLength(45);
            entity.Property(l => l.UserAgent).HasMaxLength(500);
            entity.Property(l => l.Action).IsRequired().HasMaxLength(50);
            entity.HasIndex(l => l.Timestamp);
            entity.HasIndex(l => l.UserId);
        });

        // Seed roles
        var adminRoleId = Guid.NewGuid().ToString();
        var userRoleId = Guid.NewGuid().ToString();

        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = adminRoleId
            },
            new IdentityRole
            {
                Id = userRoleId,
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = userRoleId
            }
        );
    }
}
