namespace Application.Validators;

using Application.DTOs.PaymentPlan;

using FluentValidation;

public class CreatePaymentPlanValidator : AbstractValidator<CreatePaymentPlanDto>
{
    public CreatePaymentPlanValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Plan name is required")
            .MaximumLength(100).WithMessage("Plan name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.MonthlyPrice)
            .GreaterThan(0).WithMessage("Monthly price must be greater than zero")
            .LessThanOrEqualTo(100000).WithMessage("Monthly price seems unreasonably high");

        RuleFor(x => x.BillingCycle)
            .NotEmpty().WithMessage("Billing cycle is required")
            .Must(BeValidBillingCycle).WithMessage("Invalid billing cycle. Valid values: Monthly, Quarterly, Annually");

        RuleFor(x => x.Tier)
            .NotEmpty().WithMessage("Plan tier is required")
            .Must(BeValidTier).WithMessage("Invalid tier. Valid values: Basic, Standard, Premium, Enterprise");
    }

    private bool BeValidBillingCycle(string billingCycle)
    {
        return billingCycle is "Monthly" or "Quarterly" or "Annually";
    }

    private bool BeValidTier(string tier)
    {
        return tier is "Basic" or "Standard" or "Premium" or "Enterprise";
    }
}