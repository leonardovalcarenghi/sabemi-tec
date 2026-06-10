using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sabemi.Application.Abstractions;
using Sabemi.Domain.Interfaces.Repositories;
using Sabemi.Infra.Persistence.Contexts;
using Sabemi.Infra.Persistence.Repositories;
using Sabemi.Infra.Services;

namespace Sabemi.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionString não encontrada.");

        return services
            .AddContext(connectionString)
            .AddHangfire(connectionString)
            .AddRepositories()
            .AddServices();
    }

    private static IServiceCollection AddContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        return services;
    }

    private static IServiceCollection AddHangfire(this IServiceCollection services, string connectionString)
    {
        services.AddHangfire(config =>
        {
            config
               .UseSimpleAssemblyNameTypeSerializer()
               .UseRecommendedSerializerSettings()
               .UseSqlServerStorage(connectionString, new Hangfire.SqlServer.SqlServerStorageOptions()
               {
                   SchemaName = "hangfire",
               });
        });

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 5;
            options.Queues = ["default"];
        });

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<IContractRepository, ContractRepository>()
            .AddScoped<IPaymentWebhookEventRepository, PaymentWebhookEventRepository>();
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddSignalR();

        return services
            .AddScoped<INotificationService, NotificationService>()
            .AddScoped<IWebhookSecurityService, WebhookSecurityService>();
    }
}
