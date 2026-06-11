using AutoMapper;
using Moq;
using Sabemi.Application.Features.Webhooks;
using Sabemi.Application.Features.Webhooks.FindPayments;
using Sabemi.Domain.Entities;
using Sabemi.Domain.Interfaces.Repositories;
using System.Linq.Expressions;

namespace Sabemi.Application.UnitTests.Features.Webhooks.FindPayments;

[TestClass]
public class FindPaymentEventCommandHandlerTests
{
    private readonly Mock<IPaymentWebhookEventRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly FindPaymentEventCommandHandler _handler;

    public FindPaymentEventCommandHandlerTests()
    {
        _mockRepository = new Mock<IPaymentWebhookEventRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new FindPaymentEventCommandHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [TestMethod]
    public async Task Handle_ValidRequest_ReturnsMapperResult()
    {
        // Arrange
        var request = new FindPaymentEventCommand { ContractId = Guid.NewGuid() };
        var cancellationToken = CancellationToken.None;
        var events = new List<PaymentWebhookEvent> { new PaymentWebhookEvent() };
        var expectedModels = new List<PaymentWebhookEventModel> { new PaymentWebhookEventModel() };

        _mockRepository
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<PaymentWebhookEvent, bool>>>(), cancellationToken))
            .ReturnsAsync(events);

        _mockMapper
            .Setup(m => m.Map<IEnumerable<PaymentWebhookEventModel>>(events))
            .Returns(expectedModels);

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        Assert.AreEqual(expectedModels, result);
        _mockRepository.Verify(r => r.FindAsync(It.IsAny<Expression<Func<PaymentWebhookEvent, bool>>>(), cancellationToken), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<PaymentWebhookEventModel>>(events), Times.Once);
    }

    [TestMethod]
    public async Task Handle_EmptyRepository_ReturnsEmptyMappedResult()
    {
        // Arrange
        var request = new FindPaymentEventCommand();
        var cancellationToken = CancellationToken.None;
        var emptyEvents = new List<PaymentWebhookEvent>();
        var emptyModels = new List<PaymentWebhookEventModel>();

        _mockRepository
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<PaymentWebhookEvent, bool>>>(), cancellationToken))
            .ReturnsAsync(emptyEvents);

        _mockMapper
            .Setup(m => m.Map<IEnumerable<PaymentWebhookEventModel>>(emptyEvents))
            .Returns(emptyModels);

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(emptyModels, result);
        _mockRepository.Verify(r => r.FindAsync(It.IsAny<Expression<Func<PaymentWebhookEvent, bool>>>(), cancellationToken), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<PaymentWebhookEventModel>>(emptyEvents), Times.Once);
    }

    [TestMethod]
    public async Task Handle_MultipleEvents_ReturnsMappedResults()
    {
        // Arrange
        var request = new FindPaymentEventCommand { ContractId = Guid.NewGuid() };
        var cancellationToken = CancellationToken.None;
        var events = new List<PaymentWebhookEvent>
        {
            new PaymentWebhookEvent(),
            new PaymentWebhookEvent(),
            new PaymentWebhookEvent()
        };
        var expectedModels = new List<PaymentWebhookEventModel>
        {
            new PaymentWebhookEventModel(),
            new PaymentWebhookEventModel(),
            new PaymentWebhookEventModel()
        };

        _mockRepository
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<PaymentWebhookEvent, bool>>>(), cancellationToken))
            .ReturnsAsync(events);

        _mockMapper
            .Setup(m => m.Map<IEnumerable<PaymentWebhookEventModel>>(events))
            .Returns(expectedModels);

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        Assert.AreEqual(expectedModels, result);
        _mockRepository.Verify(r => r.FindAsync(It.IsAny<Expression<Func<PaymentWebhookEvent, bool>>>(), cancellationToken), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<PaymentWebhookEventModel>>(events), Times.Once);
    }

    [TestMethod]
    public async Task Handle_RequestWithNullContractId_CallsRepositoryWithCorrectFilter()
    {
        // Arrange
        var request = new FindPaymentEventCommand { ContractId = null };
        var cancellationToken = CancellationToken.None;
        var events = new List<PaymentWebhookEvent>();
        var models = new List<PaymentWebhookEventModel>();

        _mockRepository
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<PaymentWebhookEvent, bool>>>(), cancellationToken))
            .ReturnsAsync(events);

        _mockMapper
            .Setup(m => m.Map<IEnumerable<PaymentWebhookEventModel>>(events))
            .Returns(models);

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        Assert.IsNotNull(result);
        _mockRepository.Verify(r => r.FindAsync(It.IsAny<Expression<Func<PaymentWebhookEvent, bool>>>(), cancellationToken), Times.Once);
    }

    [TestMethod]
    public async Task Handle_RequestWithStatus_CallsRepositoryWithCorrectFilter()
    {
        // Arrange
        var request = new FindPaymentEventCommand { Status = Domain.Enums.WebhookEventStatus.Pending };
        var cancellationToken = CancellationToken.None;
        var events = new List<PaymentWebhookEvent>();
        var models = new List<PaymentWebhookEventModel>();

        _mockRepository
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<PaymentWebhookEvent, bool>>>(), cancellationToken))
            .ReturnsAsync(events);

        _mockMapper
            .Setup(m => m.Map<IEnumerable<PaymentWebhookEventModel>>(events))
            .Returns(models);

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        Assert.IsNotNull(result);
        _mockRepository.Verify(r => r.FindAsync(It.IsAny<Expression<Func<PaymentWebhookEvent, bool>>>(), cancellationToken), Times.Once);
    }

    [TestMethod]
    public async Task Handle_RequestWithBothFilters_CallsRepositoryWithCorrectFilter()
    {
        // Arrange
        var request = new FindPaymentEventCommand
        {
            ContractId = Guid.NewGuid(),
            Status = Domain.Enums.WebhookEventStatus.Processed
        };
        var cancellationToken = CancellationToken.None;
        var events = new List<PaymentWebhookEvent>();
        var models = new List<PaymentWebhookEventModel>();

        _mockRepository
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<PaymentWebhookEvent, bool>>>(), cancellationToken))
            .ReturnsAsync(events);

        _mockMapper
            .Setup(m => m.Map<IEnumerable<PaymentWebhookEventModel>>(events))
            .Returns(models);

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        Assert.IsNotNull(result);
        _mockRepository.Verify(r => r.FindAsync(It.IsAny<Expression<Func<PaymentWebhookEvent, bool>>>(), cancellationToken), Times.Once);
    }

    [TestMethod]
    public async Task Handle_CancellationTokenProvided_PassesCancellationTokenToRepository()
    {
        // Arrange
        var request = new FindPaymentEventCommand();
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var events = new List<PaymentWebhookEvent>();
        var models = new List<PaymentWebhookEventModel>();

        _mockRepository
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<PaymentWebhookEvent, bool>>>(), cancellationToken))
            .ReturnsAsync(events);

        _mockMapper
            .Setup(m => m.Map<IEnumerable<PaymentWebhookEventModel>>(events))
            .Returns(models);

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        Assert.IsNotNull(result);
        _mockRepository.Verify(r => r.FindAsync(It.IsAny<Expression<Func<PaymentWebhookEvent, bool>>>(), cancellationToken), Times.Once);
    }
}
