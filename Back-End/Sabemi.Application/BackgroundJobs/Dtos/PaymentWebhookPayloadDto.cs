using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Sabemi.Application.BackgroundJobs.Dtos;

internal class PaymentWebhookPayloadDto
{
    [JsonPropertyName("id_transacao")]
    public Guid TransactionId { get; set; }

    [JsonPropertyName("id_contrato")]
    public Guid ContractId { get; set; }

    // To do: aqui eu poderia criar um enum, se der tempo faço o ajuste.
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("valor")]
    public decimal? Amount { get; set; }

    [JsonPropertyName("data_pagamento")]
    public DateTime? PaidAt { get; set; }
}
