namespace Application.Services;

using Application.DTOs.PlanChangeRequest;
using Application.Interfaces;

using Domain.Entities;
using Domain.Interfaces;

public class PlanChangeRequestService : IPlanChangeRequestService
{
    private readonly IPlanChangeRequestRepository _changeRequestRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IPaymentPlanRepository _paymentPlanRepository;

    public PlanChangeRequestService(
        IPlanChangeRequestRepository changeRequestRepository,
        IContractRepository contractRepository,
        IPaymentPlanRepository paymentPlanRepository)
    {
        _changeRequestRepository = changeRequestRepository;
        _contractRepository = contractRepository;
        _paymentPlanRepository = paymentPlanRepository;
    }

    public async Task<PlanChangeRequestDto?> GetByIdAsync(Guid id)
    {
        var request = await _changeRequestRepository.GetByIdAsync(id);
        return request == null ? null : MapToDto(request);
    }

    public async Task<IEnumerable<PlanChangeRequestDto>> GetByContractIdAsync(Guid contractId)
    {
        var requests = await _changeRequestRepository.GetByContractIdAsync(contractId);
        return requests.Select(MapToDto);
    }

    public async Task<IEnumerable<PlanChangeRequestDto>> GetPendingRequestsAsync()
    {
        var requests = await _changeRequestRepository.GetPendingRequestsAsync();
        return requests.Select(MapToDto);
    }

    public async Task<PlanChangeRequestDto> CreateAsync(CreatePlanChangeRequestDto dto)
    {
        var contract = await _contractRepository.GetByIdAsync(dto.ContractId);
        if (contract == null)
            throw new InvalidOperationException($"Contract with ID {dto.ContractId} not found");

        var toPlan = await _paymentPlanRepository.GetByIdAsync(dto.ToPlanId);
        if (toPlan == null)
            throw new InvalidOperationException($"Payment plan with ID {dto.ToPlanId} not found");

        if (!toPlan.IsActive)
            throw new InvalidOperationException("Cannot change to an inactive payment plan");

        var changeRequest = new PlanChangeRequest(
            contract,
            contract.CurrentPaymentPlan,
            toPlan,
            dto.RequestedBy,
            dto.EffectiveDate);

        await _changeRequestRepository.AddAsync(changeRequest);
        return MapToDto(changeRequest);
    }

    public async Task<PlanChangeRequestDto> ProcessAsync(Guid id, ProcessPlanChangeRequestDto dto)
    {
        var request = await _changeRequestRepository.GetByIdAsync(id);
        if (request == null)
            throw new InvalidOperationException($"Plan change request with ID {id} not found");

        if (dto.Approved)
        {
            request.Approve(dto.ProcessedBy);
        }
        else
        {
            request.Reject(dto.ProcessedBy, dto.RejectionReason ?? "No reason provided");
        }

        await _changeRequestRepository.UpdateAsync(request);
        return MapToDto(request);
    }

    public async Task<bool> CancelAsync(Guid id)
    {
        var request = await _changeRequestRepository.GetByIdAsync(id);
        if (request == null)
            return false;

        request.Cancel();
        await _changeRequestRepository.UpdateAsync(request);
        return true;
    }

    private static PlanChangeRequestDto MapToDto(PlanChangeRequest request)
    {
        return new PlanChangeRequestDto
        {
            Id = request.Id,
            ContractId = request.ContractId,
            ContractNumber = request.Contract.ContractNumber,
            FromPlanId = request.FromPlanId,
            FromPlanName = request.FromPlan.Name,
            ToPlanId = request.ToPlanId,
            ToPlanName = request.ToPlan.Name,
            Status = request.Status.ToString(),
            RequestedAt = request.RequestedAt,
            ProcessedAt = request.ProcessedAt,
            RequestedBy = request.RequestedBy,
            ProcessedBy = request.ProcessedBy,
            RejectionReason = request.RejectionReason,
            EffectiveDate = request.EffectiveDate,
            IsUpgrade = request.ToPlan.IsUpgradeFrom(request.FromPlan)
        };
    }
}