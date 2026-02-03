namespace UnitTests.Domain;

using FluentAssertions;

using global::Domain.Entities;

using Xunit;

public class ContractTests
{
    private PaymentPlan CreateTestPlan(PlanTier tier = PlanTier.Basic)
    {
        return new PaymentPlan(
            $"{tier} Plan",
            "Test plan",
            99.99m,
            BillingCycle.Monthly,
            tier);
    }

    [Fact]
    public void Constructor_WithValidData_CreatesContract()
    {
        // Arrange
        var plan = CreateTestPlan();
        var contractNumber = "CNT-2024-001";
        var customerName = "Test Customer";
        var customerEmail = "test@example.com";
        var startDate = DateTime.UtcNow;

        // Act
        var contract = new Contract(contractNumber, customerName, customerEmail, startDate, plan);

        // Assert
        contract.Should().NotBeNull();
        contract.Id.Should().NotBeEmpty();
        contract.ContractNumber.Should().Be(contractNumber);
        contract.CustomerName.Should().Be(customerName);
        contract.CustomerEmail.Should().Be(customerEmail);
        contract.StartDate.Should().Be(startDate);
        contract.Status.Should().Be(ContractStatus.Active);
        contract.CurrentPaymentPlan.Should().Be(plan);
        contract.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidContractNumber_ThrowsArgumentException(string invalidNumber)
    {
        // Arrange
        var plan = CreateTestPlan();

        // Act
        Action act = () => new Contract(invalidNumber, "Name", "email@test.com", DateTime.UtcNow, plan);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Contract number is required*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidCustomerName_ThrowsArgumentException(string invalidName)
    {
        // Arrange
        var plan = CreateTestPlan();

        // Act
        Action act = () => new Contract("CNT-001", invalidName, "email@test.com", DateTime.UtcNow, plan);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Customer name is required*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidCustomerEmail_ThrowsArgumentException(string invalidEmail)
    {
        // Arrange
        var plan = CreateTestPlan();

        // Act
        Action act = () => new Contract("CNT-001", "Name", invalidEmail, DateTime.UtcNow, plan);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Customer email is required*");
    }

    [Fact]
    public void Constructor_WithNullPlan_ThrowsArgumentException()
    {
        // Act
        Action act = () => new Contract("CNT-001", "Name", "email@test.com", DateTime.UtcNow, null!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Initial payment plan is required*");
    }

    [Fact]
    public void ChangePlan_WithValidPlan_UpdatesCurrentPlan()
    {
        // Arrange
        var initialPlan = CreateTestPlan(PlanTier.Basic);
        var newPlan = CreateTestPlan(PlanTier.Premium);
        var contract = new Contract("CNT-001", "Name", "email@test.com", DateTime.UtcNow, initialPlan);
        var originalUpdateTime = contract.UpdatedAt;

        // Act
        Thread.Sleep(10); // Ensure UpdatedAt changes
        contract.ChangePlan(newPlan);

        // Assert
        contract.CurrentPaymentPlan.Should().Be(newPlan);
        contract.CurrentPaymentPlanId.Should().Be(newPlan.Id);
        contract.UpdatedAt.Should().BeAfter(originalUpdateTime);
    }

    [Fact]
    public void ChangePlan_WithNullPlan_ThrowsArgumentException()
    {
        // Arrange
        var plan = CreateTestPlan();
        var contract = new Contract("CNT-001", "Name", "email@test.com", DateTime.UtcNow, plan);

        // Act
        Action act = () => contract.ChangePlan(null!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*New payment plan cannot be null*");
    }

    [Fact]
    public void ChangePlan_WhenContractInactive_ThrowsInvalidOperationException()
    {
        // Arrange
        var plan = CreateTestPlan();
        var contract = new Contract("CNT-001", "Name", "email@test.com", DateTime.UtcNow, plan);
        contract.Terminate(DateTime.UtcNow.AddDays(30));
        var newPlan = CreateTestPlan(PlanTier.Premium);

        // Act
        Action act = () => contract.ChangePlan(newPlan);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot change plan for inactive contract*");
    }

    [Fact]
    public void ChangePlan_WithSamePlan_ThrowsInvalidOperationException()
    {
        // Arrange
        var plan = CreateTestPlan();
        var contract = new Contract("CNT-001", "Name", "email@test.com", DateTime.UtcNow, plan);

        // Act
        Action act = () => contract.ChangePlan(plan);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*New plan must be different from current plan*");
    }

    [Fact]
    public void Terminate_WithValidEndDate_TerminatesContract()
    {
        // Arrange
        var plan = CreateTestPlan();
        var startDate = DateTime.UtcNow;
        var contract = new Contract("CNT-001", "Name", "email@test.com", startDate, plan);
        var endDate = startDate.AddMonths(6);

        // Act
        contract.Terminate(endDate);

        // Assert
        contract.EndDate.Should().Be(endDate);
        contract.Status.Should().Be(ContractStatus.Terminated);
    }

    [Fact]
    public void Terminate_WithEndDateBeforeStartDate_ThrowsArgumentException()
    {
        // Arrange
        var plan = CreateTestPlan();
        var startDate = DateTime.UtcNow;
        var contract = new Contract("CNT-001", "Name", "email@test.com", startDate, plan);
        var endDate = startDate.AddDays(-1);

        // Act
        Action act = () => contract.Terminate(endDate);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*End date cannot be before start date*");
    }

    [Fact]
    public void Suspend_ActiveContract_SuspendsContract()
    {
        // Arrange
        var plan = CreateTestPlan();
        var contract = new Contract("CNT-001", "Name", "email@test.com", DateTime.UtcNow, plan);

        // Act
        contract.Suspend();

        // Assert
        contract.Status.Should().Be(ContractStatus.Suspended);
    }

    [Fact]
    public void Suspend_TerminatedContract_ThrowsInvalidOperationException()
    {
        // Arrange
        var plan = CreateTestPlan();
        var contract = new Contract("CNT-001", "Name", "email@test.com", DateTime.UtcNow, plan);
        contract.Terminate(DateTime.UtcNow.AddDays(30));

        // Act
        Action act = () => contract.Suspend();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot suspend terminated contract*");
    }

    [Fact]
    public void Reactivate_SuspendedContract_ReactivatesContract()
    {
        // Arrange
        var plan = CreateTestPlan();
        var contract = new Contract("CNT-001", "Name", "email@test.com", DateTime.UtcNow, plan);
        contract.Suspend();

        // Act
        contract.Reactivate();

        // Assert
        contract.Status.Should().Be(ContractStatus.Active);
    }

    [Fact]
    public void Reactivate_TerminatedContract_ThrowsInvalidOperationException()
    {
        // Arrange
        var plan = CreateTestPlan();
        var contract = new Contract("CNT-001", "Name", "email@test.com", DateTime.UtcNow, plan);
        contract.Terminate(DateTime.UtcNow.AddDays(30));

        // Act
        Action act = () => contract.Reactivate();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot reactivate terminated contract*");
    }
}