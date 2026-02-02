namespace Infrastructure.Repositories;

using Domain.Entities;
using Domain.Interfaces;

using Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

public class PlanChangeRequestRepository : IPlanChangeRequestRepository
{
    private readonly ApplicationDbContext _context;

    public PlanChangeRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PlanChangeRequest?> GetByIdAsync(Guid id)
    {
        return await _context.PlanChangeRequests
            .Include(r => r.Contract)
            .Include(r => r.FromPlan)
            .Include(r => r.ToPlan)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<PlanChangeRequest>> GetByContractIdAsync(Guid contractId)
    {
        return await _context.PlanChangeRequests
            .Include(r => r.Contract)
            .Include(r => r.FromPlan)
            .Include(r => r.ToPlan)
            .Where(r => r.ContractId == contractId)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PlanChangeRequest>> GetPendingRequestsAsync()
    {
        return await _context.PlanChangeRequests
            .Include(r => r.Contract)
            .Include(r => r.FromPlan)
            .Include(r => r.ToPlan)
            .Where(r => r.Status == ChangeRequestStatus.Pending)
            .OrderBy(r => r.RequestedAt)
            .ToListAsync();
    }

    public async Task<PlanChangeRequest> AddAsync(PlanChangeRequest request)
    {
        await _context.PlanChangeRequests.AddAsync(request);
        await _context.SaveChangesAsync();
        return request;
    }

    public async Task UpdateAsync(PlanChangeRequest request)
    {
        _context.PlanChangeRequests.Update(request);
        await _context.SaveChangesAsync();
    }
}