namespace KuCloud.Api;

public static class ApiServiceExtensions
{
    public static IServiceCollection AddCorsSetting(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CorsOption>(configuration.GetSection(nameof(CorsOption)));

        services.AddCors(options =>
        {
            var corsOption = configuration.GetSection(nameof(CorsOption)).Get<CorsOption>();
            Guard.Against.Null(corsOption);

            options.AddDefaultPolicy(builder =>
            {
                builder.AllowCredentials();

                if (corsOption.AllowAnyOrigin ?? false)
                {
                    builder.AllowAnyOrigin();
                }
                else
                {
                    builder.WithOrigins(corsOption.AllowOrigins);
                }

                if (corsOption.AllowAnyMethod ?? false)
                {
                    builder.AllowAnyMethod();
                }
                else
                {
                    builder.WithMethods(corsOption.AllowMethods);
                }

                if (corsOption.AllowAnyHeader ?? false)
                {
                    builder.AllowAnyHeader();
                }
                else
                {
                    builder.WithHeaders(corsOption.AllowHeaders);
                }

                builder.WithExposedHeaders(corsOption.AllowExposedHeaders);
            });
        });

        return services;
    }
}
