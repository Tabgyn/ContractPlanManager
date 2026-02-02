namespace Infrastructure.Data;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Check if data already exists
        if (await context.PaymentPlans.AnyAsync())
            return;

        // Create Payment Plans
        var basicPlan = new PaymentPlan(
            "Basic Plan",
            "Essential features for small businesses",
            29.99m,
            BillingCycle.Monthly,
            PlanTier.Basic);

        var standardPlan = new PaymentPlan(
            "Standard Plan",
            "Advanced features for growing businesses",
            79.99m,
            BillingCycle.Monthly,
            PlanTier.Standard);

        var premiumPlan = new PaymentPlan(
            "Premium Plan",
            "Complete feature set for established businesses",
            149.99m,
            BillingCycle.Monthly,
            PlanTier.Premium);

        var enterprisePlan = new PaymentPlan(
            "Enterprise Plan",
            "Customizable solutions for large organizations",
            299.99m,
            BillingCycle.Monthly,
            PlanTier.Enterprise);

        await context.PaymentPlans.AddRangeAsync(
            basicPlan, standardPlan, premiumPlan, enterprisePlan);

        // Create sample contracts
        var contract1 = new Contract(
            "CNT-2024-001",
            "Acme Corporation",
            "contact@acme.com",
            DateTime.UtcNow.AddMonths(-6),
            basicPlan);

        var contract2 = new Contract(
            "CNT-2024-002",
            "Tech Innovations Ltd",
            "info@techinnovations.com",
            DateTime.UtcNow.AddMonths(-3),
            standardPlan);

        var contract3 = new Contract(
            "CNT-2024-003",
            "Global Solutions Inc",
            "hello@globalsolutions.com",
            DateTime.UtcNow.AddMonths(-1),
            premiumPlan);

        await context.Contracts.AddRangeAsync(contract1, contract2, contract3);

        // Create a sample plan change request
        var changeRequest = new PlanChangeRequest(
            contract1,
            basicPlan,
            standardPlan,
            "system@example.com",
            DateTime.UtcNow.AddDays(7));

        await context.PlanChangeRequests.AddAsync(changeRequest);

        await context.SaveChangesAsync();
    }
}