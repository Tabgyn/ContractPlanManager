namespace UnitTests.Domain;

using FluentAssertions;

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
        plan.Should().NotBeNull();
        plan.Id.Should().NotBeEmpty();
        plan.Name.Should().Be(name);
        plan.Description.Should().Be(description);
        plan.MonthlyPrice.Should().Be(monthlyPrice);
        plan.BillingCycle.Should().Be(billingCycle);
        plan.Tier.Should().Be(tier);
        plan.IsActive.Should().BeTrue();
        plan.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidName_ThrowsArgumentException(string invalidName)
    {
        // Act
        Action act = () => new PaymentPlan(invalidName, "Description", 99.99m, BillingCycle.Monthly, PlanTier.Basic);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Plan name is required*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WithInvalidPrice_ThrowsArgumentException(decimal invalidPrice)
    {
        // Act
        Action act = () => new PaymentPlan("Plan", "Description", invalidPrice, BillingCycle.Monthly, PlanTier.Basic);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Monthly price must be greater than zero*");
    }

    [Fact]
    public void Deactivate_ActivePlan_DeactivatesPlan()
    {
        // Arrange
        var plan = new PaymentPlan("Plan", "Description", 99.99m, BillingCycle.Monthly, PlanTier.Basic);

        // Act
        plan.Deactivate();

        // Assert
        plan.IsActive.Should().BeFalse();
        plan.DeactivatedAt.Should().NotBeNull();
        plan.DeactivatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
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
        plan.IsActive.Should().BeTrue();
        plan.DeactivatedAt.Should().BeNull();
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
        plan.MonthlyPrice.Should().Be(newPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UpdatePricing_WithInvalidPrice_ThrowsArgumentException(decimal invalidPrice)
    {
        // Arrange
        var plan = new PaymentPlan("Plan", "Description", 99.99m, BillingCycle.Monthly, PlanTier.Basic);

        // Act
        Action act = () => plan.UpdatePricing(invalidPrice);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Monthly price must be greater than zero*");
    }

    [Theory]
    [InlineData(PlanTier.Basic, PlanTier.Standard, false)]
    [InlineData(PlanTier.Standard, PlanTier.Premium, false)]
    [InlineData(PlanTier.Premium, PlanTier.Enterprise, false)]
    [InlineData(PlanTier.Standard, PlanTier.Basic, true)]
    [InlineData(PlanTier.Premium, PlanTier.Standard, true)]
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
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(PlanTier.Standard, PlanTier.Basic, false)]
    [InlineData(PlanTier.Premium, PlanTier.Standard, false)]
    [InlineData(PlanTier.Enterprise, PlanTier.Premium, false)]
    [InlineData(PlanTier.Basic, PlanTier.Standard, true)]
    [InlineData(PlanTier.Standard, PlanTier.Premium, true)]
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
        result.Should().Be(expectedResult);
    }
}