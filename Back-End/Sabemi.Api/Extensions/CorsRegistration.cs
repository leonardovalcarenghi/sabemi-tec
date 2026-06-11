namespace Sabemi.Api.Extensions;

public static class CorsRegistration
{
    public const string LocalPolicy = "Local";
    public const string StagingPolicy = "Staging";
    public const string ProductionPolicy = "Production";

    public static IServiceCollection AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(LocalPolicy, policy =>
            {
                policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            });
        });

        return services;
    }
}

