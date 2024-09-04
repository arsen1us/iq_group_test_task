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

// Объект HttpContextAccessor для сервиса JwtAuthenticationStateProvider
builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorPages();
builder.Services.AddScoped(sp => 
new HttpClient 
{ 
    BaseAddress = new Uri("https://localhost:7078/")
});
builder.Services.AddServerSideBlazor();
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
builder.Services.AddTransient<IDateTimeService, DateTimeService>();

// Получить header авторизации в компоненте Blazor
//builder.Services.AddSingleton<IAuthorizationService, AuthorizationService>();
//builder.Services.AddScoped<AuthorizationService>();

builder.Services.AddLogging();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

builder.Services.AddAuthorizationCore();

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

app.UseAuthentication();
app.UseAuthorization();

//app.UseMiddleware<AuthorizeHeadersMiddleware>();

app.Run();
