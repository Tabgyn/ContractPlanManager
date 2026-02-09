namespace UnitTests.Application.Validators;

using FluentValidation.TestHelper;

using global::Application.DTOs.Contract;
using global::Application.Validators;

using Xunit;

public class CreateContractValidatorTests
{
    private readonly CreateContractValidator _validator;

    public CreateContractValidatorTests()
    {
        _validator = new CreateContractValidator();
    }

    [Fact]
    public void Validate_WithValidData_PassesValidation()
    {
        // Arrange
        var dto = new CreateContractDto
        {
            ContractNumber = "CNT-2024-001",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            StartDate = DateTime.Today,
            InitialPaymentPlanId = Guid.NewGuid()
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
    public void Validate_WithInvalidContractNumber_FailsValidation(string invalidNumber)
    {
        // Arrange
        var dto = new CreateContractDto
        {
            ContractNumber = invalidNumber,
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            StartDate = DateTime.Today,
            InitialPaymentPlanId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ContractNumber);
    }

    [Fact]
    public void Validate_WithTooLongContractNumber_FailsValidation()
    {
        // Arrange
        var dto = new CreateContractDto
        {
            ContractNumber = new string('A', 51), // 51 characters
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            StartDate = DateTime.Today,
            InitialPaymentPlanId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ContractNumber)
            .WithErrorMessage("Contract number cannot exceed 50 characters");
    }

    [Theory]
    [InlineData("cnt-2024-001")] // lowercase
    [InlineData("CNT 2024 001")] // spaces
    [InlineData("CNT_2024_001")] // underscores
    public void Validate_WithInvalidContractNumberFormat_FailsValidation(string invalidFormat)
    {
        // Arrange
        var dto = new CreateContractDto
        {
            ContractNumber = invalidFormat,
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            StartDate = DateTime.Today,
            InitialPaymentPlanId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ContractNumber)
            .WithErrorMessage("Contract number must contain only uppercase letters, numbers, and hyphens");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithInvalidCustomerName_FailsValidation(string invalidName)
    {
        // Arrange
        var dto = new CreateContractDto
        {
            ContractNumber = "CNT-2024-001",
            CustomerName = invalidName,
            CustomerEmail = "test@example.com",
            StartDate = DateTime.Today,
            InitialPaymentPlanId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CustomerName);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test")]
    public void Validate_WithInvalidEmail_FailsValidation(string invalidEmail)
    {
        // Arrange
        var dto = new CreateContractDto
        {
            ContractNumber = "CNT-2024-001",
            CustomerName = "Test Customer",
            CustomerEmail = invalidEmail,
            StartDate = DateTime.Today,
            InitialPaymentPlanId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CustomerEmail);
    }

    [Fact]
    public void Validate_WithPastStartDate_FailsValidation()
    {
        // Arrange
        var dto = new CreateContractDto
        {
            ContractNumber = "CNT-2024-001",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            StartDate = DateTime.Today.AddDays(-1),
            InitialPaymentPlanId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartDate)
            .WithErrorMessage("Start date cannot be in the past");
    }

    [Fact]
    public void Validate_WithEmptyPaymentPlanId_FailsValidation()
    {
        // Arrange
        var dto = new CreateContractDto
        {
            ContractNumber = "CNT-2024-001",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            StartDate = DateTime.Today,
            InitialPaymentPlanId = Guid.Empty
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.InitialPaymentPlanId);
    }
}