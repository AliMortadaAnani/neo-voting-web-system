using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace GovernmentSystem.API.Application.CLI
{
    public class AdminOperationsCLI
    {
        
        public static async Task SeedAdminUser(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var logger = serviceProvider.GetRequiredService<ILogger<AdminOperationsCLI>>();

            logger.LogInformation("AdminOperationsCLI: Starting admin user seeding process");

            // 1. Get Username from Environment Variable
            // We look for a variable from environment configuration
            // we can also pass it through cli args if we want to enhance it later
            string adminUsername = configuration["Admin:Username"] ?? "none";

            if (string.IsNullOrEmpty(adminUsername) || adminUsername == "none")
            {
                logger.LogError("AdminOperationsCLI: Admin seeding failed - Admin:Username not configured");
                Console.WriteLine("Error: Environment variable 'Admin:Username' is not set.");
                return;
            }

            string adminPassword = configuration["Admin:Password"] ?? "none";

            if (string.IsNullOrEmpty(adminPassword) || adminPassword == "none")
            {
                logger.LogError("AdminOperationsCLI: Admin seeding failed - Admin:Password not configured");
                Console.WriteLine("Error: Environment variable 'Admin:Password' is not set.");
                return;
            }

            logger.LogInformation("AdminOperationsCLI: Attempting to seed admin user: {AdminUsername}", adminUsername);
            Console.WriteLine($"Attempting to seed Admin: {adminUsername}...");

            // 2. Create Role
            string adminRoleName = RoleTypesEnum.Admin.ToString();

            if (await roleManager.FindByNameAsync(adminRoleName) is null)
            {
                logger.LogInformation("AdminOperationsCLI: Creating admin role");
                ApplicationRole applicationRole = ApplicationRole.CreateAdminRole();

                await roleManager.CreateAsync(applicationRole);
            }

            // 3. Create User
            var adminUser = await userManager.FindByNameAsync(adminUsername);

            if (adminUser == null)
            {
                logger.LogInformation("AdminOperationsCLI: Creating new admin user: {AdminUsername}", adminUsername);
                adminUser = ApplicationUser.CreateAdminUser(adminUsername);

                // Use the password passed from the CLI argument
                var createResult = await userManager.CreateAsync(adminUser, adminPassword);

                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRoleName);
                    logger.LogInformation("AdminOperationsCLI: Admin user created successfully: {AdminUsername}", adminUsername);
                    Console.WriteLine("Admin user created successfully.");
                }
                else
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    logger.LogError("AdminOperationsCLI: Failed to create admin user - Errors: {Errors}", errors);
                    Console.WriteLine($"Error creating Admin user: {errors}");
                }
            }
            else
            {
                logger.LogInformation("AdminOperationsCLI: Admin user already exists: {AdminUsername}", adminUsername);
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