using Hangfire;
using Moq;
using Sabemi.Application.Abstractions;
using Sabemi.Application.BackgroundJobs;
using Sabemi.Application.Features.Webhooks.ReceivePayment;
using Sabemi.Domain.Entities;
using Sabemi.Domain.Interfaces.Repositories;

namespace Sabemi.Application.UnitTests.Features.Webhooks.ReceivePayment;

[TestClass]
public class ReceivePaymentWebhookCommandHandlerTests
{
    private Mock<IPaymentWebhookEventRepository> _repositoryMock = null!;
    private Mock<IBackgroundJobClient> _jobClientMock = null!;
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private Mock<INotificationService> _notificationServiceMock = null!;
    private ReceivePaymentWebhookCommandHandler _handler = null!;

    [TestInitialize]
    public void Setup()
    {
        _repositoryMock = new Mock<IPaymentWebhookEventRepository>();
        _jobClientMock = new Mock<IBackgroundJobClient>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _notificationServiceMock = new Mock<INotificationService>();

        _handler = new ReceivePaymentWebhookCommandHandler(
            _repositoryMock.Object,
            _jobClientMock.Object,
            _unitOfWorkMock.Object,
            _notificationServiceMock.Object
        );
    }

    [TestMethod]
    public async Task Handle_WhenTransactionExists_ReturnsEarly()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var contractId = Guid.NewGuid();
        var command = new ReceivePaymentWebhookCommand
        {
            TransactionId = transactionId,
            ContractId = contractId,
            Status = "success"
        };

        _repositoryMock
            .Setup(r => r.ExistsAsync(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.ExistsAsync(transactionId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Add(It.IsAny<PaymentWebhookEvent>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _notificationServiceMock.Verify(n => n.NotifyEventCreatedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        // Note: Cannot verify extension method Enqueue with Moq. The verifications above are sufficient to confirm early return.
    }
}
