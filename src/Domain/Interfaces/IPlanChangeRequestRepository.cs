namespace Domain.Interfaces;

using Domain.Entities;

public interface IPlanChangeRequestRepository
{
    Task<PlanChangeRequest?> GetByIdAsync(Guid id);
    Task<IEnumerable<PlanChangeRequest>> GetByContractIdAsync(Guid contractId);
    Task<IEnumerable<PlanChangeRequest>> GetPendingRequestsAsync();
    Task<PlanChangeRequest> AddAsync(PlanChangeRequest request);
    Task UpdateAsync(PlanChangeRequest request);
}