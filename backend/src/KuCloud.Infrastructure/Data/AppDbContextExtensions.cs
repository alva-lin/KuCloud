using System.Globalization;
using EFCore.NamingConventions.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KuCloud.Infrastructure.Data;

public static class AppDbContextExtensions
{
    public static void AddApplicationDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention()
        );
    }

    public static string ToSnakeCase(this string input, CultureInfo? cultureInfo = null)
    {
        return new SnakeCaseNameRewriter(cultureInfo ?? CultureInfo.CurrentCulture).RewriteName(input);
    }
}
