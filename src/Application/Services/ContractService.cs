namespace Application.Services;

using Application.DTOs.Contract;
using Application.Interfaces;

using Domain.Entities;
using Domain.Interfaces;

public class ContractService : IContractService
{
    private readonly IContractRepository _contractRepository;
    private readonly IPaymentPlanRepository _paymentPlanRepository;

    public ContractService(
        IContractRepository contractRepository,
        IPaymentPlanRepository paymentPlanRepository)
    {
        _contractRepository = contractRepository;
        _paymentPlanRepository = paymentPlanRepository;
    }

    public async Task<ContractDto?> GetByIdAsync(Guid id)
    {
        var contract = await _contractRepository.GetByIdAsync(id);
        return contract == null ? null : MapToDto(contract);
    }

    public async Task<ContractDto?> GetByContractNumberAsync(string contractNumber)
    {
        var contract = await _contractRepository.GetByContractNumberAsync(contractNumber);
        return contract == null ? null : MapToDto(contract);
    }

    public async Task<IEnumerable<ContractDto>> GetAllAsync()
    {
        var contracts = await _contractRepository.GetAllAsync();
        return contracts.Select(MapToDto);
    }

    public async Task<IEnumerable<ContractDto>> GetActiveContractsAsync()
    {
        var contracts = await _contractRepository.GetActiveContractsAsync();
        return contracts.Select(MapToDto);
    }

    public async Task<ContractDto> CreateAsync(CreateContractDto dto)
    {
        var paymentPlan = await _paymentPlanRepository.GetByIdAsync(dto.InitialPaymentPlanId);
        if (paymentPlan == null)
            throw new InvalidOperationException($"Payment plan with ID {dto.InitialPaymentPlanId} not found");

        if (!paymentPlan.IsActive)
            throw new InvalidOperationException("Cannot create contract with inactive payment plan");

        var contract = new Contract(
            dto.ContractNumber,
            dto.CustomerName,
            dto.CustomerEmail,
            dto.StartDate,
            paymentPlan);

        await _contractRepository.AddAsync(contract);
        return MapToDto(contract);
    }

    public async Task<ContractDto> UpdateAsync(Guid id, UpdateContractDto dto)
    {
        var contract = await _contractRepository.GetByIdAsync(id);
        if (contract == null)
            throw new InvalidOperationException($"Contract with ID {id} not found");

        // Using reflection to update properties since we don't have setters
        // In production, you'd add Update methods to the entity
        var contractType = contract.GetType();
        contractType.GetProperty(nameof(Contract.CustomerName))?.SetValue(contract, dto.CustomerName);
        contractType.GetProperty(nameof(Contract.CustomerEmail))?.SetValue(contract, dto.CustomerEmail);
        contractType.GetProperty(nameof(Contract.UpdatedAt))?.SetValue(contract, DateTime.UtcNow);

        await _contractRepository.UpdateAsync(contract);
        return MapToDto(contract);
    }

    public async Task<bool> TerminateAsync(Guid id, DateTime endDate)
    {
        var contract = await _contractRepository.GetByIdAsync(id);
        if (contract == null)
            return false;

        contract.Terminate(endDate);
        await _contractRepository.UpdateAsync(contract);
        return true;
    }

    public async Task<bool> SuspendAsync(Guid id)
    {
        var contract = await _contractRepository.GetByIdAsync(id);
        if (contract == null)
            return false;

        contract.Suspend();
        await _contractRepository.UpdateAsync(contract);
        return true;
    }

    public async Task<bool> ReactivateAsync(Guid id)
    {
        var contract = await _contractRepository.GetByIdAsync(id);
        if (contract == null)
            return false;

        contract.Reactivate();
        await _contractRepository.UpdateAsync(contract);
        return true;
    }

    private static ContractDto MapToDto(Contract contract)
    {
        return new ContractDto
        {
            Id = contract.Id,
            ContractNumber = contract.ContractNumber,
            CustomerName = contract.CustomerName,
            CustomerEmail = contract.CustomerEmail,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            Status = contract.Status.ToString(),
            CurrentPaymentPlanId = contract.CurrentPaymentPlanId,
            CurrentPaymentPlanName = contract.CurrentPaymentPlan.Name,
            CurrentMonthlyPrice = contract.CurrentPaymentPlan.MonthlyPrice,
            CreatedAt = contract.CreatedAt,
            UpdatedAt = contract.UpdatedAt
        };
    }
}