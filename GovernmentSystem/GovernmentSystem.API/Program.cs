using GovernmentSystem.API.API.Middlewares;
using GovernmentSystem.API.StartupExtensions;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddEnvironmentVariables();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.ConfigureServices(builder.Configuration, builder.Host);
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseMiddleware<IpWhitelistMiddleware>();
/*// =================================================================
// CLI SEEDING LOGIC
// =================================================================
if (args.Length > 0 && args[0].ToLower() == "seed")
{
    // Check if Password argument is provided
    if (args.Length < 2)
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
*/


app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHsts();
app.UseHttpsRedirection();


app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();