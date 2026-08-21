using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using GovernmentSystem.API.Infrastructure.DbContext;

namespace GovernmentSystem.API.StartupExtensions
{
    public static class SeedingWithCLI
    {
        /// <summary>
        /// Checks incoming command line arguments and routes to the correct seeding function. 
        /// Returns true if a CLI command was handled (meaning the app should exit instead of starting the web server).
        /// </summary>
        public static async Task<bool> HandleCliCommandsAsync(WebApplication app, string[] args)
        {
            if (args.Length == 0)
            {
                return false; // No CLI arguments, proceed to run web server normally
            }

            string command = args[0];

            return command.ToLower() switch
            {
                "seeddata" => await HandleSeedDataAsync(app),
                "seedadmin" => await HandleSeedAdminAsync(app),
                "updateadminpassword" => await HandleUpdateAdminPasswordAsync(app, args),
                _ => false // Command not recognized, proceed to start web server normally
            };
        }

        private static async Task<bool> HandleSeedDataAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<DataSeederCLI>();
            await seeder.SeedAsync(100, 150);
            Console.WriteLine("Data seeding complete. Exiting.");
            return true;
        }

        private static async Task<bool> HandleSeedAdminAsync(WebApplication app)
        {
           
            
            Console.WriteLine("Starting Admin Seeding Process...");

            using var scope = app.Services.CreateScope();
            try
            {
                await AdminOperationsCLI.SeedAdminUser(scope.ServiceProvider);
                Console.WriteLine("Admin seeding complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical Error: {ex.Message}");
            }

            Console.WriteLine("Process complete. Exiting.");
            return true;
        }

        private static async Task<bool> HandleUpdateAdminPasswordAsync(WebApplication app, string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Error: Username or Password arguments missing.");
                Console.WriteLine("Usage: dotnet run updateAdminPassword \"YourUsername\" \"NewStrongPassword!\"");
                return true;
            }

            string usernameFromCli = args[1];
            string passwordFromCli = args[2];

            if(string.IsNullOrWhiteSpace(usernameFromCli) || string.IsNullOrWhiteSpace(passwordFromCli))
            {
                Console.WriteLine("Error: Username or Password cannot be empty.");
                return true;
            } 
            Console.WriteLine("Starting Admin Resetting Password Process...");

            using var scope = app.Services.CreateScope();
            try
            {
                await AdminOperationsCLI.UpdateUserPassword(scope.ServiceProvider, usernameFromCli, passwordFromCli);
                Console.WriteLine("Password updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical Error: {ex.Message}");
            }

            Console.WriteLine("Process complete. Exiting.");
            return true;
        }
    }
}