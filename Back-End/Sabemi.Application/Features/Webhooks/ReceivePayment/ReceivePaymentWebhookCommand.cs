using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Sabemi.Application.Features.Webhooks.ReceivePayment;

public class ReceivePaymentWebhookCommand : PaymentWebhookData, IRequest
{
    [JsonPropertyName("status")]
    [RegularExpression("success|failed", ErrorMessage = "Status deve ser 'success' ou 'failed'.")]
    public override string? Status { get; set; }

}
