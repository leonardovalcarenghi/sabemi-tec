using MediatR;
using Sabemi.Application.Abstractions;
using Sabemi.Domain.Entities;
using Sabemi.Domain.Enums;
using Sabemi.Domain.Interfaces.Repositories;
using System.Text.Json;
namespace Sabemi.Application.Features.Webhooks.ProcessPayment;

public class ProcessPaymentWebhookCommandHandler(
    IPaymentWebhookEventRepository eventRepository,
    IContractRepository contractRepository,
    IContractPaymentRepository contractPaymentRepository,
    INotificationService notificationService,
    IUnitOfWork unitOfWork
) : IRequestHandler<ProcessPaymentWebhookCommand>
{
    public async Task Handle(ProcessPaymentWebhookCommand command, CancellationToken cancellationToken)
    {
        PaymentWebhookEvent webhookEvent = await eventRepository.FindByTransactionAsync(command.TransactionId, cancellationToken)
            ?? throw new InvalidOperationException($"Evento não encontrado para a transação: {command.TransactionId}");

        if (webhookEvent.Status is WebhookEventStatus.Processing or WebhookEventStatus.Processed)
            return;

        try
        {
            await MarkAsProcessing(webhookEvent, cancellationToken);
            await Task.Delay(5_000, cancellationToken); // ⏳ Delay de 5 segundos para simular um processamento pesado.

            var data = await DeserializePayload(webhookEvent);
            var contract = await GetContractAsync(webhookEvent, data.ContractId, cancellationToken);

            await ProcessPaymentAsync(webhookEvent, contract, data, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch
        {
            throw new InvalidOperationException($"Erro inesperado ao processar webhook para a transação: {command.TransactionId}");
        }
    }



    private async Task<PaymentWebhookData> DeserializePayload(PaymentWebhookEvent webhookEvent)
    {
        if (string.IsNullOrWhiteSpace(webhookEvent.Payload))
        {
            await MarkAsFailed(webhookEvent, "Payload do webhook está vazio.", CancellationToken.None);
            throw new InvalidOperationException($"Payload vazio para a transação: {webhookEvent.TransactionId}");
        }

        return JsonSerializer.Deserialize<PaymentWebhookData>(webhookEvent.Payload)!;
    }

    private async Task<Contract> GetContractAsync(PaymentWebhookEvent webhookEvent, Guid contractId, CancellationToken ct)
    {
        Contract? contract = await contractRepository.FindByIdAsync(contractId, ct);

        if (contract is null)
        {
            await MarkAsFailed(webhookEvent, $"Contrato com ID '{contractId}' não encontrado.", CancellationToken.None);
            throw new InvalidOperationException($"Contrato não encontrado para a transação: {webhookEvent.TransactionId}");
        }

        return contract;
    }


    private async Task ProcessPaymentAsync(PaymentWebhookEvent webhookEvent, Contract contract, PaymentWebhookData data, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(data.Status))
        {
            await MarkAsFailed(webhookEvent, "Status do webhook está vazio.", cancellationToken);
            throw new InvalidOperationException($"Status inválido para a transação: {webhookEvent.TransactionId}");
        }

        if (data.Status != "success")
        {
            await MarkAsFailed(webhookEvent, $"Status '{data.Status}' não é 'success'.", cancellationToken);
            throw new InvalidOperationException($"Status '{data.Status}' para a transação: {webhookEvent.TransactionId}");
        }

        // Adicionar pagamento ao contrato:
        ContractPayment payment = contract.AddPayment(webhookEvent.TransactionId, data.Amount ?? 0, data.PaidAt ?? DateTime.UtcNow);
        contractPaymentRepository.Add(payment);
        contractRepository.Update(contract);

        // Marcar evento como processado:s
        webhookEvent.MarkAsProcessed();
        eventRepository.Update(webhookEvent);

        if (!await unitOfWork.SaveChangesAsync(cancellationToken))
            throw new InvalidOperationException($"Falha ao salvar as alterações para a transação: {webhookEvent.TransactionId}");

        await notificationService.NotifyEventChangedAsync(webhookEvent.TransactionId, cancellationToken);
        await notificationService.NotifyContractChangedAsync(webhookEvent.ContractId, cancellationToken);
    }

    private async Task MarkAsProcessing(PaymentWebhookEvent webhookEvent, CancellationToken cancellationToken)
    {
        webhookEvent.MarkAsProcessing();
        await eventRepository.UpdateStatusAsync(webhookEvent, cancellationToken);
        await notificationService.NotifyEventChangedAsync(webhookEvent.TransactionId, cancellationToken);
    }

    private async Task MarkAsFailed(PaymentWebhookEvent webhookEvent, string errorMessage, CancellationToken cancellationToken)
    {
        webhookEvent.MarkAsFailed(errorMessage);
        await eventRepository.UpdateStatusAsync(webhookEvent, cancellationToken);
        await notificationService.NotifyEventChangedAsync(webhookEvent.TransactionId, cancellationToken);
    }
}