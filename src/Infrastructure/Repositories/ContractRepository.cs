namespace Infrastructure.Repositories;

using Domain.Entities;
using Domain.Interfaces;

using Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

public class ContractRepository : IContractRepository
{
    private readonly ApplicationDbContext _context;

    public ContractRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Contract?> GetByIdAsync(Guid id)
    {
        return await _context.Contracts
            .Include(c => c.CurrentPaymentPlan)
            .Include(c => c.PaymentPlanHistory)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Contract?> GetByContractNumberAsync(string contractNumber)
    {
        return await _context.Contracts
            .Include(c => c.CurrentPaymentPlan)
            .Include(c => c.PaymentPlanHistory)
            .FirstOrDefaultAsync(c => c.ContractNumber == contractNumber);
    }

    public async Task<IEnumerable<Contract>> GetAllAsync()
    {
        return await _context.Contracts
            .Include(c => c.CurrentPaymentPlan)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Contract>> GetActiveContractsAsync()
    {
        return await _context.Contracts
            .Include(c => c.CurrentPaymentPlan)
            .Where(c => c.Status == ContractStatus.Active)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Contract> AddAsync(Contract contract)
    {
        await _context.Contracts.AddAsync(contract);
        await _context.SaveChangesAsync();
        return contract;
    }

    public async Task UpdateAsync(Contract contract)
    {
        _context.Contracts.Update(contract);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var contract = await _context.Contracts.FindAsync(id);
        if (contract != null)
        {
            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
        }
    }
}