using GovernmentSystem.API.API.Middlewares;
using GovernmentSystem.API.Infrastructure.DbContext;
using GovernmentSystem.API.StartupExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add user-secrets and environment variables before calling ConfigureServices
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>(optional: true);
}

builder.Services.ConfigureServices(builder.Configuration, builder.Host);

//builder.Configuration.AddEnvironmentVariables(); // called automatically by CreateBuilder

// 2. Register Seeder
builder.Services.AddTransient<DbSeeder>();

var app = builder.Build();

//CLI data seeding logic
if (args.Length > 0 && args[0].Equals("seedData", StringComparison.OrdinalIgnoreCase))
{
    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();

        // Pass how many records you want to create
        await seeder.SeedAsync(100, 150);
    }
    return; // Exit
}

// =================================================================
// CLI admin seeding logic
// =================================================================
if (args.Length > 0 && args[0].ToLower() == "seedAdmin") // we run dotnet with command line argument "seed" => dotnet run seed "YourStrongPassword!"
{
    // Check if Password argument is provided
    if (args.Length < 2) //args[0] = seed and args[1] = password
    {
        Console.WriteLine("Error: Password argument missing.");
        Console.WriteLine("Usage: dotnet run seed \"YourStrongPassword!\"");
        return; // Exit
    }

    string passwordFromCli = args[1];

    Console.WriteLine("Starting Admin Seeding Process...");

    using (var scope = app.Services.CreateScope())
    {
        try
        {
            // Pass the password to the method
            await DbInitializer.SeedAdminUser(scope.ServiceProvider, passwordFromCli);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Critical Error: {ex.Message}");
        }
    }

    Console.WriteLine("Process complete. Exiting.");
    return; // Stop app, do not start web server
}
// =================================================================

// =================================================================
// CLI admin reset password logic
// =================================================================
if (args.Length > 0 && args[0].ToLower() == "updateadminpassword") // we run dotnet with command line argument "updateAdminPassword" => dotnet run updateAdminPassword "Yourusername" "NewStrongPassword!"
{
    // Check if Username and password arguments are provided
    if (args.Length < 3) //args[0] = updateAdminPassword and args[1] = Username and args[2] = new password
    {
        Console.WriteLine("Error: Username or Password arguments missing.");
        Console.WriteLine("Usage: dotnet run updateAdminPassword \"Yourusername\" \"NewStrongPassword!\" ");
        return; // Exit
    }

    string usernameFromCli = args[1];
    string passwordFromCli = args[2];

    Console.WriteLine("Starting Admin Resetting Password Process...");

    using (var scope = app.Services.CreateScope())
    {
        try
        {
            // Pass the password to the method
            await DbInitializer.UpdateUserPassword(scope.ServiceProvider, usernameFromCli, passwordFromCli);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Critical Error: {ex.Message}");
        }
    }

    Console.WriteLine("Process complete. Exiting.");
    return; // Stop app, do not start web server
}
// =================================================================

app.UseSwagger();
app.UseSwaggerUI();
app.UseExceptionHandler(); // Custom Exception Handler Middleware : Global Exception Handling
//in Application layer , registered in StartupExtensions/ServiceExtensions.cs

// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    //app.UseDeveloperExceptionPage();
    //app.UseSwagger();
    //app.UseSwaggerUI();
}*/

app.UseHsts();
app.UseHttpsRedirection();

app.UseMiddleware<IpWhitelistMiddleware>(); // IP Whitelisting Middleware in API layer
//app.UseCors("FrontendPolicy"); // Must be before Auth
//app.UseStaticFiles();// For serving static files if needed
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();