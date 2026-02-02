namespace Application.Interfaces;

using Application.DTOs.PlanChangeRequest;

public interface IPlanChangeRequestService
{
    Task<PlanChangeRequestDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<PlanChangeRequestDto>> GetByContractIdAsync(Guid contractId);
    Task<IEnumerable<PlanChangeRequestDto>> GetPendingRequestsAsync();
    Task<PlanChangeRequestDto> CreateAsync(CreatePlanChangeRequestDto dto);
    Task<PlanChangeRequestDto> ProcessAsync(Guid id, ProcessPlanChangeRequestDto dto);
    Task<bool> CancelAsync(Guid id);
}