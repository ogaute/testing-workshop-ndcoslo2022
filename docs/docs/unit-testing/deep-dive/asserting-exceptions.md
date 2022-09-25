---
description: Writing assertions for exceptions
---

# Asserting exceptions

It is not uncommon for exceptions to be part of our application's logic. 
This means that we should be unit testing that exceptions are thrown when they should be. 

But here is the question. How do we assert that an exception is thrown, when throwing an exception in a test will make it fail?

Let's say that we want to write a test called `GetQuoteAsync_ThrowsException_WhenSameCurrencyIsUsed` which ensures that an exception is thrown when the `baseCurrency` and the `quoteCurrency` are the same value.

```csharp
[Fact]
public async Task GetQuoteAsync_ThrowsException_WhenSameCurrencyIsUsed()
{
    // Arrange
    var fromCurrency = "GBP";
    var toCurrency = "GBP";
    var amount = 100;
    var expectedRate = new FxRate
    {
        FromCurrency = fromCurrency,
        ToCurrency = toCurrency,
        TimestampUtc = DateTime.UtcNow,
        Rate = 1.6m
    };
    
    _ratesRepository.GetRateAsync(fromCurrency, toCurrency)
        .Returns(expectedRate);
    
    // Act
    var quote = _sut.GetQuoteAsync(fromCurrency, toCurrency, amount);
}
```

This is as far as our test will go before it failed with a `SameCurrencyException`.

To solve that problem, we need to wrap our `Act` part in an `Action`. 
Once we do that we can use FluentAssertions' `Should().ThrowAsync<ExceptionType>` extension methods to validate that the right exception was thrown, without failing our test.

This will make our Act look like this:

```csharp
// Act
var quoteAction = () => _sut.GetQuoteAsync(fromCurrency, toCurrency, amount);
```

And our Assert look like this:

```csharp
// Assert
await quoteAction.Should().ThrowAsync<SameCurrencyException>();
```

And now our test will pass 🥳! 

There is however one thing that we haven't tested and we really should. 
That's the exception message. We can use the chained `WithMessage` method to achieved that.

```csharp
// Assert
await quoteAction.Should().ThrowAsync<SameCurrencyException>().WithMessage($"You cannot convert currency {fromCurrency} to itself");
```
