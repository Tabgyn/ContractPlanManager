namespace Domain.Entities;

public class Contract
{
    public Guid Id { get; private set; }
    public string ContractNumber { get; private set; }
    public string CustomerName { get; private set; }
    public string CustomerEmail { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public ContractStatus Status { get; private set; }
    public Guid CurrentPaymentPlanId { get; private set; }
    public PaymentPlan CurrentPaymentPlan { get; private set; }
    public ICollection<PaymentPlan> PaymentPlanHistory { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Contract()
    {
        ContractNumber = string.Empty;
        CustomerName = string.Empty;
        CustomerEmail = string.Empty;
        CurrentPaymentPlan = null!;
        PaymentPlanHistory = new List<PaymentPlan>();
    }

    public Contract(
        string contractNumber,
        string customerName,
        string customerEmail,
        DateTime startDate,
        PaymentPlan initialPlan) : this()
    {
        if (string.IsNullOrWhiteSpace(contractNumber))
            throw new ArgumentException("Contract number is required", nameof(contractNumber));

        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Customer name is required", nameof(customerName));

        if (string.IsNullOrWhiteSpace(customerEmail))
            throw new ArgumentException("Customer email is required", nameof(customerEmail));

        if (initialPlan == null)
            throw new ArgumentException("Initial payment plan is required", nameof(initialPlan));

        Id = Guid.NewGuid();
        ContractNumber = contractNumber;
        CustomerName = customerName;
        CustomerEmail = customerEmail;
        StartDate = startDate;
        Status = ContractStatus.Active;
        CurrentPaymentPlan = initialPlan;
        CurrentPaymentPlanId = initialPlan.Id;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePlan(PaymentPlan newPlan)
    {
        if (newPlan == null)
            throw new ArgumentException("New payment plan cannot be null", nameof(newPlan));

        if (Status != ContractStatus.Active)
            throw new InvalidOperationException("Cannot change plan for inactive contract");

        if (CurrentPaymentPlanId == newPlan.Id)
            throw new InvalidOperationException("New plan must be different from current plan");

        CurrentPaymentPlan = newPlan;
        CurrentPaymentPlanId = newPlan.Id;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Terminate(DateTime endDate)
    {
        if (endDate < StartDate)
            throw new ArgumentException("End date cannot be before start date");

        EndDate = endDate;
        Status = ContractStatus.Terminated;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Suspend()
    {
        if (Status == ContractStatus.Terminated)
            throw new InvalidOperationException("Cannot suspend terminated contract");

        Status = ContractStatus.Suspended;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reactivate()
    {
        if (Status == ContractStatus.Terminated)
            throw new InvalidOperationException("Cannot reactivate terminated contract");

        Status = ContractStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }
}