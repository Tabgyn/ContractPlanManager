namespace Application.DTOs.Contract;

public class CreateContractDto
{
    public string ContractNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public Guid InitialPaymentPlanId { get; set; }
}