using Hangfire;
using Sabemi.Application.Abstractions;
using Sabemi.Application.BackgroundJobs.Dtos;
using Sabemi.Domain.Entities;
using Sabemi.Domain.Interfaces.Repositories;
using System.Text.Json;
namespace Sabemi.Application.BackgroundJobs;

public class ProcessPaymentWebhookEventJob(
    IPaymentWebhookEventRepository eventRepository,
    IContractRepository contractRepository,
    INotificationService notificationService
)
{
    [AutomaticRetry(Attempts = 3)]
    public async Task ExecuteAsync(Guid transactionId, CancellationToken cancellationToken)
    {
        PaymentWebhookEvent? webhookEvent = await eventRepository.FindByTransaction(transactionId, cancellationToken);

        try
        {
            if (webhookEvent is null)
            {
                throw new InvalidOperationException("Evento de webhook não encontrado para a transação: " + transactionId);
            }

            if (webhookEvent.Status is Domain.Enums.WebhookEventStatus.Processing)
            {
                return; // O evento já está sendo processado por outro processo, então apenas sair sem fazer nada.
            }

            // Marcar o evento como "Processing" para evitar que outros processos tentem processá-lo simultaneamente.
            webhookEvent.MarkAsProcessing();
            await eventRepository.UpdateAsync(webhookEvent, cancellationToken);

            // 🕑 Aguardar 7 segundos (simulando processamento pesado)
            await Task.Delay(7_000, cancellationToken);

            // Verificar se o payload do evento de webhook está presente e é válido.
            if (string.IsNullOrEmpty(webhookEvent.Payload))
            {
                webhookEvent.MarkAsFailed("Payload do evento de webhook está vazio.");
                throw new InvalidOperationException("Payload do evento de webhook está vazio para a transação: " + transactionId);
            }

            // Extrair os dados do payload do evento de webhook e validar as informações necessárias.
            PaymentWebhookPayloadDto data = JsonSerializer.Deserialize<PaymentWebhookPayloadDto>(webhookEvent.Payload)!;

            // Verificar se o contrato associado ao evento de webhook existe no banco de dados.
            Contract? contract = await contractRepository.FindByIdAsync(data.ContractId, cancellationToken);
            if (contract is null)
            {
                webhookEvent.MarkAsFailed("Contrato associado ao evento de webhook não encontrado.");
                throw new InvalidOperationException("Contrato associado ao evento de webhook não encontrado para a transação: " + transactionId);
            }

            // Verificar se o status do evento de webhook é válido e processar o evento de acordo com o status.
            if (string.IsNullOrEmpty(data.Status))
            {
                webhookEvent.MarkAsFailed("Status do evento de webhook está vazio.");
                throw new InvalidOperationException("Status do evento de webhook é inválido para a transação: " + transactionId);
            }

            if (data.Status == "success")
            {
                contract.AddPayment(transactionId, data.Amount ?? 0, data.PaidAt ?? DateTime.UtcNow);
                webhookEvent.MarkAsProcessed();
                await eventRepository.UpdateAsync(webhookEvent, cancellationToken);
                await notificationService.NotifyPaymentWebhookChangedAsync(data.ContractId, cancellationToken);
            }
            else
            {
                webhookEvent.MarkAsFailed($"Status do evento de webhook é '{data.Status}' e não 'success'.");
                throw new InvalidOperationException($"Status do evento de webhook é '{data.Status}' e não 'success' para a transação: {transactionId}");
            }
        }
        catch (InvalidOperationException)
        {
            if (webhookEvent is not null)
            {
                // Salvar o erro no banco de dados:
                await eventRepository.UpdateAsync(webhookEvent, cancellationToken);

                // Notificar o front:
                await notificationService.NotifyErrorOnPaymentWebhookProcessingAsync(transactionId, webhookEvent.ErrorMessage!, cancellationToken);
            }

            throw;
        }
        catch
        {
            throw new InvalidOperationException("Ocorreu um erro ao processar o evento de webhook para a transação: " + transactionId);
        }
    }
}