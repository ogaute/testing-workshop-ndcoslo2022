---
description: What is mocking and why we need it
---

# Introducing Mocking

The idea of mocking is pretty simple. Since we are already injecting interfaces (or abstract classes) into our classes, 
why not use something that can create in-memory proxy implementations of those interfaces, instead of calling the real implementation?

The main thing we are trying to do is to replace the real `IRatesRepository` implementation that calls a database with something that doesn't.
We could just write our own, but then we need to also write code to handle every scenario in our tests.
Instead of doing that we can add a mocking library in our test project and let it create in-memory proxies of the classes for use.

The mocking library we will be using is [NSubstitute](https://github.com/nsubstitute/NSubstitute) and can be installed either via your Nuget client or by running the command:

```commandline
dotnet add package NSubstitute
```

Now we can use the `Substitute.For` method to create a substitute or a mock implementation for our `IRatesRepository` class:

```csharp title="QuoteServiceTests.cs"
private readonly IRatesRepository _ratesRepository = Substitute.For<IRatesRepository>();
```

Once we do that we can use a set of extension methods that NSubstitute provides to make the `IRatesRepository` methods of our choice responds in a way that we expect them to.

In our case, instead of going to the database and getting the `FxRate` for a currency, I would simply like the `GetRateAsync` method to return a rate of `1.6` when the `baseCurrency` is `GBP` and the `quoteCurrency` is `USD`.
In terms of code, the object I am expecting back looks like this:

```csharp
var fromCurrency = "GBP";
var toCurrency = "USD";

var expectedRate = new FxRate
{
    FromCurrency = fromCurrency,
    ToCurrency = toCurrency,
    TimestampUtc = DateTime.UtcNow,
    Rate = 1.6m
};
```

In order to make the `GetRateAsync` method of the `IRatesRepository` to return that rate all we need to do is invoke it and chain the `Returns` method with the expected rate.

```csharp
_ratesRepository.GetRateAsync(fromCurrency, toCurrency).Returns(expectedRate);
```

And that's it! Now you can run the test and you will get a green checkmark ✅.

<details>

<summary>The full GetQuoteAsync_ReturnsQuote_WhenCurrenciesAreValid code</summary>

```csharp
[Fact]
public async Task GetQuoteAsync_ReturnsQuote_WhenCurrenciesAreValid()
{
    // Arrange
    var fromCurrency = "GBP";
    var toCurrency = "USD";
    var amount = 100;
    var expectedRate = new FxRate
    {
        FromCurrency = fromCurrency,
        ToCurrency = toCurrency,
        TimestampUtc = DateTime.UtcNow,
        Rate = 1.6m
    };

    var expectedQuote = new ConversionQuote
    {
        BaseCurrency = fromCurrency,
        QuoteCurrency = toCurrency,
        BaseAmount = amount,
        QuoteAmount = 160
    };
    
    _ratesRepository.GetRateAsync(fromCurrency, toCurrency)
        .Returns(expectedRate);
    
    // Act
    var quote = await _sut.GetQuoteAsync(fromCurrency, toCurrency, amount);

    // Assert
    quote!.Should().BeEquivalentTo(expectedQuote);
}
```

</details>
