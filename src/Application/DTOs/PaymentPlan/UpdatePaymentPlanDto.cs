namespace Application.DTOs.PaymentPlan;

public class UpdatePaymentPlanDto
{
    public decimal MonthlyPrice { get; set; }
    public string Description { get; set; } = string.Empty;
}