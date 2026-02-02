namespace Domain.Entities;

public class PlanChangeRequest
{
    public Guid Id { get; private set; }
    public Guid ContractId { get; private set; }
    public Contract Contract { get; private set; }
    public Guid FromPlanId { get; private set; }
    public PaymentPlan FromPlan { get; private set; }
    public Guid ToPlanId { get; private set; }
    public PaymentPlan ToPlan { get; private set; }
    public ChangeRequestStatus Status { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string RequestedBy { get; private set; }
    public string? ProcessedBy { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTime EffectiveDate { get; private set; }

    private PlanChangeRequest()
    {
        Contract = null!;
        FromPlan = null!;
        ToPlan = null!;
        RequestedBy = string.Empty;
    }

    public PlanChangeRequest(
        Contract contract,
        PaymentPlan fromPlan,
        PaymentPlan toPlan,
        string requestedBy,
        DateTime effectiveDate)
    {
        if (contract == null)
            throw new ArgumentException("Contract is required", nameof(contract));

        if (fromPlan == null)
            throw new ArgumentException("From plan is required", nameof(fromPlan));

        if (toPlan == null)
            throw new ArgumentException("To plan is required", nameof(toPlan));

        if (string.IsNullOrWhiteSpace(requestedBy))
            throw new ArgumentException("Requested by is required", nameof(requestedBy));

        if (effectiveDate < DateTime.UtcNow.Date)
            throw new ArgumentException("Effective date cannot be in the past", nameof(effectiveDate));

        Id = Guid.NewGuid();
        ContractId = contract.Id;
        Contract = contract;
        FromPlanId = fromPlan.Id;
        FromPlan = fromPlan;
        ToPlanId = toPlan.Id;
        ToPlan = toPlan;
        Status = ChangeRequestStatus.Pending;
        RequestedAt = DateTime.UtcNow;
        RequestedBy = requestedBy;
        EffectiveDate = effectiveDate;
    }

    public void Approve(string processedBy)
    {
        if (Status != ChangeRequestStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be approved");

        if (string.IsNullOrWhiteSpace(processedBy))
            throw new ArgumentException("Processed by is required", nameof(processedBy));

        Status = ChangeRequestStatus.Approved;
        ProcessedAt = DateTime.UtcNow;
        ProcessedBy = processedBy;

        Contract.ChangePlan(ToPlan);
    }

    public void Reject(string processedBy, string reason)
    {
        if (Status != ChangeRequestStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be rejected");

        if (string.IsNullOrWhiteSpace(processedBy))
            throw new ArgumentException("Processed by is required", nameof(processedBy));

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Rejection reason is required", nameof(reason));

        Status = ChangeRequestStatus.Rejected;
        ProcessedAt = DateTime.UtcNow;
        ProcessedBy = processedBy;
        RejectionReason = reason;
    }

    public void Cancel()
    {
        if (Status != ChangeRequestStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be cancelled");

        Status = ChangeRequestStatus.Cancelled;
        ProcessedAt = DateTime.UtcNow;
    }
}