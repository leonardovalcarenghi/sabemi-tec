using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Sabemi.Application.Features.Webhooks.ReceivePaymentWebhook;

public class ReceivePaymentWebhookCommand : IRequest
{
    [JsonPropertyName("id_transacao")]
    public Guid TransactionId { get; set; }

    [JsonPropertyName("id_contrato")]
    public Guid ContractId { get; set; }

    [JsonPropertyName("status")]
    [RegularExpression("success|failed", ErrorMessage = "Status deve ser 'success' ou 'failed'.")]
    public string? Status { get; set; }

    /// <summary>
    /// Valor do pagamento recebido.
    /// </summary>
    /// <remarks>
    /// Pode ser nulo se o <see cref="Status"/> indicar que houve falha no pagamento.
    /// </remarks>
    [JsonPropertyName("valor")]
    public decimal? Amount { get; set; }

    /// <summary>
    /// Data e hora em que o pagamento foi recebido.
    /// </summary>
    /// <remarks>
    /// Pode ser nulo se o <see cref="Status"/> indicar que houve falha no pagamento.
    /// </remarks>
    [JsonPropertyName("data_pagamento")]
    public DateTime? PaidAt { get; set; }

    /// <summary>
    /// Converte o comando para uma string JSON, para ser armazenada no banco de dados como payload do evento de webhook.
    /// </summary>
    public string ToJson() => JsonSerializer.Serialize(this);
}
