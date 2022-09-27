using System.Net;
using System.Net.Http.Json;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Customers.Api.Tests.Integration;

public class CustomerControllerTests : IAsyncLifetime
{
    private readonly List<Guid> _idsToDelete = new();
    private readonly WebApplicationFactory<IApiMarker> _waf = new();

    private readonly HttpClient _client;
    // {
    //     BaseAddress = new Uri("https://localhost:5001")
    // };

    public CustomerControllerTests()
    {
        _client = _waf.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost:5001")
        });
    }

    [Fact]
    public async Task Create_ShouldCreateCustomer_WhenDetailsAreValid()
    {
        // Arrange
        var request = new CustomerRequest
        {
            Email = "nick@chapsas.com",
            FullName = "Nick Chapsas",
            DateOfBirth = new DateTime(1993, 01, 01),
            GitHubUsername = "nickchapsas"
        };

        // Act
        var response = await _client.PostAsJsonAsync("customers", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdCustomer = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        createdCustomer.Should().BeEquivalentTo(request);
        response.Headers.Location.Should().Be($"https://localhost:5001/customers/{createdCustomer.Id}");
        
        // Cleanup
        _idsToDelete.Add(createdCustomer.Id);
    }

    [Fact]
    public async Task Get_ReturnsCustomer_WhenCustomerExists()
    {
        // Arrange
        var request = new CustomerRequest
        {
            Email = "nick@chapsas.com",
            FullName = "Nick Chapsas",
            DateOfBirth = new DateTime(1993, 01, 01),
            GitHubUsername = "nickchapsas"
        };
        
        var createdResponse = await _client.PostAsJsonAsync("customers", request);
        var createdCustomer = await createdResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        
        // Act
        var response = await _client.GetAsync($"customers/{createdCustomer!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var retrievedCustomer = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        retrievedCustomer.Should().BeEquivalentTo(createdCustomer);
        
        // Cleanup
        _idsToDelete.Add(createdCustomer.Id);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        foreach (var id in _idsToDelete)
        {
            await _client.DeleteAsync($"customers/{id}");
        }
    }
}
