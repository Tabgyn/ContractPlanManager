namespace IntegrationTests.Controllers;

using System.Net;
using System.Net.Http.Json;

using Application.DTOs.Contract;

using API.Models;

using IntegrationTests.Setup;

using Xunit;

public class ContractsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ContractsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSuccessWithContracts()
    {
        // Act
        var response = await _client.GetAsync("/api/contracts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<ContractDto>>>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotEmpty(result.Data);
    }

    [Fact]
    public async Task GetById_WithExistingId_ReturnsContract()
    {
        // Arrange
        var allResponse = await _client.GetAsync("/api/contracts");
        var allResult = await allResponse.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<ContractDto>>>();
        var existingId = allResult!.Data!.First().Id;

        // Act
        var response = await _client.GetAsync($"/api/contracts/{existingId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ContractDto>>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(existingId, result.Data.Id);
    }

    [Fact]
    public async Task GetById_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/contracts/{nonExistingId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithValidData_CreatesContract()
    {
        // Arrange
        var plansResponse = await _client.GetAsync("/api/paymentplans");
        var plansResult = await plansResponse.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<Application.DTOs.PaymentPlan.PaymentPlanDto>>>();
        var planId = plansResult!.Data!.First().Id;

        var createDto = new CreateContractDto
        {
            ContractNumber = "INT-TEST-001",
            CustomerName = "Integration Test Customer",
            CustomerEmail = "integration@test.com",
            StartDate = DateTime.Today,
            InitialPaymentPlanId = planId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/contracts", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ContractDto>>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("INT-TEST-001", result.Data.ContractNumber);
        Assert.Equal("Integration Test Customer", result.Data.CustomerName);
    }

    [Fact]
    public async Task Create_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var plansResponse = await _client.GetAsync("/api/paymentplans");
        var plansResult = await plansResponse.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<Application.DTOs.PaymentPlan.PaymentPlanDto>>>();
        var planId = plansResult!.Data!.First().Id;

        var createDto = new CreateContractDto
        {
            ContractNumber = "INT-TEST-002",
            CustomerName = "Test",
            CustomerEmail = "invalid-email",
            StartDate = DateTime.Today,
            InitialPaymentPlanId = planId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/contracts", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Suspend_ExistingContract_SuspendsSuccessfully()
    {
        // Arrange
        var allResponse = await _client.GetAsync("/api/contracts");
        var allResult = await allResponse.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<ContractDto>>>();
        var contractId = allResult!.Data!.First().Id;

        // Act
        var response = await _client.PostAsync($"/api/contracts/{contractId}/suspend", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.True(result.Data);

        // Verify
        var getResponse = await _client.GetAsync($"/api/contracts/{contractId}");
        var getResult = await getResponse.Content.ReadFromJsonAsync<ApiResponse<ContractDto>>();
        Assert.Equal("Suspended", getResult!.Data!.Status);
    }

    [Fact]
    public async Task Reactivate_SuspendedContract_ReactivatesSuccessfully()
    {
        // Arrange
        var allResponse = await _client.GetAsync("/api/contracts");
        var allResult = await allResponse.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<ContractDto>>>();
        var contractId = allResult!.Data!.Last().Id;

        await _client.PostAsync($"/api/contracts/{contractId}/suspend", null);

        // Act
        var response = await _client.PostAsync($"/api/contracts/{contractId}/reactivate", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify
        var getResponse = await _client.GetAsync($"/api/contracts/{contractId}");
        var getResult = await getResponse.Content.ReadFromJsonAsync<ApiResponse<ContractDto>>();
        Assert.Equal("Active", getResult!.Data!.Status);
    }
}