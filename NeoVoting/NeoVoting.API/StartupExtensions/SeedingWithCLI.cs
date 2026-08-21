using NeoVoting.Application.SeederServices;

namespace NeoVoting.API.StartupExtensions
{
    public static class SeedingWithCLI
    {
        /// <summary>
        /// Handles incoming command line arguments. Returns true if handled (stops app execution).
        /// </summary>
        public static async Task<bool> HandleCliCommandsAsync(WebApplication app, string[] args)
        {
            if (args.Length == 0) return false;

            string command = args[0].ToLower();

            return command switch
            {
                "seedadmin" => await HandleSeedAdminAsync(app, args),
                "updateadminpassword" => await HandleUpdateAdminPasswordAsync(app, args),
                _ => false
            };
        }

        private static async Task<bool> HandleSeedAdminAsync(WebApplication app, string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Error: Password argument missing.");
                Console.WriteLine("Usage: dotnet run seedadmin \"YourStrongPassword!\"");
                return true;
            }

            string passwordFromCli = args[1];
            Console.WriteLine("Starting Admin Seeding Process...");

            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    await DbInitializer.SeedAdminUser(scope.ServiceProvider, passwordFromCli);
                    Console.WriteLine("Admin seeding complete.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Critical Error: {ex.Message}");
                }
            }

            Console.WriteLine("Process complete. Exiting.");
            return true;
        }

        private static async Task<bool> HandleUpdateAdminPasswordAsync(WebApplication app, string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Error: Username or Password arguments missing.");
                Console.WriteLine("Usage: dotnet run updateadminpassword \"Yourusername\" \"NewStrongPassword!\"");
                return true;
            }

            string usernameFromCli = args[1];
            string passwordFromCli = args[2];
            Console.WriteLine("Starting Admin Resetting Password Process...");

            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    await DbInitializer.UpdateUserPassword(scope.ServiceProvider, usernameFromCli, passwordFromCli);
                    Console.WriteLine("Password updated successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Critical Error: {ex.Message}");
                }
            }

            Console.WriteLine("Process complete. Exiting.");
            return true;
        }
    }
}
