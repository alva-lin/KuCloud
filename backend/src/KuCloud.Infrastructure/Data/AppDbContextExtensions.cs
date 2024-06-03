using System.Globalization;
using System.Reflection;
using EFCore.NamingConventions.Internal;
using KuCloud.Infrastructure.Data.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KuCloud.Infrastructure.Data;

public static class AppDbContextExtensions
{
    public static void AddApplicationDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<SetAuditInfoInterceptor>();
        services.AddScoped<DispatchAndClearEventsInterceptor>();

        services.AddDbContext<AppDbContext>((provider, options) =>
            {
                options.UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention();

#if DEBUG
                options.EnableSensitiveDataLogging();
#endif

                options.AddInterceptors([
                    provider.GetRequiredService<SetAuditInfoInterceptor>(),
                    provider.GetRequiredService<DispatchAndClearEventsInterceptor>(),
                ]);
            }
        );
    }

    public static string ToSnakeCase(this string input, CultureInfo? cultureInfo = null)
    {
        return new SnakeCaseNameRewriter(cultureInfo ?? CultureInfo.CurrentCulture).RewriteName(input);
    }

    public static ModelBuilder ApplyBasicEntityConfigurationsFromAssembly(
        this ModelBuilder builder,
        ILogger logger,
        params Assembly[] assemblies
    )
    {
        using var _ = logger.BeginScope("Configuration BasicEntity Types");

        var configuredTypes = new List<Type>();
        var tConfigs = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type is
            {
                IsClass: true,
                IsAbstract: false,
                IsGenericType: false,
                BaseType.IsGenericType: true,
            })
            .Where(type => type.BaseType?.GetGenericTypeDefinition() == typeof(BasicEntityConfiguration<,>))
            .Distinct()
            .ToList();

        foreach (var tConfig in tConfigs)
        {
            var config = Activator.CreateInstance(tConfig)!;

            builder.ApplyConfiguration((dynamic) config);

            var tEntity = tConfig.BaseType!.GetGenericArguments().First();
            configuredTypes.Add(tEntity);

            logger.LogInformation("Configuring {EntityType} with {EntityConfigurationType}", tEntity.Name,
                tConfig.Name);
        }

        var entityTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type is { IsClass: true, IsAbstract: false })
            .Where(type => type.IsAssignableTo(typeof(BasicEntity<>)))
            .Where(type => !configuredTypes.Contains(type))
            .Distinct()
            .ToList();
        foreach (var tEntity in entityTypes)
        {
            var tKey = tEntity.BaseType!.GetGenericArguments().First();

            var tConfig = typeof(BasicEntityConfiguration<,>).MakeGenericType(tEntity, tKey);

            var config = Activator.CreateInstance(tConfig)!;

            builder.ApplyConfiguration((dynamic) config);

            logger.LogInformation("Configuring {EntityType} with {EntityConfigurationType}", tEntity.Name,
                tConfig.Name);
        }

        return builder;
    }
}
