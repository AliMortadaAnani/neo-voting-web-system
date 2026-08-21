using GovernmentSystem.API.Infrastructure.DbContext;
using GovernmentSystem.API.StartupExtensions;

var builder = WebApplication.CreateBuilder(args);

// Calls all extension methods sequentially under the hood
builder.ConfigureAllServices();

var app = builder.Build();

// Check and execute any CLI commands. If true, exit the app immediately.
if (await SeedingWithCLI.HandleCliCommandsAsync(app, args))
{
    return;
}


app.UseSwagger();
app.UseSwaggerUI();


app.UseExceptionHandler(); // Custom Exception Handler Middleware : Global Exception Handling
//in Application layer , registered in StartupExtensions/ServiceExtensions.cs


//app.UseHttpsRedirection();

//if (!app.Environment.IsDevelopment())
//{
//    app.UseHsts();
//}

//app.UseCors("FrontendPolicy");

//app.UseMiddleware<IpWhitelistMiddleware>(); // IP Whitelisting Middleware in API layer

//app.UseAuthentication();
//app.UseAuthorization();

//app.UseStaticFiles();

app.MapControllers();

app.Run();