using AutoMapper;
using Moq;
using Sabemi.Application.Abstractions;
using Sabemi.Application.Features.Contracts.CreateContract;
using Sabemi.Domain.Entities;
using Sabemi.Domain.Interfaces.Repositories;

namespace Sabemi.Application.UnitTests.Features.Contracts.CreateContract;

[TestClass]
public class CreateContractCommandHandlerTests
{
    private readonly Mock<IContractRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly CreateContractCommandHandler _handler;

    public CreateContractCommandHandlerTests()
    {
        _repositoryMock = new Mock<IContractRepository>();
        _mapperMock = new Mock<IMapper>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _notificationServiceMock = new Mock<INotificationService>();
        _handler = new CreateContractCommandHandler(
            _repositoryMock.Object,
            _mapperMock.Object,
            _unitOfWorkMock.Object,
            _notificationServiceMock.Object);
    }

    [TestMethod]
    public async Task Handle_ValidRequest_ReturnsContractId()
    {
        // Arrange
        var request = new CreateContractCommand
        {
            Name = "Test Contract",
            TotalAmount = 1000.00m
        };
        var cancellationToken = CancellationToken.None;

        _repositoryMock
            .Setup(r => r.ExistsByNameAsync(request.Name, cancellationToken))
            .ReturnsAsync(false);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(cancellationToken))
            .ReturnsAsync(true);

        _notificationServiceMock
            .Setup(n => n.NotifyContractCreatedAsync(It.IsAny<Guid>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        Assert.AreNotEqual(Guid.Empty, result);
        _repositoryMock.Verify(r => r.ExistsByNameAsync(request.Name, cancellationToken), Times.Once);
        _repositoryMock.Verify(r => r.Add(It.IsAny<Contract>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(cancellationToken), Times.Once);
        _notificationServiceMock.Verify(n => n.NotifyContractCreatedAsync(result, cancellationToken), Times.Once);
    }

    [TestMethod]
    public async Task Handle_DuplicateContractName_ThrowsInvalidOperationException()
    {
        // Arrange
        var request = new CreateContractCommand
        {
            Name = "Existing Contract",
            TotalAmount = 1000.00m
        };
        var cancellationToken = CancellationToken.None;

        _repositoryMock
            .Setup(r => r.ExistsByNameAsync(request.Name, cancellationToken))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.Handle(request, cancellationToken));

        Assert.AreEqual($"O contrato com o nome '{request.Name}' já existe.", exception.Message);
        _repositoryMock.Verify(r => r.ExistsByNameAsync(request.Name, cancellationToken), Times.Once);
        _repositoryMock.Verify(r => r.Add(It.IsAny<Contract>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _notificationServiceMock.Verify(n => n.NotifyContractCreatedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_SaveChangesFails_ThrowsInvalidOperationException()
    {
        // Arrange
        var request = new CreateContractCommand
        {
            Name = "Test Contract",
            TotalAmount = 1000.00m
        };
        var cancellationToken = CancellationToken.None;

        _repositoryMock
            .Setup(r => r.ExistsByNameAsync(request.Name, cancellationToken))
            .ReturnsAsync(false);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(cancellationToken))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.Handle(request, cancellationToken));

        Assert.AreEqual("Ocorreu um erro ao salvar o contrato. Por favor, tente novamente.", exception.Message);
        _repositoryMock.Verify(r => r.ExistsByNameAsync(request.Name, cancellationToken), Times.Once);
        _repositoryMock.Verify(r => r.Add(It.IsAny<Contract>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(cancellationToken), Times.Once);
        _notificationServiceMock.Verify(n => n.NotifyContractCreatedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
