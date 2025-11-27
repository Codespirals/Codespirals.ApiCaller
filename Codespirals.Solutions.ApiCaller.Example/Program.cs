using Codespirals.Solutions.ApiCaller;
using Codespirals.Solutions.ApiCaller.Example;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Logging.AddDebug().SetMinimumLevel(LogLevel.Debug);

/// Add the api service to your services with the following command:
/// Replace the name with whatever you name your Api AND the section in the <see cref="IConfigurationSection"/>
builder.RegisterApiCaller("CatFactApi");

/// add your newly created service that uses the <see cref="IApiService"/>
builder.Services.AddScoped<IApiExampleService, ApiExampleService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
