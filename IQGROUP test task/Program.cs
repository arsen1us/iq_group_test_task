using IQGROUP_test_task;
using IQGROUP_test_task.Data;
using IQGROUP_test_task.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("local", policy =>
//    {
//        policy.AllowAnyOrigin();
//        policy.AllowAnyHeader();
//        policy.AllowAnyMethod();
//    });
//});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped(sp => 
new HttpClient 
{ 
    BaseAddress = new Uri("https://localhost:7078/")
});
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddControllers();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();



builder.Services.AddSignalR();
// Подключение к БД

var configurationBuilder = new ConfigurationBuilder();
configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
configurationBuilder.AddJsonFile("appsettings.json");

var config = configurationBuilder.Build();
string connectionString = config.GetConnectionString("MongoDb");

builder.Services.AddSingleton<IMongoClient, MongoClient>(options =>
{
    //var settings = options.GetRequiredService<MongoDatabaseSettings>();
    return new MongoClient(config["MongoDbSettings:ConnectionString"]);
});
// Кастомные зависимости

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ITokenService, TokenService>();


builder.Services.AddLogging();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.MapHub<ColorThemeHub>("/color-theme-hub");

//app.Run(async (context) =>
//{
//    app.Logger.LogWarning("First log");
//    context.Response.Redirect("/");
//});

//app.UseCors("local");

app.MapControllers();

app.Run();
