using Application.Services;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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

    // Register services and repositories
    builder.Services.AddSingleton<DbHelper>(); // Ensure correct helper name

    builder.Services.AddScoped<ProductService>();
    builder.Services.AddScoped<UserService>();

    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();

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

    builder.Services.AddControllers(); // Ensure controllers are added

    //// Add API versioning
    //builder.Services.AddApiVersioning(options =>
    //{
    //    //options.DefaultApiVersion = new ApiVersion(1, 0);
    //    options.DefaultApiVersion = ApiVersion.Default;
    //    options.AssumeDefaultVersionWhenUnspecified = true;
    //    options.ReportApiVersions = true;
    //}).AddApiExplorer(option =>
    //{
    //    option.GroupNameFormat = "'v'V";
    //    option.SubstituteApiVersionInUrl = true;
    //});

    // ✅ Register API Versioning
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader(); // Read version from URL
    }).AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";  // ✅ Ensures "v1", "v2", etc.
        options.SubstituteApiVersionInUrl = true;
    });

    // ✅ Configure Swagger to display all API versions on a single page
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Product API v1", Version = "1.0" });
        options.SwaggerDoc("v2", new OpenApiInfo { Title = "Product API v2", Version = "2.0" });

        // ✅ Group endpoints using Controller name + Version
        options.TagActionsBy(apiDesc =>
        {
            var controllerName = apiDesc.ActionDescriptor.RouteValues["controller"];
            var version = apiDesc.ActionDescriptor.EndpointMetadata
                .OfType<ApiVersionAttribute>()
                .FirstOrDefault()?.Versions.FirstOrDefault()?.ToString();

            return new[] { $"{version}" }; // ✅ Group as "ProductsV1" or "ProductsV2"
        });

        options.DocInclusionPredicate((version, apiDesc) => true);
    });


    var app = builder.Build();

    // ✅ Enable Swagger on a single page
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Product API {description.ApiVersion}");
            }

            // ✅ Remove the version dropdown
            options.DefaultModelsExpandDepth(-1); // Hide schemas for better readability
        });
    }



    app.UseHttpsRedirection();
    app.UseAuthentication();
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
