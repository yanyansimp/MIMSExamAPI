using Application.Services;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using System.Text;

var logger = LogManager.Setup().LoadConfigurationFromFile("NLog.config").GetCurrentClassLogger();
logger.Info("Starting Web API...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Use NLog for logging
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    //builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

    // Register services and repositories
    builder.Services.AddSingleton<DbHelper>();

    builder.Services.AddScoped<ProductService>();
    builder.Services.AddScoped<UserService>();

    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();

    // Add authentication and authorization 
    //builder.Services.AddAuthentication();

    // Configure JWT Authentication
    var jwtSecret = builder.Configuration["JwtSettings:Secret"];

    if (string.IsNullOrEmpty(jwtSecret) || jwtSecret.Length < 32)
        throw new InvalidOperationException("JWT Secret key must be at least 32 characters long.");

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
            };

            options.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    var logger = LogManager.GetCurrentClassLogger();
                    logger.Warn($"Unauthorized request to {context.Request.Path}");

                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("{\"message\": \"Unauthorized Request\"}");
                }
            };
        });

    builder.Services.AddAuthorization();

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    logger.Info("Web API started successfully.");
    app.Run();

}
catch (Exception ex)
{
    logger.Error(ex, "Application encountered a fatal error.");
    throw;
}
finally
{
    LogManager.Shutdown(); // Ensures logs are flushed before exit
}

