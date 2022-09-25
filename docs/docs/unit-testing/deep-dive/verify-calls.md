---
description: Verifying calls on items that don't return a result
---

# Verifying expected calls

## Checking for received calls

Sometimes, you want to verify that something happened, but the method you are testing, won't return anything related to that thing.

In our case, we want to make sure that the `LogInformation` call of the `ILogger` was invoked with the correct message and parameters.
The reason for that is that we might have alerts that get triggered given a specific log message being logged, or a specific `elapsedMilliseconds` threshold being exceeded.

Since there is no return item we can verify against, we need to use the `Received` extension method.

The Arrange and Act part of our code can be the same as in the happy path test.

```csharp
[Fact]
public async Task GetQuoteAsync_LogsAppropriateMessage_WhenInvoked()
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
    
    _ratesRepository.GetRateAsync(fromCurrency, toCurrency)
        .Returns(expectedRate);
    
    // Act
    await _sut.GetQuoteAsync(fromCurrency, toCurrency, amount);
```

Assert, however, will be different. Instead of calling the `LogInformation` method and then making sure it returned something, we will called the `Received` method.
This will allow us to check how many times the method received an invocation and we'll be able to assert the parameters themselves.

If we just wanted to assert that it received a call with text `Retrieved quote for currencies {FromCurrency}->{ToCurrency} in {ElapsedMilliseconds}ms` and any parameters then Assert would look like this:

```csharp
// Assert
_logger.Received(1).LogInformation("Retrieved quote for currencies {FromCurrency}->{ToCurrency} in {ElapsedMilliseconds}ms", Arg.Any<object?[]>());
```

The way this reads is "The logger class, should have received 1 call on the `LogInformation` method, will text `Retrieved quote for currencies {FromCurrency}->{ToCurrency} in {ElapsedMilliseconds}ms` and any parameters".

This *SHOULD* work, however our test fails. This is due to a different problem.

## The problem

`LogInformation` is a static extension method on the ILogger interface.

```csharp title="LoggerExtensions.cs"
public static void LogInformation(this ILogger logger, string? message, params object?[] args)
{
    logger.Log(LogLevel.Information, message, args);
}
```

Since we can only mock instance methods of interfaces, the capturing of the call fails.

In order to make this work, we need to follow the invocations of what the extension method is calling behind the scenes. The deepest level of invocation is this:

```csharp title="LoggerExtensions.cs"
public static void Log(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, string? message, params object?[] args)
{
    if (logger == null)
    {
        throw new ArgumentNullException(nameof(logger));
    }

    logger.Log(logLevel, eventId, new FormattedLogValues(message, args), exception, _messageFormatter);
}
```

Even though I could attempt to mock this call and verify it, we have a very serious problem. `FormattedLogValues` is an `internal` `struct`.

Since we don't have access to that struct we cannot _easily_ assert its values. There is, however, a solution.

## The solution

We will use the Adapter pattern. We will create our own `ILoggerAdapter` and `LoggerAdapter` implementation that injects the `ILogger<T>` into it and effectively proxies the calls.

Here are the class and interface that we need:

```csharp title="ILoggerAdapter.cs"
public interface ILoggerAdapter<T>
{
    void LogInformation(string messageTemplate, params object?[] args);
}
```

```csharp title="LoggerAdapter.cs"
public class LoggerAdapter<T> : ILoggerAdapter<T>
{
    private readonly ILogger<T> _logger;

    public LoggerAdapter(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string messageTemplate, params object?[] args)
    {
        _logger.LogInformation(messageTemplate, args);
    }
}
```

:::tip
Don't worry that we don't implement all the methods of the `ILogger`. We can implement them as we go forward and as we require them. 
:::

With that, we can change the injected services of our `QuoteService` to use the logger adapter:

```csharp title="QuoteService"
private readonly IRatesRepository _ratesRepository;
private readonly ILoggerAdapter<QuoteService> _logger;

public QuoteService(IRatesRepository ratesRepository, ILoggerAdapter<QuoteService> logger)
{
    _ratesRepository = ratesRepository;
    _logger = logger;
}
```

And last but not least, register our open types to dependency injection:

```csharp title="Program.cs"
builder.Services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));
```

Now we can run our test again and it passes ✅!

## Verifying specific arguments

As you might have noticed, we used `Arg.Any<object?[]>()` above to say "I don't case what parameters this method was invoked with".

This _might_ be fine in some scenarios but what if we want to check the specific parameters? This is where the `Arg.Is` method comes in.

Since we are verifying an array, we can use a delegate and check each item individually. This is how it would look like:

```csharp
// Assert
_logger.Received(1)
    .LogInformation("Retrieved quote for currencies {FromCurrency}->{ToCurrency} in {ElapsedMilliseconds}ms",
        Arg.Is<object[]>(x => 
            x[0].ToString() == fromCurrency && 
            x[1].ToString() == toCurrency));
```
