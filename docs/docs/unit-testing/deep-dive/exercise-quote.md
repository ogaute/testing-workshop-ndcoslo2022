---
description: It's your turn to write the rest of the tests for the Quote Service
---

# Exercise: The quote service

Now it is your turn to use what you've learned and write tests to cover the remaining use cases.

The tests needed are: 

- `GetQuoteAsync_ThrowsException_WhenAmountIsZeroOrNegative`
- `GetQuoteAsync_ReturnsNull_WhenNoRateExists`
- `GetQuoteAsync_ReturnsQuote_WhenCurrenciesAreValid` - Rewrite this test to use [Theory] and multiple parameters
