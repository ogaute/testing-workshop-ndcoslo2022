using FluentAssertions;
using ForeignExchange.Api.Logger;
using ForeignExchange.Api.Models;
using ForeignExchange.Api.Repositories;
using ForeignExchange.Api.Services;
using ForeignExchange.Api.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Xunit;

namespace ForeignExchange.Api.Tests.Unit;

public class QuoteServiceTests
{
    private readonly QuoteService _sut;
    private readonly IRatesRepository _ratesRepository = Substitute.For<IRatesRepository>();
    private readonly ILoggerAdapter<QuoteService> _logger = Substitute.For<ILoggerAdapter<QuoteService>>();

    public QuoteServiceTests()
    {
        _sut = new QuoteService(_ratesRepository, _logger);
    }

    [Theory]
    [InlineData("GBP", "USD", 1.6, 160)]
    [InlineData("USD", "GBP", 1.7, 170)]
    public async Task GetQuoteAsync_ReturnsCorrectRate_WhenCurrenciesAreSupported(
        string baseCurrency, string quoteCurrency, decimal rate, decimal finalResult)
    {
        // Arrange
        var amount = 100;

        var fxRate = new FxRate
        {
            FromCurrency = baseCurrency,
            ToCurrency = quoteCurrency,
            Rate = rate,
            TimestampUtc = DateTime.UtcNow
        };

        _ratesRepository.GetRateAsync(baseCurrency, quoteCurrency)
            .Returns(fxRate);
        
        var expected = new ConversionQuote
        {
            BaseCurrency = baseCurrency,
            BaseAmount = amount,
            QuoteCurrency = quoteCurrency,
            QuoteAmount = finalResult
        };
        
        // Act
        var result = await _sut.GetQuoteAsync(baseCurrency, quoteCurrency, amount);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetQuoteAsync_ShouldThrowSameCurrencyException_WhenSameCurrencyIsUsed()
    {
        // Arrange
        var fromCurrency = "GBP";
        var toCurrency = "GBP";
        var amount = 100;
        
        var fxRate = new FxRate
        {
            FromCurrency = fromCurrency,
            ToCurrency = toCurrency,
            Rate = 1.6m,
            TimestampUtc = DateTime.UtcNow
        };
        
        _ratesRepository.GetRateAsync(fromCurrency, toCurrency)
            .Returns(fxRate);
        
        // Act
        var result = () => _sut.GetQuoteAsync(fromCurrency, toCurrency, amount);
        
        // Assert
        await result.Should().ThrowAsync<SameCurrencyException>()
            .WithMessage("You cannot convert currency GBP to itself");
    }

    [Fact]
    public async Task GetQuoteAsync_LogsInformationMessage_WhenQuoteIsRetrieved()
    {
        // Arrange
        var fromCurrency = "GBP";
        var toCurrency = "USD";
        var amount = 100;

        var fxRate = new FxRate
        {
            FromCurrency = fromCurrency,
            ToCurrency = toCurrency,
            Rate = 1.6m,
            TimestampUtc = DateTime.UtcNow
        };

        _ratesRepository.GetRateAsync(fromCurrency, toCurrency)
            .Returns(fxRate);
        
        // Act
        await _sut.GetQuoteAsync(fromCurrency, toCurrency, amount);
        
        // Assert
		_logger.Received(1).LogInformation(
            "Retrieved quote for currencies {FromCurrency}->{ToCurrency} in {ElapsedMilliseconds}ms",
            Arg.Is<object[]>(objects => 
                objects[0].ToString() == fromCurrency && 
                objects[1].ToString() == toCurrency));
    }
}
