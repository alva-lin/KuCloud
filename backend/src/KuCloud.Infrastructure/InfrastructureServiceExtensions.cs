using System.Reflection;
using Ardalis.GuardClauses;
using KuCloud.Core.Domains.StorageAggregate;
using KuCloud.Core.Interfaces;
using KuCloud.Infrastructure.Behaviors;
using KuCloud.Infrastructure.Data;
using KuCloud.Infrastructure.Email;
using KuCloud.Infrastructure.Services;
using KuCloud.UseCases.Storages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KuCloud.Infrastructure;

public static class InfrastructureServiceExtensions
{
    private const string Development = "Development";
    private const string Production = "Production";


    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        ConfigurationManager config,
        ILogger logger,
        string environmentName
    )
    {
        services.AddMediatR();

        services.AddDataContext(config);

        services.AddFileService(config, logger);

        services.AddServiceByEnvironment(config, logger, environmentName);

        logger.LogInformation("{Project} services registered", "Infrastructure");

        return services;
    }

    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        var mediatRAssemblies = new[]
        {
            Assembly.GetAssembly(typeof(Folder)), // Core
            Assembly.GetAssembly(typeof(CreateFolderCommand)) // UseCases
        };

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(mediatRAssemblies!);
            cfg.AddOpenBehavior(typeof(MyLoggingBehavior<,>));
        });
        // services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();
        return services;
    }

    private static IServiceCollection AddDataContext(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("KuCloud");
        Guard.Against.Null(connectionString);
        services.AddApplicationDbContext(connectionString);

        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

        return services;
    }

    private static IServiceCollection AddFileService(
        this IServiceCollection services,
        IConfiguration configuration,
        ILogger logger)
    {
        logger.LogInformation("File service registered: {FileServiceType}", nameof(LocalStorageService));
        services.AddScoped<IFileService, LocalStorageService>();

        return services;
    }

    private static IServiceCollection AddServiceByEnvironment(
        this IServiceCollection services,
        ConfigurationManager config,
        ILogger logger,
        string environmentName
    )
    {
        if (Development == environmentName)
        {
            logger.LogInformation("Development environment detected");
            services.AddScoped<IEmailSender, FakeEmailSender>();
        }
        else if (Production == environmentName)
        {
            logger.LogInformation("Production environment detected");
            services.AddScoped<IEmailSender, FakeEmailSender>();
        }

        return services;
    }
}
