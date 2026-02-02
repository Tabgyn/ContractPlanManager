namespace Application.Services;

using Application.DTOs.PaymentPlan;
using Application.Interfaces;

using Domain.Entities;
using Domain.Interfaces;

public class PaymentPlanService : IPaymentPlanService
{
    private readonly IPaymentPlanRepository _paymentPlanRepository;

    public PaymentPlanService(IPaymentPlanRepository paymentPlanRepository)
    {
        _paymentPlanRepository = paymentPlanRepository;
    }

    public async Task<PaymentPlanDto?> GetByIdAsync(Guid id)
    {
        var plan = await _paymentPlanRepository.GetByIdAsync(id);
        return plan == null ? null : MapToDto(plan);
    }

    public async Task<IEnumerable<PaymentPlanDto>> GetAllAsync()
    {
        var plans = await _paymentPlanRepository.GetAllAsync();
        return plans.Select(MapToDto);
    }

    public async Task<IEnumerable<PaymentPlanDto>> GetActivePlansAsync()
    {
        var plans = await _paymentPlanRepository.GetActivePerPlansAsync();
        return plans.Select(MapToDto);
    }

    public async Task<PaymentPlanDto> CreateAsync(CreatePaymentPlanDto dto)
    {
        var billingCycle = Enum.Parse<BillingCycle>(dto.BillingCycle);
        var tier = Enum.Parse<PlanTier>(dto.Tier);

        var plan = new PaymentPlan(
            dto.Name,
            dto.Description,
            dto.MonthlyPrice,
            billingCycle,
            tier);

        await _paymentPlanRepository.AddAsync(plan);
        return MapToDto(plan);
    }

    public async Task<PaymentPlanDto> UpdateAsync(Guid id, UpdatePaymentPlanDto dto)
    {
        var plan = await _paymentPlanRepository.GetByIdAsync(id);
        if (plan == null)
            throw new InvalidOperationException($"Payment plan with ID {id} not found");

        plan.UpdatePricing(dto.MonthlyPrice);

        // Update description using reflection (in production, add method to entity)
        var planType = plan.GetType();
        planType.GetProperty(nameof(PaymentPlan.Description))?.SetValue(plan, dto.Description);

        await _paymentPlanRepository.UpdateAsync(plan);
        return MapToDto(plan);
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var plan = await _paymentPlanRepository.GetByIdAsync(id);
        if (plan == null)
            return false;

        plan.Deactivate();
        await _paymentPlanRepository.UpdateAsync(plan);
        return true;
    }

    public async Task<bool> ReactivateAsync(Guid id)
    {
        var plan = await _paymentPlanRepository.GetByIdAsync(id);
        if (plan == null)
            return false;

        plan.Reactivate();
        await _paymentPlanRepository.UpdateAsync(plan);
        return true;
    }

    private static PaymentPlanDto MapToDto(PaymentPlan plan)
    {
        return new PaymentPlanDto
        {
            Id = plan.Id,
            Name = plan.Name,
            Description = plan.Description,
            MonthlyPrice = plan.MonthlyPrice,
            BillingCycle = plan.BillingCycle.ToString(),
            Tier = plan.Tier.ToString(),
            IsActive = plan.IsActive,
            CreatedAt = plan.CreatedAt
        };
    }
}