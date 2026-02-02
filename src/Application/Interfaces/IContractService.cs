namespace Application.Interfaces;

using Application.DTOs.Contract;

public interface IContractService
{
    Task<ContractDto?> GetByIdAsync(Guid id);
    Task<ContractDto?> GetByContractNumberAsync(string contractNumber);
    Task<IEnumerable<ContractDto>> GetAllAsync();
    Task<IEnumerable<ContractDto>> GetActiveContractsAsync();
    Task<ContractDto> CreateAsync(CreateContractDto dto);
    Task<ContractDto> UpdateAsync(Guid id, UpdateContractDto dto);
    Task<bool> TerminateAsync(Guid id, DateTime endDate);
    Task<bool> SuspendAsync(Guid id);
    Task<bool> ReactivateAsync(Guid id);
}