using IQGROUP_test_task;
using IQGROUP_test_task.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MongoDB.Driver;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IQGROUP_test_task.Middleware;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using IQGROUP_test_task.Services;

var builder = WebApplication.CreateBuilder(args);

//Cors
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("local", policy =>
//    {
//        policy.AllowAnyOrigin();
//        policy.AllowAnyHeader();
//        policy.AllowAnyMethod();
//    });
//});

// Объект HttpContextAccessor для сервиса JwtAuthenticationStateProvider

builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorPages();

builder.Services.AddScoped(sp => 
new HttpClient 
{ 
    BaseAddress = new Uri("https://localhost:7078/")
});
builder.Services.AddServerSideBlazor();
// Подключение сервиса SignalR

builder.Services.AddSignalR();

builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddControllers();

var _configuration = builder.Configuration;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = _configuration["JwtSettings:Issuer"],
        ValidAudience = _configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]))
    };
});

builder.Services.AddAuthorizationCore();

// Получение строки подключения к mongodb из appsettings.json
//var configurationBuilder = new ConfigurationBuilder();
//configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
//configurationBuilder.AddJsonFile("appsettings.json");
//var config = configurationBuilder.Build();
//string connectionString = config.GetConnectionString("MongoDb");

// Подключение к БД

builder.Services.AddSingleton<IMongoClient, MongoClient>(options =>
{
    //var settings = options.GetRequiredService<MongoDatabaseSettings>();
    return new MongoClient(_configuration["MongoDbSettings:ConnectionString"]);
});
// Внеднение кастомных зависимоcтей  

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IDateTimeService, DateTimeService>();

// Получить header авторизации в компоненте Blazor
//builder.Services.AddSingleton<IAuthorizationService, AuthorizationService>();
//builder.Services.AddScoped<AuthorizationService>();

builder.Services.AddLogging();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.MapHub<ColorThemeHub>("/color-theme-hub");

//app.Run(async (context) =>
//{
//    app.Logger.LogWarning("First log");
//    context.Response.Redirect("/");
//});

//app.UseCors("local");

app.MapRazorPages();

app.MapControllers();

//app.UseMiddleware<AuthorizeHeadersMiddleware>();

app.Run();
