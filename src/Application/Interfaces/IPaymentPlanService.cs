namespace Application.Interfaces;

using Application.DTOs.PaymentPlan;

public interface IPaymentPlanService
{
    Task<PaymentPlanDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<PaymentPlanDto>> GetAllAsync();
    Task<IEnumerable<PaymentPlanDto>> GetActivePlansAsync();
    Task<PaymentPlanDto> CreateAsync(CreatePaymentPlanDto dto);
    Task<PaymentPlanDto> UpdateAsync(Guid id, UpdatePaymentPlanDto dto);
    Task<bool> DeactivateAsync(Guid id);
    Task<bool> ReactivateAsync(Guid id);
}