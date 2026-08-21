using NeoVoting.API.StartupExtensions;
using NeoVoting.Application.SeederServices;

var builder = WebApplication.CreateBuilder(args);

// Calls all extension methods sequentially under the hood
builder.ConfigureAllServices();



var app = builder.Build();


// Check and process any CLI arguments (exits early if a command is run)
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