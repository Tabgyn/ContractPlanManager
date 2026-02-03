namespace UnitTests.Application.Services;

using global::Application.DTOs.Contract;
using global::Application.Services;

using FluentAssertions;

using Moq;

using Xunit;
using global::Domain.Interfaces;
using global::Domain.Entities;

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
        result.Should().NotBeNull();
        result!.Id.Should().Be(contract.Id);
        result.ContractNumber.Should().Be("CNT-001");
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
        result.Should().BeNull();
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
        result.Should().NotBeNull();
        result.ContractNumber.Should().Be("CNT-001");
        result.CustomerName.Should().Be("Customer");
        result.Status.Should().Be("Active");
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

        // Act
        Func<Task> act = async () => await _service.CreateAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
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

        // Act
        Func<Task> act = async () => await _service.CreateAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*inactive payment plan*");
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
        result.Should().BeTrue();
        contract.Status.Should().Be(ContractStatus.Terminated);
        contract.EndDate.Should().Be(endDate);
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
        result.Should().BeTrue();
        contract.Status.Should().Be(ContractStatus.Suspended);
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
        result.Should().BeTrue();
        contract.Status.Should().Be(ContractStatus.Active);
        _contractRepositoryMock.Verify(x => x.UpdateAsync(contract), Times.Once);
    }
}