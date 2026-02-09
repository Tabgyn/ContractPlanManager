using Infrastructure;
using Infrastructure.Data;

using FluentValidation;
using FluentValidation.AspNetCore;

using Application.Validators;

using Microsoft.EntityFrameworkCore;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateContractValidator>();

// Add Infrastructure services (DbContext, Repositories, Services)
builder.Services.AddInfrastructure(builder.Configuration);

// Add OpenAPI with Scalar
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "Contract Plan Manager API",
            Version = "v1.0.0",
            Description = """
                RESTful API for managing contract payment plan modifications.
                
                **Features:**
                - Contract management (CRUD operations)
                - Payment plan management
                - Plan change request workflow
                
                **Architecture:**
                - Clean Architecture with DDD
                - .NET 10.0
                - Entity Framework Core
                - SQL Server (primary) + PostgreSQL (reporting)
                """,
            Contact = new()
            {
                Name = "Tiago Azevedo Borges",
                Url = new Uri("https://github.com/Tabgyn/ContractPlanManager")
            }
        };
        return Task.CompletedTask;
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/docs", options =>
    {
        options
            .WithTitle("Contract Plan Manager API")
            .WithTheme(ScalarTheme.Default)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Seed database on startup (development only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Check if using a relational database (not in-memory for tests)
        var isRelational = context.Database.IsRelational();

        if (isRelational)
        {
            // Apply pending migrations for relational databases
            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully");
        }
        else
        {
            // For in-memory databases (testing), just ensure created
            logger.LogInformation("Using in-memory database, ensuring created...");
            await context.Database.EnsureCreatedAsync();
            logger.LogInformation("In-memory database created successfully");
        }

        // Seed data
        logger.LogInformation("Seeding database...");
        await DbSeeder.SeedAsync(context);
        logger.LogInformation("Database seeded successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating or seeding the database");
        throw;
    }
}

app.Run();