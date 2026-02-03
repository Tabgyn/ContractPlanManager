namespace IntegrationTests.Setup;

using System.Data.Common;

using Infrastructure.Data;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration

            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(IDbContextOptionsConfiguration<ApplicationDbContext>));

            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbConnection));

            if (dbConnectionDescriptor != null)
                services.Remove(dbConnectionDescriptor);

            // Add DbContext using in-memory database for testing
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("IntegrationTestDb");
            });

            // Build service provider
            var serviceProvider = services.BuildServiceProvider();

            // Create a scope to get the database context
            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ApplicationDbContext>();

            // Ensure the database is created
            db.Database.EnsureCreated();

            // Seed test data
            SeedTestData(db);
        });

        builder.UseEnvironment("Development");
    }

    private static void SeedTestData(ApplicationDbContext context)
    {
        // Only seed if empty
        if (context.PaymentPlans.Any())
            return;

        // Clear existing data (shouldn't be necessary, but safe)
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Add test payment plans
        var basicPlan = new Domain.Entities.PaymentPlan(
            "Basic Plan",
            "Test basic plan",
            29.99m,
            Domain.Entities.BillingCycle.Monthly,
            Domain.Entities.PlanTier.Basic);

        var standardPlan = new Domain.Entities.PaymentPlan(
            "Standard Plan",
            "Test standard plan",
            79.99m,
            Domain.Entities.BillingCycle.Monthly,
            Domain.Entities.PlanTier.Standard);

        context.PaymentPlans.AddRange(basicPlan, standardPlan);
        context.SaveChanges();

        // Add test contract
        var testContract = new Domain.Entities.Contract(
            "TEST-001",
            "Test Customer",
            "test@example.com",
            DateTime.UtcNow,
            basicPlan);

        context.Contracts.Add(testContract);
        context.SaveChanges();
    }
}