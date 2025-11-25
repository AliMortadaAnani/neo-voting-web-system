using GovernmentSystem.API.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace GovernmentSystem.API.Infrastructure.DbContext
{
    public class DbInitializer
    {

        // Note: We added 'adminPassword' as a parameter
        public static async Task SeedAdminUser(IServiceProvider serviceProvider, string adminPassword)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            // 1. Get Username from Environment Variable
            // We look for a variable from environment configuration
            string adminUsername = configuration["Admin:Username"] ?? "none";

            if (string.IsNullOrEmpty(adminUsername) || adminUsername == "none")
            {
                Console.WriteLine("Error: Environment variable 'Admin:Username' is not set.");
                return;
            }

            Console.WriteLine($"Attempting to seed Admin: {adminUsername}...");

            // 2. Create Role
            string adminRoleName = "Admin";


            if (await roleManager.FindByNameAsync("Admin") is null)
            {
                ApplicationRole applicationRole = new ApplicationRole
                {
                    Name = adminRoleName
                };
                await roleManager.CreateAsync(applicationRole);

            }


            // 3. Create User
            var adminUser = await userManager.FindByNameAsync(adminUsername);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminUsername,

                };

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

    }
}
