namespace Application.DTOs.PlanChangeRequest;

public class ProcessPlanChangeRequestDto
{
    public bool Approved { get; set; }
    public string ProcessedBy { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
}