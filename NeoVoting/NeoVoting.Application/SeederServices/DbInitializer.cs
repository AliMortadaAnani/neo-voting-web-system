using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.SeederServices
{
    public class DbInitializer
    {
        // Note: We added 'adminPassword' as a parameter instead of fetching it from configuration
        // This allows passing the password securely from the CLI argument
        public static async Task SeedAdminUser(IServiceProvider serviceProvider, string adminPassword)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            // 1. Get Username from Environment Variable
            // We look for a variable from environment configuration
            // we can also pass it through cli args if we want to enhance it later
            string adminUsername = configuration["Admin:Username"] ?? "none";

            if (string.IsNullOrEmpty(adminUsername) || adminUsername == "none")
            {
                Console.WriteLine("Error: Environment variable 'Admin:Username' is not set.");
                return;
            }

            Console.WriteLine($"Attempting to seed Admin: {adminUsername}...");

            // 2. Create Role
            string adminRoleName = RoleTypesEnum.Admin.ToString();

            if (await roleManager.FindByNameAsync(adminRoleName) is null)
            {
                ApplicationRole applicationRole = ApplicationRole.CreateAdminRole();
                await roleManager.CreateAsync(applicationRole);
            }

            // 3. Create User
            var adminUser = await userManager.FindByNameAsync(adminUsername);

            if (adminUser == null)
            {
                adminUser = ApplicationUser.CreateAdminAccount(adminUsername);
                // Use the password passed from the CLI argument
                var createResult = await userManager.CreateAsync(adminUser, adminPassword);

                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRoleName);
                    Console.WriteLine("Admin user created successfully.");
                }
                else
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    Console.WriteLine($"Error creating Admin user: {errors}");
                }
            }
            else
            {
                Console.WriteLine("Admin user already exists.");
            }
        }

        public static async Task UpdateUserPassword(IServiceProvider serviceProvider, string username, string newPassword)
        {
            // here both username and newPassword are passed from CLI arguments
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. Find the user
            var user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                Console.WriteLine($"Error: User '{username}' not found. Cannot update password.");
                return;
            }

            Console.WriteLine($"Updating password for user: {username}...");

            // 2. Generate a reset token
            // This allows us to change the password without knowing the current one.
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            // 3. Reset the password
            var result = await userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                Console.WriteLine($"Password for '{username}' has been updated successfully.");
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                Console.WriteLine($"Error updating password: {errors}");
            }
        }
    }
}