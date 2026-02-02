namespace Application.Validators;

using Application.DTOs.PlanChangeRequest;

using FluentValidation;

public class CreatePlanChangeRequestValidator : AbstractValidator<CreatePlanChangeRequestDto>
{
    public CreatePlanChangeRequestValidator()
    {
        RuleFor(x => x.ContractId)
            .NotEmpty().WithMessage("Contract ID is required");

        RuleFor(x => x.ToPlanId)
            .NotEmpty().WithMessage("Target plan ID is required");

        RuleFor(x => x.RequestedBy)
            .NotEmpty().WithMessage("Requester information is required")
            .MaximumLength(200).WithMessage("Requester name cannot exceed 200 characters");

        RuleFor(x => x.EffectiveDate)
            .NotEmpty().WithMessage("Effective date is required")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Effective date cannot be in the past");
    }
}