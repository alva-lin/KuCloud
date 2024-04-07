using System.Reflection;
using Ardalis.GuardClauses;
using Ardalis.SharedKernel;
using KuCloud.Core.ContributorAggregate;
using KuCloud.Infrastructure.Behaviors;
using KuCloud.Infrastructure.Data;
using KuCloud.UseCases.Contributors.Create;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KuCloud.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        ConfigurationManager config,
        ILogger logger,
        string environmentName
        )
    {
        services.AddMediatR();

        var connectionString = config.GetConnectionString("KuCloud");
        Guard.Against.Null(connectionString);
        services.AddApplicationDbContext(connectionString);

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

        // AddServiceByEnvironment(services, config, logger, environmentName);

        logger.LogInformation("{Project} services registered", "Infrastructure");

        return services;
    }

    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        var mediatRAssemblies = new[]
        {
            Assembly.GetAssembly(typeof(Contributor)),             // Core
            Assembly.GetAssembly(typeof(CreateContributorCommand)) // UseCases
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

    // private static IServiceCollection AddServiceByEnvironment(
    //     this IServiceCollection services,
    //     ConfigurationManager config,
    //     ILogger logger,
    //     string environmentName
    // )
    // {
    //     if (Environments.Development == environmentName)
    //     {
    //         logger.LogInformation("Development environment detected");
    //         services.AddScoped<IEmailSender, FakeEmailSender>();
    //     }
    //     else
    //     {
    //         logger.LogInformation("Production environment detected");
    //         services.AddScoped<IEmailSender, FakeEmailSender>();
    //     }
    //
    //     return services;
    // }
}
