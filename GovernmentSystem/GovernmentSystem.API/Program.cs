using GovernmentSystem.API.API.Middlewares;
using GovernmentSystem.API.StartupExtensions;

var builder = WebApplication.CreateBuilder(args);

// Calls all extension methods sequentially under the hood
builder.ConfigureAllServices();

var app = builder.Build();

app.Logger.LogInformation("GovernmentSystem API is starting up.");

// Check and execute any CLI commands. If true, exit the app immediately.
if (await SeedingWithCLI.HandleCliCommandsAsync(app, args))
{
    return;
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandler(); // Custom Exception Handler Middleware : Global Exception Handling
//in Application layer , registered in StartupExtensions/ServiceExtensions.cs

app.UseHttpsRedirection();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseCors("FrontendPolicy");

app.UseMiddleware<IpWhitelistMiddleware>(); // IP Whitelisting Middleware in API layer

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Logger.LogInformation("GovernmentSystem API has started and is listening for requests.");


app.Run();