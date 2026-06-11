using Moq;
using Sabemi.Application.Abstractions;
using Sabemi.Application.Features.Webhooks.ProcessPayment;
using Sabemi.Domain.Entities;
using Sabemi.Domain.Enums;
using Sabemi.Domain.Interfaces.Repositories;

namespace Sabemi.Application.UnitTests.Features.Webhooks.ProcessPayment;

[TestClass]
public class ProcessPaymentWebhookCommandHandlerTests
{
    private Mock<IPaymentWebhookEventRepository> _eventRepositoryMock = null!;
    private Mock<IContractRepository> _contractRepositoryMock = null!;
    private Mock<IContractPaymentRepository> _contractPaymentRepositoryMock = null!;
    private Mock<INotificationService> _notificationServiceMock = null!;
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private ProcessPaymentWebhookCommandHandler _handler = null!;

    [TestInitialize]
    public void Setup()
    {
        _eventRepositoryMock = new Mock<IPaymentWebhookEventRepository>();
        _contractRepositoryMock = new Mock<IContractRepository>();
        _contractPaymentRepositoryMock = new Mock<IContractPaymentRepository>();
        _notificationServiceMock = new Mock<INotificationService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new ProcessPaymentWebhookCommandHandler(
            _eventRepositoryMock.Object,
            _contractRepositoryMock.Object,
            _contractPaymentRepositoryMock.Object,
            _notificationServiceMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [TestMethod]
    public async Task Handle_WhenEventNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var command = new ProcessPaymentWebhookCommand(transactionId);
        _eventRepositoryMock
            .Setup(x => x.FindByTransactionAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PaymentWebhookEvent?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None)
        );

        Assert.AreEqual($"Evento não encontrado para a transação: {transactionId}", exception.Message);
    }

    [TestMethod]
    public async Task Handle_WhenEventStatusIsProcessing_ReturnsEarly()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var command = new ProcessPaymentWebhookCommand(transactionId);
        var webhookEvent = PaymentWebhookEvent.Create(Guid.NewGuid(), transactionId, "{}");
        webhookEvent.MarkAsProcessing();

