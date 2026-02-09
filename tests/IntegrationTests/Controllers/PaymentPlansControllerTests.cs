namespace IntegrationTests.Controllers;

using System.Net;
using System.Net.Http.Json;

using Application.DTOs.PaymentPlan;

using API.Models;

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
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<PaymentPlanDto>>>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.Count() >= 2); // We seeded 2 plans
    }

    [Fact]
    public async Task GetActive_ReturnsOnlyActivePlans()
    {
        // Act
        var response = await _client.GetAsync("/api/paymentplans/active");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<PaymentPlanDto>>>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.All(p => p.IsActive));
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
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<PaymentPlanDto>>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("Integration Test Plan", result.Data.Name);
        Assert.Equal(199.99m, result.Data.MonthlyPrice);
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
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<PaymentPlanDto>>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(39.99m, result.Data.MonthlyPrice);
        Assert.Equal("Updated description", result.Data.Description);
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
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify the plan is deactivated
        var getResponse = await _client.GetAsync($"/api/paymentplans/{planId}");
        var getResult = await getResponse.Content.ReadFromJsonAsync<ApiResponse<PaymentPlanDto>>();
        Assert.NotNull(getResult);
        Assert.True(getResult.Success);
        Assert.False(getResult.Data.IsActive);
    }
}