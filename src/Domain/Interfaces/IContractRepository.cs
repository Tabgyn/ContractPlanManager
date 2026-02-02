namespace Domain.Interfaces;

using Domain.Entities;

public interface IContractRepository
{
    Task<Contract?> GetByIdAsync(Guid id);
    Task<Contract?> GetByContractNumberAsync(string contractNumber);
    Task<IEnumerable<Contract>> GetAllAsync();
    Task<IEnumerable<Contract>> GetActiveContractsAsync();
    Task<Contract> AddAsync(Contract contract);
    Task UpdateAsync(Contract contract);
    Task DeleteAsync(Guid id);
}