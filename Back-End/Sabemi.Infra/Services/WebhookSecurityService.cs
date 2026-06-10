using Microsoft.Extensions.Configuration;
using Sabemi.Application.Abstractions;

namespace Sabemi.Infra.Services;

internal class WebhookSecurityService(IConfiguration configuration) : IWebhookSecurityService
{
    private readonly string _key = configuration["Webhook:ApiKey"] ?? throw new InvalidOperationException("Webhook:ApiKey não foi configurado.");
    private readonly string _secret = configuration["Webhook:ApiSecret"] ?? throw new InvalidOperationException("Webhook:ApiSecret não foi configurado.");

    public bool IsValidSecret(string apiKey, string apiSecret)
    {
        return string.Equals(apiKey, _key, StringComparison.Ordinal) && string.Equals(apiSecret, _secret, StringComparison.Ordinal);
    }
}
