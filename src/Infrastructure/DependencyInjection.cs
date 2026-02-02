namespace Infrastructure;

using Application.Interfaces;
using Application.Services;

using Domain.Interfaces;

using Infrastructure.Data;
using Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add DbContext for SQL Server
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("Infrastructure")));

        // Register Repositories
        services.AddScoped<IContractRepository, ContractRepository>();
        services.AddScoped<IPaymentPlanRepository, PaymentPlanRepository>();
        services.AddScoped<IPlanChangeRequestRepository, PlanChangeRequestRepository>();

        // Register Services
        services.AddScoped<IContractService, ContractService>();
        services.AddScoped<IPaymentPlanService, PaymentPlanService>();
        services.AddScoped<IPlanChangeRequestService, PlanChangeRequestService>();

        return services;
    }
}