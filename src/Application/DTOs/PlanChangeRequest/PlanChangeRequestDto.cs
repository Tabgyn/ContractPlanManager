namespace Application.DTOs.PlanChangeRequest;

public class PlanChangeRequestDto
{
    public Guid Id { get; set; }
    public Guid ContractId { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public Guid FromPlanId { get; set; }
    public string FromPlanName { get; set; } = string.Empty;
    public Guid ToPlanId { get; set; }
    public string ToPlanName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public string? ProcessedBy { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime EffectiveDate { get; set; }
    public bool IsUpgrade { get; set; }
}