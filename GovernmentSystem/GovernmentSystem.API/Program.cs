using GovernmentSystem.API.API.Middlewares;
using GovernmentSystem.API.Infrastructure.DbContext;
using GovernmentSystem.API.StartupExtensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureServices(builder.Configuration, builder.Host);

//builder.Configuration.AddEnvironmentVariables(); // called automatically by CreateBuilder
// 2. Register Seeder
builder.Services.AddTransient<DbSeeder>();
var app = builder.Build();


// 3. CLI Logic
if (args.Length > 0 && args[0].Equals("seed", StringComparison.OrdinalIgnoreCase))
{
    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();

        // Pass how many records you want to create
        await seeder.SeedAsync(100,50);
    }
    return; // Exit
}



// =================================================================
// CLI SEEDING LOGIC
// =================================================================
//if (args.Length > 0 && args[0].ToLower() == "seed") // we run dotnet with command line argument "seed" => dotnet run seed "YourStrongPassword!"
//{
//    // Check if Password argument is provided
//    if (args.Length < 2) //args[0] = seed and args[1] = password
//    {
//        Console.WriteLine("Error: Password argument missing.");
//        Console.WriteLine("Usage: dotnet run seed \"YourStrongPassword!\"");
//        return; // Exit
//    }

//    string passwordFromCli = args[1];

//    Console.WriteLine("Starting Admin Seeding Process...");

//    using (var scope = app.Services.CreateScope())
//    {
//        try
//        {
//            // Pass the password to the method
//            await DbInitializer.SeedAdminUser(scope.ServiceProvider, passwordFromCli);
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Critical Error: {ex.Message}");
//        }
//    }

//    Console.WriteLine("Process complete. Exiting.");
//    return; // Stop app, do not start web server
//}
// =================================================================




app.UseSwagger();
app.UseSwaggerUI();
app.UseExceptionHandler(); // Custom Exception Handler Middleware : Global Exception Handling
//in Application layer , registered in StartupExtensions/ServiceExtensions.cs

// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}*/

app.UseHsts();
app.UseHttpsRedirection();

app.UseMiddleware<IpWhitelistMiddleware>(); // IP Whitelisting Middleware in API layer

//app.UseStaticFiles();// For serving static files if needed
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();