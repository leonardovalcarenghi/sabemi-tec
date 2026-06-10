namespace Sabemi.Application.Abstractions;

public interface IWebhookSecurityService
{
    bool IsValidSecret(string apiKey, string apiSecret); 
}
