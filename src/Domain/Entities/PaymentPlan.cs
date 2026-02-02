namespace Domain.Entities;

public class PaymentPlan
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal MonthlyPrice { get; private set; }
    public BillingCycle BillingCycle { get; private set; }
    public PlanTier Tier { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? DeactivatedAt { get; private set; }

    private PaymentPlan()
    {
        Name = string.Empty;
        Description = string.Empty;
    }

    public PaymentPlan(
        string name,
        string description,
        decimal monthlyPrice,
        BillingCycle billingCycle,
        PlanTier tier)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Plan name is required", nameof(name));

        if (monthlyPrice <= 0)
            throw new ArgumentException("Monthly price must be greater than zero", nameof(monthlyPrice));

        Id = Guid.NewGuid();
        Name = name;
        Description = description ?? string.Empty;
        MonthlyPrice = monthlyPrice;
        BillingCycle = billingCycle;
        Tier = tier;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        DeactivatedAt = DateTime.UtcNow;
    }

    public void Reactivate()
    {
        IsActive = true;
        DeactivatedAt = null;
    }

    public void UpdatePricing(decimal newMonthlyPrice)
    {
        if (newMonthlyPrice <= 0)
            throw new ArgumentException("Monthly price must be greater than zero", nameof(newMonthlyPrice));

        MonthlyPrice = newMonthlyPrice;
    }

    public bool IsUpgradeFrom(PaymentPlan otherPlan)
    {
        return Tier > otherPlan.Tier;
    }

    public bool IsDowngradeFrom(PaymentPlan otherPlan)
    {
        return Tier < otherPlan.Tier;
    }
}