namespace Application.Validators;

using Application.DTOs.Contract;

using FluentValidation;

public class CreateContractValidator : AbstractValidator<CreateContractDto>
{
    public CreateContractValidator()
    {
        RuleFor(x => x.ContractNumber)
            .NotEmpty().WithMessage("Contract number is required")
            .MaximumLength(50).WithMessage("Contract number cannot exceed 50 characters")
            .Matches(@"^[A-Z0-9-]+$").WithMessage("Contract number must contain only uppercase letters, numbers, and hyphens");

        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Customer name is required")
            .MaximumLength(200).WithMessage("Customer name cannot exceed 200 characters");

        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("Customer email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Start date cannot be in the past");

        RuleFor(x => x.InitialPaymentPlanId)
            .NotEmpty().WithMessage("Initial payment plan is required");
    }
}