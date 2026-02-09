namespace UnitTests.Domain;

using global::Domain.Entities;

using Xunit;

public class PaymentPlanTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesPaymentPlan()
    {
        // Arrange
        var name = "Premium Plan";
        var description = "Premium features";
        var monthlyPrice = 149.99m;
        var billingCycle = BillingCycle.Monthly;
        var tier = PlanTier.Premium;

        // Act
        var plan = new PaymentPlan(name, description, monthlyPrice, billingCycle, tier);

        // Assert
        Assert.NotNull(plan);
        Assert.NotEqual(Guid.Empty, plan.Id);
        Assert.Equal(name, plan.Name);
        Assert.Equal(description, plan.Description);
        Assert.Equal(monthlyPrice, plan.MonthlyPrice);
        Assert.Equal(billingCycle, plan.BillingCycle);
        Assert.Equal(tier, plan.Tier);
        Assert.True(plan.IsActive);
        Assert.True(Math.Abs((plan.CreatedAt - DateTime.UtcNow).TotalSeconds) < 2);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidName_ThrowsArgumentException(string invalidName)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new PaymentPlan(invalidName, "Description", 99.99m, BillingCycle.Monthly, PlanTier.Basic));

        Assert.Contains("Plan name is required", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WithInvalidPrice_ThrowsArgumentException(decimal invalidPrice)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new PaymentPlan("Plan", "Description", invalidPrice, BillingCycle.Monthly, PlanTier.Basic));

        Assert.Contains("Monthly price must be greater than zero", exception.Message);
    }

    [Fact]
    public void Deactivate_ActivePlan_DeactivatesPlan()
    {
        // Arrange
        var plan = new PaymentPlan("Plan", "Description", 99.99m, BillingCycle.Monthly, PlanTier.Basic);

        // Act
        plan.Deactivate();

        // Assert
        Assert.False(plan.IsActive);
        Assert.NotNull(plan.DeactivatedAt);
        Assert.True(Math.Abs((plan.DeactivatedAt.Value - DateTime.UtcNow).TotalSeconds) < 2);
    }

    [Fact]
    public void Reactivate_DeactivatedPlan_ReactivatesPlan()
    {
        // Arrange
        var plan = new PaymentPlan("Plan", "Description", 99.99m, BillingCycle.Monthly, PlanTier.Basic);
        plan.Deactivate();

        // Act
        plan.Reactivate();

        // Assert
        Assert.True(plan.IsActive);
        Assert.Null(plan.DeactivatedAt);
    }

    [Fact]
    public void UpdatePricing_WithValidPrice_UpdatesPrice()
    {
        // Arrange
        var plan = new PaymentPlan("Plan", "Description", 99.99m, BillingCycle.Monthly, PlanTier.Basic);
        var newPrice = 149.99m;

        // Act
        plan.UpdatePricing(newPrice);

        // Assert
        Assert.Equal(newPrice, plan.MonthlyPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UpdatePricing_WithInvalidPrice_ThrowsArgumentException(decimal invalidPrice)
    {
        // Arrange
        var plan = new PaymentPlan("Plan", "Description", 99.99m, BillingCycle.Monthly, PlanTier.Basic);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => plan.UpdatePricing(invalidPrice));
        Assert.Contains("Monthly price must be greater than zero", exception.Message);
    }

    [Theory]
    [InlineData(PlanTier.Standard, PlanTier.Basic, true)]
    [InlineData(PlanTier.Premium, PlanTier.Standard, true)]
    [InlineData(PlanTier.Enterprise, PlanTier.Premium, true)]
    [InlineData(PlanTier.Basic, PlanTier.Standard, false)]
    [InlineData(PlanTier.Standard, PlanTier.Premium, false)]
    [InlineData(PlanTier.Basic, PlanTier.Basic, false)]
    public void IsUpgradeFrom_ComparingTiers_ReturnsCorrectResult(
        PlanTier currentTier, PlanTier otherTier, bool expectedResult)
    {
        // Arrange
        var currentPlan = new PaymentPlan("Current", "Description", 99.99m, BillingCycle.Monthly, currentTier);
        var otherPlan = new PaymentPlan("Other", "Description", 99.99m, BillingCycle.Monthly, otherTier);

        // Act
        var result = currentPlan.IsUpgradeFrom(otherPlan);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(PlanTier.Basic, PlanTier.Standard, true)]
    [InlineData(PlanTier.Standard, PlanTier.Premium, true)]
    [InlineData(PlanTier.Premium, PlanTier.Enterprise, true)]
    [InlineData(PlanTier.Standard, PlanTier.Basic, false)]
    [InlineData(PlanTier.Premium, PlanTier.Standard, false)]
    [InlineData(PlanTier.Basic, PlanTier.Basic, false)]
    public void IsDowngradeFrom_ComparingTiers_ReturnsCorrectResult(
        PlanTier currentTier, PlanTier otherTier, bool expectedResult)
    {
        // Arrange
        var currentPlan = new PaymentPlan("Current", "Description", 99.99m, BillingCycle.Monthly, currentTier);
        var otherPlan = new PaymentPlan("Other", "Description", 99.99m, BillingCycle.Monthly, otherTier);

        // Act
        var result = currentPlan.IsDowngradeFrom(otherPlan);

        // Assert
        Assert.Equal(expectedResult, result);
    }
}