namespace IntegrationTests.Controllers;

using System.Net;
using System.Net.Http.Json;

using Application.DTOs.Contract;

using API.Models;

using FluentAssertions;

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
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<ContractDto>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_WithExistingId_ReturnsContract()
    {
        // Arrange - First get all contracts to get a valid ID
        var allResponse = await _client.GetAsync("/api/contracts");
        var allResult = await allResponse.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<ContractDto>>>();
        var existingId = allResult!.Data!.First().Id;

        // Act
        var response = await _client.GetAsync($"/api/contracts/{existingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ContractDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(existingId);
    }

    [Fact]
    public async Task GetById_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/contracts/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WithValidData_CreatesContract()
    {
        // Arrange - Get a valid payment plan ID
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
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ContractDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.ContractNumber.Should().Be("INT-TEST-001");
        result.Data.CustomerName.Should().Be("Integration Test Customer");
    }

    [Fact]
    public async Task Create_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange - Invalid email
        var plansResponse = await _client.GetAsync("/api/paymentplans");
        var plansResult = await plansResponse.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<Application.DTOs.PaymentPlan.PaymentPlanDto>>>();
        var planId = plansResult!.Data!.First().Id;

        var createDto = new CreateContractDto
        {
            ContractNumber = "INT-TEST-002",
            CustomerName = "Test",
            CustomerEmail = "invalid-email", // Invalid email format
            StartDate = DateTime.Today,
            InitialPaymentPlanId = planId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/contracts", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Suspend_ExistingContract_SuspendsSuccessfully()
    {
        // Arrange - Get existing contract
        var allResponse = await _client.GetAsync("/api/contracts");
        var allResult = await allResponse.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<ContractDto>>>();
        var contractId = allResult!.Data!.First().Id;

        // Act
        var response = await _client.PostAsync($"/api/contracts/{contractId}/suspend", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().BeTrue();

        // Verify the contract is suspended
        var getResponse = await _client.GetAsync($"/api/contracts/{contractId}");
        var getResult = await getResponse.Content.ReadFromJsonAsync<ApiResponse<ContractDto>>();
        getResult!.Data!.Status.Should().Be("Suspended");
    }

    [Fact]
    public async Task Reactivate_SuspendedContract_ReactivatesSuccessfully()
    {
        // Arrange - Get existing contract and suspend it
        var allResponse = await _client.GetAsync("/api/contracts");
        var allResult = await allResponse.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<ContractDto>>>();
        var contractId = allResult!.Data!.Last().Id; // Use different contract

        await _client.PostAsync($"/api/contracts/{contractId}/suspend", null);

        // Act
        var response = await _client.PostAsync($"/api/contracts/{contractId}/reactivate", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify the contract is active
        var getResponse = await _client.GetAsync($"/api/contracts/{contractId}");
        var getResult = await getResponse.Content.ReadFromJsonAsync<ApiResponse<ContractDto>>();
        getResult!.Data!.Status.Should().Be("Active");
    }
}