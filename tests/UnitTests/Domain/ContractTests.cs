namespace UnitTests.Domain;

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
        Assert.NotNull(contract);
        Assert.NotEqual(Guid.Empty, contract.Id);
        Assert.Equal(contractNumber, contract.ContractNumber);
        Assert.Equal(customerName, contract.CustomerName);
        Assert.Equal(customerEmail, contract.CustomerEmail);
        Assert.Equal(startDate, contract.StartDate);
        Assert.Equal(ContractStatus.Active, contract.Status);
        Assert.Equal(plan, contract.CurrentPaymentPlan);
        Assert.True(Math.Abs((contract.CreatedAt - DateTime.UtcNow).TotalSeconds) < 2);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidContractNumber_ThrowsArgumentException(string invalidNumber)
    {
        // Arrange
        var plan = CreateTestPlan();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Contract(invalidNumber, "Name", "email@test.com", DateTime.UtcNow, plan));

        Assert.Contains("Contract number is required", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidCustomerName_ThrowsArgumentException(string invalidName)
    {
        // Arrange
        var plan = CreateTestPlan();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Contract("CNT-001", invalidName, "email@test.com", DateTime.UtcNow, plan));

        Assert.Contains("Customer name is required", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidCustomerEmail_ThrowsArgumentException(string invalidEmail)
    {
        // Arrange
        var plan = CreateTestPlan();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Contract("CNT-001", "Name", invalidEmail, DateTime.UtcNow, plan));

        Assert.Contains("Customer email is required", exception.Message);
    }

    [Fact]
    public void Constructor_WithNullPlan_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Contract("CNT-001", "Name", "email@test.com", DateTime.UtcNow, null!));

        Assert.Contains("Initial payment plan is required", exception.Message);
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
        Thread.Sleep(10);
        contract.ChangePlan(newPlan);

        // Assert
        Assert.Equal(newPlan, contract.CurrentPaymentPlan);
        Assert.Equal(newPlan.Id, contract.CurrentPaymentPlanId);
        Assert.True(contract.UpdatedAt > originalUpdateTime);
    }

    [Fact]
    public void ChangePlan_WithNullPlan_ThrowsArgumentException()
    {
        // Arrange
        var plan = CreateTestPlan();
        var contract = new Contract("CNT-001", "Name", "email@test.com", DateTime.UtcNow, plan);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => contract.ChangePlan(null!));
        Assert.Contains("New payment plan cannot be null", exception.Message);
    }

    [Fact]
    public void ChangePlan_WhenContractInactive_ThrowsInvalidOperationException()
    {
        // Arrange
        var plan = CreateTestPlan();
        var contract = new Contract("CNT-001", "Name", "email@test.com", DateTime.UtcNow, plan);
        contract.Terminate(DateTime.UtcNow.AddDays(30));
        var newPlan = CreateTestPlan(PlanTier.Premium);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => contract.ChangePlan(newPlan));
        Assert.Contains("Cannot change plan for inactive contract", exception.Message);
    }

    [Fact]
    public void ChangePlan_WithSamePlan_ThrowsInvalidOperationException()
    {
        // Arrange
        var plan = CreateTestPlan();
        var contract = new Contract("CNT-001", "Name", "email@test.com", DateTime.UtcNow, plan);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => contract.ChangePlan(plan));
        Assert.Contains("New plan must be different from current plan", exception.Message);
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
        Assert.Equal(endDate, contract.EndDate);
        Assert.Equal(ContractStatus.Terminated, contract.Status);
    }

    [Fact]
    public void Terminate_WithEndDateBeforeStartDate_ThrowsArgumentException()
    {
        // Arrange
        var plan = CreateTestPlan();
        var startDate = DateTime.UtcNow;
        var contract = new Contract("CNT-001", "Name", "email@test.com", startDate, plan);
        var endDate = startDate.AddDays(-1);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => contract.Terminate(endDate));
        Assert.Contains("End date cannot be before start date", exception.Message);
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
        Assert.Equal(ContractStatus.Suspended, contract.Status);
    }

    [Fact]
    public void Suspend_TerminatedContract_ThrowsInvalidOperationException()
    {
        // Arrange
        var plan = CreateTestPlan();
        var contract = new Contract("CNT-001", "Name", "email@test.com", DateTime.UtcNow, plan);
        contract.Terminate(DateTime.UtcNow.AddDays(30));

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => contract.Suspend());
        Assert.Contains("Cannot suspend terminated contract", exception.Message);
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
        Assert.Equal(ContractStatus.Active, contract.Status);
    }

    [Fact]
    public void Reactivate_TerminatedContract_ThrowsInvalidOperationException()
    {
        // Arrange
        var plan = CreateTestPlan();
        var contract = new Contract("CNT-001", "Name", "email@test.com", DateTime.UtcNow, plan);
        contract.Terminate(DateTime.UtcNow.AddDays(30));

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => contract.Reactivate());
        Assert.Contains("Cannot reactivate terminated contract", exception.Message);
    }
}