using NeoVoting.API.StartupExtensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureServices(builder.Configuration, builder.Host);





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHsts();
app.UseHttpsRedirection();


// Enable serving static files (images, css, js) // wwwroot/uploads are gitignored 
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
