using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ForeignExchange.Api.Tests.Integration;

public class FxControllerTests : IClassFixture<ForexApiFactory>
{
    private readonly HttpClient _client;
    
    public FxControllerTests(ForexApiFactory customerApiFactory)
    {
        _client = customerApiFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost:5001")
        });
    }
    
    [Fact]
    public async Task GetQuote_ShouldReturnQuote_WhenCurrencyPairIsSupported()
    {
        // Arrange
        
        // Act
        
        // Assert
		
    }
    
    [Fact]
    public async Task GetQuote_ShouldReturnBadRequest_WhenBaseAndQuoteCurrenciesAreTheSame()
    {
        // Arrange
        
        // Act
        
        // Assert
		
    }
    
    [Fact]
    public async Task GetQuote_ShouldReturnNotFound_WhenCurrencyIsNotSupported()
    {
        // Arrange
        
        // Act
        
        // Assert
		
    }
    
    [Fact]
    public async Task GetQuote_ShouldReturnBadRequest_WhenAmountUsedIsZeroOrLess()
    {
        // Arrange
        
        // Act
        
        // Assert
		
    }
}
