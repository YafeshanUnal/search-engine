using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SearchEngineService.Constants;
using SearchEngineService.Data;
using SearchEngineService.Exceptions;
using SearchEngineService.Middleware;
using SearchEngineService.Models;
using SearchEngineService.Options;
using SearchEngineService.Providers;
using SearchEngineService.Providers.Behavior;
using SearchEngineService.Repositories;
using SearchEngineService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Swagger/OpenAPI configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Search Engine API",
        Version = "v1",
        Description = "ASP.NET Core Web API for content search and provider integration",
        Contact = new OpenApiContact
        {
            Name = "Search Engine Team",
            Email = "team@searchengine.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Include XML Comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Add security definition (if needed)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddMemoryCache();
builder.Services.Configure<ScoringOptions>(builder.Configuration.GetSection(ConfigurationConstants.ScoringSection));
builder.Services.Configure<ProviderOptions>(builder.Configuration.GetSection(ConfigurationConstants.ProvidersSection));
builder.Services.AddCors(options =>
{
    options.AddPolicy(ConfigurationConstants.DashboardPolicy, policy =>
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var conn = builder.Configuration.GetConnectionString("DefaultConnection")
               ?? ConfigurationConstants.DefaultConnection;
    options.UseNpgsql(conn);
});

builder.Services.AddScoped<IContentRepository, ContentRepository>();
builder.Services.AddScoped<ContentIngestionService>();
builder.Services.AddScoped<ContentSearchService>();
builder.Services.AddScoped<ScoreCalculator>();
builder.Services.AddScoped<StartupService>();
builder.Services.AddTransient<JsonProviderRateLimitHandler>();
builder.Services.AddTransient<XmlProviderRateLimitHandler>();

var jsonProviderUrl = builder.Configuration[ConfigurationConstants.JsonUrlKey];
var xmlProviderUrl = builder.Configuration[ConfigurationConstants.XmlUrlKey];
if (string.IsNullOrWhiteSpace(jsonProviderUrl) || string.IsNullOrWhiteSpace(xmlProviderUrl))
{
    throw new InvalidOperationException($"{ConfigurationConstants.JsonUrlKey} and {ConfigurationConstants.XmlUrlKey} must be configured in appsettings or environment variables.");
}

builder.Services.AddHttpClient<JsonProviderClient>(client =>
{
    client.BaseAddress = new Uri(jsonProviderUrl);
}).AddHttpMessageHandler<JsonProviderRateLimitHandler>();
builder.Services.AddHttpClient<XmlProviderClient>(client =>
{
    client.BaseAddress = new Uri(xmlProviderUrl);
}).AddHttpMessageHandler<XmlProviderRateLimitHandler>();
builder.Services.AddScoped<IProviderClient>(sp => sp.GetRequiredService<JsonProviderClient>());
builder.Services.AddScoped<IProviderClient>(sp => sp.GetRequiredService<XmlProviderClient>());

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    // Enable Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Search Engine API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "Search Engine API Documentation";
        c.DefaultModelsExpandDepth(-1); // Hide models by default
        c.DefaultModelExpandDepth(1);
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        c.DisplayRequestDuration();
    });
}

app.UseCors(ConfigurationConstants.DashboardPolicy);
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    
    // Perform startup tasks after database is ready
    var startupService = scope.ServiceProvider.GetRequiredService<StartupService>();
    await startupService.PerformStartupTasksAsync();
}

app.Run();
