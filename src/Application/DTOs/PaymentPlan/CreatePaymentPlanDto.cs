namespace Application.DTOs.PaymentPlan;

public class CreatePaymentPlanDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal MonthlyPrice { get; set; }
    public string BillingCycle { get; set; } = string.Empty;
    public string Tier { get; set; } = string.Empty;
}