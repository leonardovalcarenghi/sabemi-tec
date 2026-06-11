using Hangfire;
using Moq;
using Sabemi.Application.BackgroundJobs;
using Sabemi.Application.Features.Webhooks.ReprocessPayment;
using Sabemi.Domain.Entities;
using Sabemi.Domain.Enums;
using Sabemi.Domain.Exceptions;
using Sabemi.Domain.Interfaces.Repositories;

namespace Sabemi.Application.UnitTests.Features.Webhooks.ReprocessPayment;

[TestClass]
public class ReprocessPaymentEventCommandHandlerTests
{
    private Mock<IPaymentWebhookEventRepository> _repositoryMock = null!;
    private Mock<IBackgroundJobClient> _jobClientMock = null!;
    private ReprocessPaymentEventCommandHandler _handler = null!;

    [TestInitialize]
    public void Setup()
    {
        _repositoryMock = new Mock<IPaymentWebhookEventRepository>();
        _jobClientMock = new Mock<IBackgroundJobClient>();

        _handler = new ReprocessPaymentEventCommandHandler(
            _repositoryMock.Object,
            _jobClientMock.Object
        );
    }

    [TestMethod]
    public async Task Handle_WhenEventNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var command = new ReprocessPaymentEventCommand { TransactionId = transactionId };
        _repositoryMock
            .Setup(x => x.FindByTransactionAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PaymentWebhookEvent?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            async () => await _handler.Handle(command, CancellationToken.None)
        );

        Assert.AreEqual($"Evento de webhook para a transação {transactionId} não encontrado.", exception.Message);
    }

    [TestMethod]
    public async Task Handle_WhenEventStatusIsNotFailed_ThrowsInvalidOperationException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var command = new ReprocessPaymentEventCommand { TransactionId = transactionId };
        var paymentEvent = PaymentWebhookEvent.Create(Guid.NewGuid(), transactionId, "{}");
        paymentEvent.MarkAsProcessed();

        _repositoryMock
            .Setup(x => x.FindByTransactionAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentEvent);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None)
        );

        Assert.AreEqual($"Evento de webhook para a transação {transactionId} não está em estado 'Failed' e não pode ser reprocessado.", exception.Message);
    }

    [TestMethod]
    public async Task Handle_WhenEventStatusIsPending_ThrowsInvalidOperationException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var command = new ReprocessPaymentEventCommand { TransactionId = transactionId };
        var paymentEvent = PaymentWebhookEvent.Create(Guid.NewGuid(), transactionId, "{}");

        _repositoryMock
            .Setup(x => x.FindByTransactionAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentEvent);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None)
        );

        Assert.AreEqual($"Evento de webhook para a transação {transactionId} não está em estado 'Failed' e não pode ser reprocessado.", exception.Message);
    }

    [TestMethod]
    public async Task Handle_WhenEventStatusIsProcessing_ThrowsInvalidOperationException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var command = new ReprocessPaymentEventCommand { TransactionId = transactionId };
        var paymentEvent = PaymentWebhookEvent.Create(Guid.NewGuid(), transactionId, "{}");
        paymentEvent.MarkAsProcessing();

        _repositoryMock
            .Setup(x => x.FindByTransactionAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentEvent);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None)
        );

        Assert.AreEqual($"Evento de webhook para a transação {transactionId} não está em estado 'Failed' e não pode ser reprocessado.", exception.Message);
    }

    [TestMethod]
    public async Task Handle_WhenEventStatusIsFailed_MarksAsPendingAndUpdatesStatus()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var command = new ReprocessPaymentEventCommand { TransactionId = transactionId };
        var paymentEvent = PaymentWebhookEvent.Create(Guid.NewGuid(), transactionId, "{}");
        paymentEvent.MarkAsFailed("Some error");

        _repositoryMock
            .Setup(x => x.FindByTransactionAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentEvent);
        _repositoryMock
            .Setup(x => x.UpdateStatusAsync(paymentEvent, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.AreEqual(WebhookEventStatus.Pending, paymentEvent.Status);
        _repositoryMock.Verify(x => x.UpdateStatusAsync(paymentEvent, It.IsAny<CancellationToken>()), Times.Once);
        _jobClientMock.Verify(x => x.Create(It.IsAny<Hangfire.Common.Job>(), It.IsAny<Hangfire.States.IState>()), Times.Once);
    }

    [TestMethod]
    public async Task Handle_WhenEventStatusIsNone_ThrowsInvalidOperationException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var command = new ReprocessPaymentEventCommand { TransactionId = transactionId };
        var paymentEvent = new PaymentWebhookEvent();

        _repositoryMock
            .Setup(x => x.FindByTransactionAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentEvent);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None)
        );

        Assert.AreEqual($"Evento de webhook para a transação {transactionId} não está em estado 'Failed' e não pode ser reprocessado.", exception.Message);
    }
}
