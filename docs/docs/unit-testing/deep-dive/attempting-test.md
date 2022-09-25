---
description: Let's try to write a test
---

# Trying to write a test

We can use everything we learned in the Getting Started section to write our first "happy path" test for the `GetQuoteAsync` method.

The test name would be `GetQuoteAsync_ReturnsQuote_WhenCurrenciesAreValid` and would look like this:

```csharp title="QuoteServiceTests.cs"

public class QuoteServiceTests
{
    private readonly QuoteService _sut;
    private readonly IRatesRepository _ratesRepository = new RatesRepository(new NpgsqlConnectionFactory("Server=localhost;Port=5432;Database=mydb;User ID=workshop;Password=changeme;"));
    private readonly ILogger<QuoteService> _logger = new NullLogger<QuoteService>();

    public QuoteServiceTests()
    {
        _sut = new(_ratesRepository, _logger);
    }
    
    [Fact]
    public async Task GetQuoteAsync_ReturnsQuote_WhenCurrenciesAreValid()
    {
        // Arrange
        var fromCurrency = "GBP";
        var toCurrency = "USD";
        var amount = 100;
    
        var expectedQuote = new ConversionQuote
        {
            BaseCurrency = fromCurrency,
            QuoteCurrency = toCurrency,
            BaseAmount = amount,
            QuoteAmount = 160
        };
        
        // Act
        var quote = await _sut.GetQuoteAsync(fromCurrency, toCurrency, amount);
    
        // Assert
        quote!.Should().BeEquivalentTo(expectedQuote);
    }    
}
```

However, this test is pretty...weird. We don't really arrange what data will be returned.
On top of that, even if you assert against a value that matches the rate that the database contains, the rate can change at any time, making the test fail.

If I try to run the test in my workstation this is the response I get:

```csharp
Npgsql.NpgsqlException: Failed to connect to 127.0.0.1:5432

Npgsql.NpgsqlException
Failed to connect to 127.0.0.1:5432
   at Npgsql.Internal.NpgsqlConnector.ConnectAsync(NpgsqlTimeout timeout, CancellationToken cancellationToken)
```

This is because the database isn't running on my workstation, and there is no way for us to guarantee that everyone who will ever run these unit test will have a database running.

On top of that, testing the database integration is out of scope. We don't really care about it at all. All we want to do is assume that the database responds fine and then test our own code.

We can solve that problem by introducing **Mocking**.
