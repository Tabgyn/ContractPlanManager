namespace IntegrationTests.Controllers;

using System.Net;
using System.Net.Http.Json;

using Application.DTOs.PaymentPlan;

using API.Models;

using FluentAssertions;

using IntegrationTests.Setup;

using Xunit;

public class PaymentPlansControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PaymentPlansControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSuccessWithPlans()
    {
        // Act
        var response = await _client.GetAsync("/api/paymentplans");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<PaymentPlanDto>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        result.Data.Should().HaveCountGreaterThanOrEqualTo(2); // We seeded 2 plans
    }

    [Fact]
    public async Task GetActive_ReturnsOnlyActivePlans()
    {
        // Act
        var response = await _client.GetAsync("/api/paymentplans/active");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<PaymentPlanDto>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        result.Data!.All(p => p.IsActive).Should().BeTrue();
    }

    [Fact]
    public async Task Create_WithValidData_CreatesPlan()
    {
        // Arrange
        var createDto = new CreatePaymentPlanDto
        {
            Name = "Integration Test Plan",
            Description = "Created during integration test",
            MonthlyPrice = 199.99m,
            BillingCycle = "Monthly",
            Tier = "Premium"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/paymentplans", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<PaymentPlanDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Integration Test Plan");
        result.Data.MonthlyPrice.Should().Be(199.99m);
    }

    [Fact]
    public async Task Create_WithInvalidBillingCycle_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreatePaymentPlanDto
        {
            Name = "Invalid Plan",
            Description = "Invalid billing cycle",
            MonthlyPrice = 99.99m,
            BillingCycle = "Weekly", // Invalid
            Tier = "Basic"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/paymentplans", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_ExistingPlan_UpdatesSuccessfully()
    {
        // Arrange - Get existing plan
        var allResponse = await _client.GetAsync("/api/paymentplans");
        var allResult = await allResponse.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<PaymentPlanDto>>>();
        var planId = allResult!.Data!.First().Id;

        var updateDto = new UpdatePaymentPlanDto
        {
            MonthlyPrice = 39.99m,
            Description = "Updated description"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/paymentplans/{planId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<PaymentPlanDto>>();
        result!.Data!.MonthlyPrice.Should().Be(39.99m);
        result.Data.Description.Should().Be("Updated description");
    }

    [Fact]
    public async Task Deactivate_ExistingPlan_DeactivatesSuccessfully()
    {
        // Arrange - Get existing plan
        var allResponse = await _client.GetAsync("/api/paymentplans");
        var allResult = await allResponse.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<PaymentPlanDto>>>();
        var planId = allResult!.Data!.Last().Id;

        // Act
        var response = await _client.PostAsync($"/api/paymentplans/{planId}/deactivate", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify the plan is deactivated
        var getResponse = await _client.GetAsync($"/api/paymentplans/{planId}");
        var getResult = await getResponse.Content.ReadFromJsonAsync<ApiResponse<PaymentPlanDto>>();
        getResult!.Data!.IsActive.Should().BeFalse();
    }
}