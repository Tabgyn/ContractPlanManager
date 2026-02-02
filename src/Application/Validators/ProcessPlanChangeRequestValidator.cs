namespace Application.Validators;

using Application.DTOs.PlanChangeRequest;

using FluentValidation;

public class ProcessPlanChangeRequestValidator : AbstractValidator<ProcessPlanChangeRequestDto>
{
    public ProcessPlanChangeRequestValidator()
    {
        RuleFor(x => x.ProcessedBy)
            .NotEmpty().WithMessage("Processor information is required")
            .MaximumLength(200).WithMessage("Processor name cannot exceed 200 characters");

        RuleFor(x => x.RejectionReason)
            .NotEmpty().When(x => !x.Approved)
            .WithMessage("Rejection reason is required when rejecting a request")
            .MaximumLength(1000).When(x => !x.Approved)
            .WithMessage("Rejection reason cannot exceed 1000 characters");
    }
}