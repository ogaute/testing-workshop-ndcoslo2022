---
description: The Quote Service we'll be testing
---

# The QuoteService

In this section of the workshop we will be focusing on unit testing one particular class, the `QuoteService` which can be found in the `Services` folder. 
This class contains 4 main aspects that you should know how to unit test because they appear over and over again in all code-bases.

Those are:
- A returned value
- An IO call, in this case a database call
- An exception thrown
- A method invocation that doesn't return anything

All of these aspects lead to code paths that we need to assert against but the means to do so isn't always obvious.

The `GetQuoteAsync` method will be what we will be focusing on:

```csharp
public async Task<ConversionQuote?> GetQuoteAsync(string fromCurrency, string toCurrency, decimal amount)
{
    var sw = Stopwatch.StartNew();
    try
    {
        if (amount <= 0)
        {
            throw new NegativeAmountException();
        }
        
        if (fromCurrency == toCurrency)
        {
            throw new SameCurrencyException(fromCurrency);
        }
    
        var rate = await _ratesRepository.GetRateAsync(fromCurrency, toCurrency);

        if (rate is null)
        {
            return null;
        }

        var quoteAmount = rate.Rate * amount;
    
        return new ConversionQuote
        {
            BaseCurrency = fromCurrency,
            QuoteCurrency = toCurrency,
            BaseAmount = amount,
            QuoteAmount = quoteAmount
        };
    }
    finally
    {
        _logger.LogInformation(
            "Retrieved quote for currencies {FromCurrency}->{ToCurrency} in {ElapsedMilliseconds}ms",
            fromCurrency, toCurrency, sw.ElapsedMilliseconds);
    }
}
```