        _eventRepositoryMock
            .Setup(x => x.FindByTransactionAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(webhookEvent);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _eventRepositoryMock.Verify(
            x => x.UpdateStatusAsync(It.IsAny<PaymentWebhookEvent>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [TestMethod]
    public async Task Handle_WhenEventStatusIsProcessed_ReturnsEarly()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var command = new ProcessPaymentWebhookCommand(transactionId);
        var webhookEvent = PaymentWebhookEvent.Create(Guid.NewGuid(), transactionId, "{}");
        webhookEvent.MarkAsProcessed();

        _eventRepositoryMock
            .Setup(x => x.FindByTransactionAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(webhookEvent);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _eventRepositoryMock.Verify(
            x => x.UpdateStatusAsync(It.IsAny<PaymentWebhookEvent>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [TestMethod]
    public async Task Handle_WhenPayloadIsEmpty_ThrowsInvalidOperationException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var command = new ProcessPaymentWebhookCommand(transactionId);
        var webhookEvent = PaymentWebhookEvent.Create(Guid.NewGuid(), transactionId, "");

        _eventRepositoryMock
            .Setup(x => x.FindByTransactionAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(webhookEvent);

        _eventRepositoryMock
            .Setup(x => x.UpdateStatusAsync(It.IsAny<PaymentWebhookEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _notificationServiceMock
            .Setup(x => x.NotifyEventChangedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None)
        );

        Assert.AreEqual($"Erro inesperado ao processar webhook para a transação: {transactionId}", exception.Message);
    }

    [TestMethod]
    public async Task Handle_WhenContractNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var contractId = Guid.NewGuid();
        var command = new ProcessPaymentWebhookCommand(transactionId);
        var payload = $"{{\"id_transacao\":\"{transactionId}\",\"id_contrato\":\"{contractId}\",\"status\":\"success\",\"valor\":100.50,\"data_pagamento\":\"2024-01-01T00:00:00Z\"}}";
        var webhookEvent = PaymentWebhookEvent.Create(contractId, transactionId, payload);

        _eventRepositoryMock
            .Setup(x => x.FindByTransactionAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(webhookEvent);

        _eventRepositoryMock
            .Setup(x => x.UpdateStatusAsync(It.IsAny<PaymentWebhookEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _notificationServiceMock
            .Setup(x => x.NotifyEventChangedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _contractRepositoryMock
            .Setup(x => x.FindByIdAsync(contractId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Contract?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None)
        );

        Assert.AreEqual($"Erro inesperado ao processar webhook para a transação: {transactionId}", exception.Message);
    }

    [TestMethod]
    public async Task Handle_WhenStatusIsNotSuccess_ThrowsInvalidOperationException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var contractId = Guid.NewGuid();
        var command = new ProcessPaymentWebhookCommand(transactionId);
        var payload = $"{{\"id_transacao\":\"{transactionId}\",\"id_contrato\":\"{contractId}\",\"status\":\"failed\",\"valor\":100.50,\"data_pagamento\":\"2024-01-01T00:00:00Z\"}}";
        var webhookEvent = PaymentWebhookEvent.Create(contractId, transactionId, payload);
        var contract = new Contract();

        _eventRepositoryMock
            .Setup(x => x.FindByTransactionAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(webhookEvent);

        _eventRepositoryMock
            .Setup(x => x.UpdateStatusAsync(It.IsAny<PaymentWebhookEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _notificationServiceMock
            .Setup(x => x.NotifyEventChangedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _contractRepositoryMock
            .Setup(x => x.FindByIdAsync(contractId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contract);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None)
        );

        Assert.AreEqual($"Erro inesperado ao processar webhook para a transação: {transactionId}", exception.Message);
    }

    [TestMethod]
    public async Task Handle_WhenAmountIsNull_ThrowsInvalidOperationException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var contractId = Guid.NewGuid();
        var command = new ProcessPaymentWebhookCommand(transactionId);
        var payload = $"{{\"id_transacao\":\"{transactionId}\",\"id_contrato\":\"{contractId}\",\"status\":\"success\",\"data_pagamento\":\"2024-01-01T00:00:00Z\"}}";
        var webhookEvent = PaymentWebhookEvent.Create(contractId, transactionId, payload);
        var contract = new Contract();

        _eventRepositoryMock
            .Setup(x => x.FindByTransactionAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(webhookEvent);

        _eventRepositoryMock
            .Setup(x => x.UpdateStatusAsync(It.IsAny<PaymentWebhookEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _notificationServiceMock
            .Setup(x => x.NotifyEventChangedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _contractRepositoryMock
            .Setup(x => x.FindByIdAsync(contractId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contract);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None)
        );

        Assert.AreEqual($"Erro inesperado ao processar webhook para a transação: {transactionId}", exception.Message);
    }

    [TestMethod]
    public async Task Handle_WhenAmountIsZero_ThrowsInvalidOperationException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var contractId = Guid.NewGuid();
        var command = new ProcessPaymentWebhookCommand(transactionId);
        var payload = $"{{\"id_transacao\":\"{transactionId}\",\"id_contrato\":\"{contractId}\",\"status\":\"success\",\"valor\":0,\"data_pagamento\":\"2024-01-01T00:00:00Z\"}}";
        var webhookEvent = PaymentWebhookEvent.Create(contractId, transactionId, payload);
        var contract = new Contract();

        _eventRepositoryMock
            .Setup(x => x.FindByTransactionAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(webhookEvent);

        _eventRepositoryMock
            .Setup(x => x.UpdateStatusAsync(It.IsAny<PaymentWebhookEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _notificationServiceMock
            .Setup(x => x.NotifyEventChangedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _contractRepositoryMock
            .Setup(x => x.FindByIdAsync(contractId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contract);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None)
        );

        Assert.AreEqual($"Erro inesperado ao processar webhook para a transação: {transactionId}", exception.Message);
    }

    [TestMethod]
    public async Task Handle_WhenPaidAtIsNull_ThrowsInvalidOperationException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var contractId = Guid.NewGuid();
        var command = new ProcessPaymentWebhookCommand(transactionId);
        var payload = $"{{\"id_transacao\":\"{transactionId}\",\"id_contrato\":\"{contractId}\",\"status\":\"success\",\"valor\":100.50}}";
        var webhookEvent = PaymentWebhookEvent.Create(contractId, transactionId, payload);
        var contract = new Contract();

        _eventRepositoryMock
            .Setup(x => x.FindByTransactionAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(webhookEvent);

        _eventRepositoryMock
            .Setup(x => x.UpdateStatusAsync(It.IsAny<PaymentWebhookEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _notificationServiceMock
            .Setup(x => x.NotifyEventChangedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _contractRepositoryMock
            .Setup(x => x.FindByIdAsync(contractId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contract);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None)
        );

        Assert.AreEqual($"Erro inesperado ao processar webhook para a transação: {transactionId}", exception.Message);
    }

    [TestMethod]
    public async Task Handle_WhenSaveChangesFails_ThrowsInvalidOperationException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var contractId = Guid.NewGuid();
        var command = new ProcessPaymentWebhookCommand(transactionId);
        var payload = $"{{\"id_transacao\":\"{transactionId}\",\"id_contrato\":\"{contractId}\",\"status\":\"success\",\"valor\":100.50,\"data_pagamento\":\"2024-01-01T00:00:00Z\"}}";
        var webhookEvent = PaymentWebhookEvent.Create(contractId, transactionId, payload);
        var contract = new Contract();

        _eventRepositoryMock
            .Setup(x => x.FindByTransactionAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(webhookEvent);

        _eventRepositoryMock
            .Setup(x => x.UpdateStatusAsync(It.IsAny<PaymentWebhookEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _eventRepositoryMock
            .Setup(x => x.Update(It.IsAny<PaymentWebhookEvent>()));

        _notificationServiceMock
            .Setup(x => x.NotifyEventChangedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _contractRepositoryMock
            .Setup(x => x.FindByIdAsync(contractId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contract);

        _contractRepositoryMock
            .Setup(x => x.Update(It.IsAny<Contract>()));

        _contractPaymentRepositoryMock
            .Setup(x => x.Add(It.IsAny<ContractPayment>()));

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None)
        );

        Assert.AreEqual($"Erro inesperado ao processar webhook para a transação: {transactionId}", exception.Message);
    }

    [TestMethod]
    public async Task Handle_WhenValidPayload_ProcessesSuccessfully()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var contractId = Guid.NewGuid();
        var command = new ProcessPaymentWebhookCommand(transactionId);
        var payload = $"{{\"id_transacao\":\"{transactionId}\",\"id_contrato\":\"{contractId}\",\"status\":\"success\",\"valor\":100.50,\"data_pagamento\":\"2024-01-01T00:00:00Z\"}}";
        var webhookEvent = PaymentWebhookEvent.Create(contractId, transactionId, payload);
        var contract = new Contract();

        _eventRepositoryMock
            .Setup(x => x.FindByTransactionAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(webhookEvent);

        _eventRepositoryMock
            .Setup(x => x.UpdateStatusAsync(It.IsAny<PaymentWebhookEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _eventRepositoryMock
            .Setup(x => x.Update(It.IsAny<PaymentWebhookEvent>()));

        _notificationServiceMock
            .Setup(x => x.NotifyEventChangedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _notificationServiceMock
            .Setup(x => x.NotifyContractChangedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _contractRepositoryMock
            .Setup(x => x.FindByIdAsync(contractId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contract);

        _contractRepositoryMock
            .Setup(x => x.Update(It.IsAny<Contract>()));

        _contractPaymentRepositoryMock
            .Setup(x => x.Add(It.IsAny<ContractPayment>()));

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _eventRepositoryMock.Verify(
            x => x.UpdateStatusAsync(webhookEvent, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _eventRepositoryMock.Verify(
            x => x.Update(webhookEvent),
            Times.Once
        );

        _contractRepositoryMock.Verify(
            x => x.Update(contract),
            Times.Once
        );

        _contractPaymentRepositoryMock.Verify(
            x => x.Add(It.IsAny<ContractPayment>()),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );

        _notificationServiceMock.Verify(
            x => x.NotifyEventChangedAsync(transactionId, It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );

        _notificationServiceMock.Verify(
            x => x.NotifyContractChangedAsync(contractId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [TestMethod]
    public async Task Handle_WhenCancellationRequested_ThrowsInvalidOperationException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var contractId = Guid.NewGuid();
        var command = new ProcessPaymentWebhookCommand(transactionId);
        var payload = $"{{\"id_transacao\":\"{transactionId}\",\"id_contrato\":\"{contractId}\",\"status\":\"success\",\"valor\":100.50,\"data_pagamento\":\"2024-01-01T00:00:00Z\"}}";
        var webhookEvent = PaymentWebhookEvent.Create(contractId, transactionId, payload);
        var cts = new CancellationTokenSource();

        _eventRepositoryMock
            .Setup(x => x.FindByTransactionAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(webhookEvent);

        _eventRepositoryMock
            .Setup(x => x.UpdateStatusAsync(It.IsAny<PaymentWebhookEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _notificationServiceMock
            .Setup(x => x.NotifyEventChangedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        cts.Cancel();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, cts.Token)
        );

        Assert.AreEqual($"Erro inesperado ao processar webhook para a transação: {transactionId}", exception.Message);
    }

    [TestMethod]
    public async Task Handle_WhenStatusIsEmptyString_ThrowsInvalidOperationException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var contractId = Guid.NewGuid();
        var command = new ProcessPaymentWebhookCommand(transactionId);
        var payload = $"{{\"id_transacao\":\"{transactionId}\",\"id_contrato\":\"{contractId}\",\"status\":\"\",\"valor\":100.50,\"data_pagamento\":\"2024-01-01T00:00:00Z\"}}";
        var webhookEvent = PaymentWebhookEvent.Create(contractId, transactionId, payload);
        var contract = new Contract();

        _eventRepositoryMock
            .Setup(x => x.FindByTransactionAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(webhookEvent);

        _eventRepositoryMock
            .Setup(x => x.UpdateStatusAsync(It.IsAny<PaymentWebhookEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _notificationServiceMock
            .Setup(x => x.NotifyEventChangedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _contractRepositoryMock
            .Setup(x => x.FindByIdAsync(contractId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contract);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None)
        );

        Assert.AreEqual($"Erro inesperado ao processar webhook para a transação: {transactionId}", exception.Message);
    }

    [TestMethod]
    public async Task Handle_WhenAmountIsNegative_ThrowsInvalidOperationException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var contractId = Guid.NewGuid();
        var command = new ProcessPaymentWebhookCommand(transactionId);
        var payload = $"{{\"id_transacao\":\"{transactionId}\",\"id_contrato\":\"{contractId}\",\"status\":\"success\",\"valor\":-100.50,\"data_pagamento\":\"2024-01-01T00:00:00Z\"}}";
        var webhookEvent = PaymentWebhookEvent.Create(contractId, transactionId, payload);
        var contract = new Contract();

        _eventRepositoryMock
            .Setup(x => x.FindByTransactionAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(webhookEvent);

        _eventRepositoryMock
            .Setup(x => x.UpdateStatusAsync(It.IsAny<PaymentWebhookEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _notificationServiceMock
            .Setup(x => x.NotifyEventChangedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _contractRepositoryMock
            .Setup(x => x.FindByIdAsync(contractId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contract);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None)
        );

        Assert.AreEqual($"Erro inesperado ao processar webhook para a transação: {transactionId}", exception.Message);
    }

    [TestMethod]
    public async Task Handle_WhenPayloadIsWhitespace_ThrowsInvalidOperationException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var command = new ProcessPaymentWebhookCommand(transactionId);
        var webhookEvent = PaymentWebhookEvent.Create(Guid.NewGuid(), transactionId, "   ");

        _eventRepositoryMock
            .Setup(x => x.FindByTransactionAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(webhookEvent);

        _eventRepositoryMock
            .Setup(x => x.UpdateStatusAsync(It.IsAny<PaymentWebhookEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _notificationServiceMock
            .Setup(x => x.NotifyEventChangedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None)
        );

        Assert.AreEqual($"Erro inesperado ao processar webhook para a transação: {transactionId}", exception.Message);
    }
}
