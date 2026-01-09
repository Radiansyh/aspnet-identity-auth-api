using ApiAuth.Domain;
using ApiAuth.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApiAuth.Infrastructure.Seed;

/// <summary>
/// Database seeder for creating initial admin user
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Seeds the database with an admin user if no admin exists
    /// </summary>
    public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (roleResult.Succeeded)
                    {
                        logger.LogInformation("Role '{RoleName}' created successfully", roleName);
                    }
                    else
                    {
                        logger.LogWarning("Failed to create role '{RoleName}'", roleName);
                    }
                }
            }

            var adminEmail = "admin@apiauth.com";
            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    await userManager.AddToRoleAsync(adminUser, "User");
                    
                    var roles = await userManager.GetRolesAsync(adminUser);
                    logger.LogInformation("Admin user created successfully. Email: {Email}, Password: Admin@123, Roles: {Roles}", 
                        adminEmail, string.Join(", ", roles));
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    logger.LogError("Failed to create admin user: {Errors}", errors);
                }
            }
            else
            {
                var existingRoles = await userManager.GetRolesAsync(existingAdmin);
                logger.LogInformation("Admin user already exists. Current roles: {Roles}", string.Join(", ", existingRoles));
                
                if (!existingRoles.Contains("Admin"))
                {
                    await userManager.AddToRoleAsync(existingAdmin, "Admin");
                    logger.LogInformation("Added 'Admin' role to existing admin user");
                }
                if (!existingRoles.Contains("User"))
                {
                    await userManager.AddToRoleAsync(existingAdmin, "User");
                    logger.LogInformation("Added 'User' role to existing admin user");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the admin user");
        }
    }
}
