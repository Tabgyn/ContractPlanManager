namespace UnitTests.Application.Validators;

using global::Application.DTOs.PaymentPlan;
using global::Application.Validators;

using FluentValidation.TestHelper;

using Xunit;

public class CreatePaymentPlanValidatorTests
{
    private readonly CreatePaymentPlanValidator _validator;

    public CreatePaymentPlanValidatorTests()
    {
        _validator = new CreatePaymentPlanValidator();
    }

    [Fact]
    public void Validate_WithValidData_PassesValidation()
    {
        // Arrange
        var dto = new CreatePaymentPlanDto
        {
            Name = "Premium Plan",
            Description = "Premium features",
            MonthlyPrice = 149.99m,
            BillingCycle = "Monthly",
            Tier = "Premium"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithInvalidName_FailsValidation(string invalidName)
    {
        // Arrange
        var dto = new CreatePaymentPlanDto
        {
            Name = invalidName,
            Description = "Description",
            MonthlyPrice = 99.99m,
            BillingCycle = "Monthly",
            Tier = "Basic"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_WithInvalidPrice_FailsValidation(decimal invalidPrice)
    {
        // Arrange
        var dto = new CreatePaymentPlanDto
        {
            Name = "Plan",
            Description = "Description",
            MonthlyPrice = invalidPrice,
            BillingCycle = "Monthly",
            Tier = "Basic"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MonthlyPrice)
            .WithErrorMessage("Monthly price must be greater than zero");
    }

    [Fact]
    public void Validate_WithExcessivePrice_FailsValidation()
    {
        // Arrange
        var dto = new CreatePaymentPlanDto
        {
            Name = "Plan",
            Description = "Description",
            MonthlyPrice = 100001m,
            BillingCycle = "Monthly",
            Tier = "Basic"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MonthlyPrice)
            .WithErrorMessage("Monthly price seems unreasonably high");
    }

    [Theory]
    [InlineData("Weekly")]
    [InlineData("Biweekly")]
    [InlineData("Invalid")]
    [InlineData("")]
    public void Validate_WithInvalidBillingCycle_FailsValidation(string invalidCycle)
    {
        // Arrange
        var dto = new CreatePaymentPlanDto
        {
            Name = "Plan",
            Description = "Description",
            MonthlyPrice = 99.99m,
            BillingCycle = invalidCycle,
            Tier = "Basic"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BillingCycle);
    }

    [Theory]
    [InlineData("Monthly")]
    [InlineData("Quarterly")]
    [InlineData("Annually")]
    public void Validate_WithValidBillingCycle_PassesValidation(string validCycle)
    {
        // Arrange
        var dto = new CreatePaymentPlanDto
        {
            Name = "Plan",
            Description = "Description",
            MonthlyPrice = 99.99m,
            BillingCycle = validCycle,
            Tier = "Basic"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BillingCycle);
    }

    [Theory]
    [InlineData("Starter")]
    [InlineData("Pro")]
    [InlineData("Invalid")]
    [InlineData("")]
    public void Validate_WithInvalidTier_FailsValidation(string invalidTier)
    {
        // Arrange
        var dto = new CreatePaymentPlanDto
        {
            Name = "Plan",
            Description = "Description",
            MonthlyPrice = 99.99m,
            BillingCycle = "Monthly",
            Tier = invalidTier
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tier);
    }

    [Theory]
    [InlineData("Basic")]
    [InlineData("Standard")]
    [InlineData("Premium")]
    [InlineData("Enterprise")]
    public void Validate_WithValidTier_PassesValidation(string validTier)
    {
        // Arrange
        var dto = new CreatePaymentPlanDto
        {
            Name = "Plan",
            Description = "Description",
            MonthlyPrice = 99.99m,
            BillingCycle = "Monthly",
            Tier = validTier
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Tier);
    }
}