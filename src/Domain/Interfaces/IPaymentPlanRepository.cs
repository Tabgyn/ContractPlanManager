namespace Domain.Interfaces;

using Domain.Entities;

public interface IPaymentPlanRepository
{
    Task<PaymentPlan?> GetByIdAsync(Guid id);
    Task<IEnumerable<PaymentPlan>> GetAllAsync();
    Task<IEnumerable<PaymentPlan>> GetActivePerPlansAsync();
    Task<PaymentPlan> AddAsync(PaymentPlan plan);
    Task UpdateAsync(PaymentPlan plan);
}