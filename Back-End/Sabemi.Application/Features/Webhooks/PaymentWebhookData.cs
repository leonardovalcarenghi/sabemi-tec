using System.Text.Json.Serialization;

namespace Sabemi.Application.Features.Webhooks;

public class PaymentWebhookData
{
    [JsonPropertyName("id_transacao")]
    public Guid TransactionId { get; set; }

    [JsonPropertyName("id_contrato")]
    public Guid ContractId { get; set; }

    // To do: aqui eu poderia criar um enum, se der tempo faço o ajuste.
    [JsonPropertyName("status")]
    public virtual string? Status { get; set; }

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
}
