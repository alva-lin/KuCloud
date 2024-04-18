using System.Text.Json.Serialization;
using Ardalis.ListStartupServices;
using FastEndpoints.Swagger;
using KuCloud.Api;
using KuCloud.Infrastructure;
using Serilog;
using Serilog.Extensions.Logging;

Environment.CurrentDirectory = AppContext.BaseDirectory;

var logger = Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

logger.Information("Starting web host");

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, config) =>
    config
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(builder.Configuration)
);
var microsoftLogger = new SerilogLoggerFactory(logger)
    .CreateLogger<Program>();

builder.Services.AddSingleton<AppStatusMonitor>();

// Configure Web Behavior
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddFastEndpoints()
    .SwaggerDocument(o =>
    {
        o.ShortSchemaNames = true;
        o.SerializerSettings = s =>
        {
            s.PropertyNamingPolicy = null;
            s.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        };
    });

builder.Services.AddInfrastructureServices(
    builder.Configuration,
    microsoftLogger,
    builder.Environment.EnvironmentName);

builder.Services.AddCorsSetting(builder.Configuration);

if (builder.Environment.IsDevelopment())
{
    AddShowAllServicesSupport();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseShowAllServicesMiddleware(); // see https://github.com/ardalis/AspNetCoreStartupServices
}
else
{
    app.UseDefaultExceptionHandler(); // from FastEndpoints
    app.UseHsts();
}

app.UseCors();

app.UseFastEndpoints(config =>
    {
        ProblemDetails.TitleValue = "One or more validation errors occurred.";
        ProblemDetails.AllowDuplicates = true;
        ProblemDetails.Error.IndicateSeverity = true;
        config.Errors.UseProblemDetails();

        config.Serializer.Options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    })
    .UseSwaggerGen(); // Includes AddFileServer and static files middleware

// app.UseHttpsRedirection();

SeedDatabase(app);

app.Run();

static void SeedDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        //          context.Database.Migrate();
        context.Database.EnsureCreated();
        SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
    }
}

void AddShowAllServicesSupport()
{
    // add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
    builder.Services.Configure<ServiceConfig>(config =>
    {
        config.Services = new List<ServiceDescriptor>(builder.Services);

        // optional - default path to view services is /listallservices - recommended to choose your own path
        config.Path = "/listservices";
    });
}

// Make the implicit Program.cs class public, so integration tests can reference the correct assembly for host building
public partial class Program
{
}
