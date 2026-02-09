namespace UnitTests.Application.Services;

using global::Application.DTOs.Contract;
using global::Application.Services;
using global::Domain.Entities;
using global::Domain.Interfaces;

using Moq;

using Xunit;

public class ContractServiceTests
{
    private readonly Mock<IContractRepository> _contractRepositoryMock;
    private readonly Mock<IPaymentPlanRepository> _paymentPlanRepositoryMock;
    private readonly ContractService _service;

    public ContractServiceTests()
    {
        _contractRepositoryMock = new Mock<IContractRepository>();
        _paymentPlanRepositoryMock = new Mock<IPaymentPlanRepository>();
        _service = new ContractService(_contractRepositoryMock.Object, _paymentPlanRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsContract()
    {
        // Arrange
        var plan = new PaymentPlan("Basic", "Description", 99.99m, BillingCycle.Monthly, PlanTier.Basic);
        var contract = new Contract("CNT-001", "Customer", "test@example.com", DateTime.UtcNow, plan);

        _contractRepositoryMock
            .Setup(x => x.GetByIdAsync(contract.Id))
            .ReturnsAsync(contract);

        // Act
        var result = await _service.GetByIdAsync(contract.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(contract.Id, result.Id);
        Assert.Equal("CNT-001", result.ContractNumber);
        _contractRepositoryMock.Verify(x => x.GetByIdAsync(contract.Id), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ReturnsNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();
        _contractRepositoryMock
            .Setup(x => x.GetByIdAsync(nonExistingId))
            .ReturnsAsync((Contract?)null);

        // Act
        var result = await _service.GetByIdAsync(nonExistingId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesContract()
    {
        // Arrange
        var plan = new PaymentPlan("Basic", "Description", 99.99m, BillingCycle.Monthly, PlanTier.Basic);
        var dto = new CreateContractDto
        {
            ContractNumber = "CNT-001",
            CustomerName = "Customer",
            CustomerEmail = "test@example.com",
            StartDate = DateTime.Today,
            InitialPaymentPlanId = plan.Id
        };

        _paymentPlanRepositoryMock
            .Setup(x => x.GetByIdAsync(plan.Id))
            .ReturnsAsync(plan);

        _contractRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Contract>()))
            .ReturnsAsync((Contract c) => c);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("CNT-001", result.ContractNumber);
        Assert.Equal("Customer", result.CustomerName);
        Assert.Equal("Active", result.Status);
        _paymentPlanRepositoryMock.Verify(x => x.GetByIdAsync(plan.Id), Times.Once);
        _contractRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Contract>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNonExistingPlan_ThrowsInvalidOperationException()
    {
        // Arrange
        var dto = new CreateContractDto
        {
            ContractNumber = "CNT-001",
            CustomerName = "Customer",
            CustomerEmail = "test@example.com",
            StartDate = DateTime.Today,
            InitialPaymentPlanId = Guid.NewGuid()
        };

        _paymentPlanRepositoryMock
            .Setup(x => x.GetByIdAsync(dto.InitialPaymentPlanId))
            .ReturnsAsync((PaymentPlan?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_WithInactivePlan_ThrowsInvalidOperationException()
    {
        // Arrange
        var plan = new PaymentPlan("Basic", "Description", 99.99m, BillingCycle.Monthly, PlanTier.Basic);
        plan.Deactivate();

        var dto = new CreateContractDto
        {
            ContractNumber = "CNT-001",
            CustomerName = "Customer",
            CustomerEmail = "test@example.com",
            StartDate = DateTime.Today,
            InitialPaymentPlanId = plan.Id
        };

        _paymentPlanRepositoryMock
            .Setup(x => x.GetByIdAsync(plan.Id))
            .ReturnsAsync(plan);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.CreateAsync(dto));
        Assert.Contains("inactive payment plan", exception.Message);
    }

    [Fact]
    public async Task TerminateAsync_WithExistingContract_TerminatesContract()
    {
        // Arrange
        var plan = new PaymentPlan("Basic", "Description", 99.99m, BillingCycle.Monthly, PlanTier.Basic);
        var contract = new Contract("CNT-001", "Customer", "test@example.com", DateTime.Today, plan);
        var endDate = DateTime.Today.AddMonths(1);

        _contractRepositoryMock
            .Setup(x => x.GetByIdAsync(contract.Id))
            .ReturnsAsync(contract);

        _contractRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Contract>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.TerminateAsync(contract.Id, endDate);

        // Assert
        Assert.True(result);
        Assert.Equal(ContractStatus.Terminated, contract.Status);
        Assert.Equal(endDate, contract.EndDate);
        _contractRepositoryMock.Verify(x => x.UpdateAsync(contract), Times.Once);
    }

    [Fact]
    public async Task SuspendAsync_WithExistingContract_SuspendsContract()
    {
        // Arrange
        var plan = new PaymentPlan("Basic", "Description", 99.99m, BillingCycle.Monthly, PlanTier.Basic);
        var contract = new Contract("CNT-001", "Customer", "test@example.com", DateTime.Today, plan);

        _contractRepositoryMock
            .Setup(x => x.GetByIdAsync(contract.Id))
            .ReturnsAsync(contract);

        _contractRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Contract>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.SuspendAsync(contract.Id);

        // Assert
        Assert.True(result);
        Assert.Equal(ContractStatus.Suspended, contract.Status);
        _contractRepositoryMock.Verify(x => x.UpdateAsync(contract), Times.Once);
    }

    [Fact]
    public async Task ReactivateAsync_WithSuspendedContract_ReactivatesContract()
    {
        // Arrange
        var plan = new PaymentPlan("Basic", "Description", 99.99m, BillingCycle.Monthly, PlanTier.Basic);
        var contract = new Contract("CNT-001", "Customer", "test@example.com", DateTime.Today, plan);
        contract.Suspend();

        _contractRepositoryMock
            .Setup(x => x.GetByIdAsync(contract.Id))
            .ReturnsAsync(contract);

        _contractRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Contract>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ReactivateAsync(contract.Id);

        // Assert
        Assert.True(result);
        Assert.Equal(ContractStatus.Active, contract.Status);
        _contractRepositoryMock.Verify(x => x.UpdateAsync(contract), Times.Once);
    }
}