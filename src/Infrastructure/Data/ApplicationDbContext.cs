namespace Infrastructure.Data;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<PaymentPlan> PaymentPlans => Set<PaymentPlan>();
    public DbSet<PlanChangeRequest> PlanChangeRequests => Set<PlanChangeRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Contract entity
        modelBuilder.Entity<Contract>(entity =>
        {
            entity.ToTable("Contracts");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ContractNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(e => e.ContractNumber)
                .IsUnique();

            entity.Property(e => e.CustomerName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.CustomerEmail)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .IsRequired();

            // Relationship with PaymentPlan
            entity.HasOne(e => e.CurrentPaymentPlan)
                .WithMany()
                .HasForeignKey(e => e.CurrentPaymentPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship with PaymentPlanHistory
            entity.HasMany(e => e.PaymentPlanHistory)
                .WithMany()
                .UsingEntity(j => j.ToTable("ContractPaymentPlanHistory"));
        });

        // Configure PaymentPlan entity
        modelBuilder.Entity<PaymentPlan>(entity =>
        {
            entity.ToTable("PaymentPlans");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.MonthlyPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.BillingCycle)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.Tier)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.IsActive)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Tier);
        });

        // Configure PlanChangeRequest entity
        modelBuilder.Entity<PlanChangeRequest>(entity =>
        {
            entity.ToTable("PlanChangeRequests");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.RequestedAt)
                .IsRequired();

            entity.Property(e => e.RequestedBy)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.ProcessedBy)
                .HasMaxLength(200);

            entity.Property(e => e.RejectionReason)
                .HasMaxLength(1000);

            entity.Property(e => e.EffectiveDate)
                .IsRequired();

            // Relationships
            entity.HasOne(e => e.Contract)
                .WithMany()
                .HasForeignKey(e => e.ContractId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.FromPlan)
                .WithMany()
                .HasForeignKey(e => e.FromPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ToPlan)
                .WithMany()
                .HasForeignKey(e => e.ToPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.ContractId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.RequestedAt);
        });
    }
}