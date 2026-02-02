namespace Application.DTOs.PlanChangeRequest;

public class CreatePlanChangeRequestDto
{
    public Guid ContractId { get; set; }
    public Guid ToPlanId { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
}