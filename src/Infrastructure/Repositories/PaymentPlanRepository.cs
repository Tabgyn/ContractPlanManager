namespace Infrastructure.Repositories;

using Domain.Entities;
using Domain.Interfaces;

using Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

public class PaymentPlanRepository : IPaymentPlanRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentPlanRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentPlan?> GetByIdAsync(Guid id)
    {
        return await _context.PaymentPlans.FindAsync(id);
    }

    public async Task<IEnumerable<PaymentPlan>> GetAllAsync()
    {
        return await _context.PaymentPlans
            .OrderBy(p => p.Tier)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<PaymentPlan>> GetActivePerPlansAsync()
    {
        return await _context.PaymentPlans
            .Where(p => p.IsActive)
            .OrderBy(p => p.Tier)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<PaymentPlan> AddAsync(PaymentPlan plan)
    {
        await _context.PaymentPlans.AddAsync(plan);
        await _context.SaveChangesAsync();
        return plan;
    }

    public async Task UpdateAsync(PaymentPlan plan)
    {
        _context.PaymentPlans.Update(plan);
        await _context.SaveChangesAsync();
    }
}