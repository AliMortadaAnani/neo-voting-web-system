using NeoVoting.API.StartupExtensions;

var builder = WebApplication.CreateBuilder(args);


// Calls all extension methods sequentially under the hood
builder.ConfigureAllServices();

var app = builder.Build();

app.Logger.LogInformation("NeoVoting API is starting up.");

// Check and process any CLI arguments (exits early if a command is run)
if (await SeedingWithCLI.HandleCliCommandsAsync(app, args))
{
    app.Logger.LogInformation("NeoVoting API is exiting after CLI processing.");
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

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Logger.LogInformation("NeoVoting API has started and is listening for requests.");
app.Run();