---
description: Time to run our first mutation tests
---

# Let's run some Mutation Tests

## The calculator class

Consider the following calculator class:

```csharp
public class Calculator
{
    public int Add(int first, int second)
    {
        return first + second;
    }

    public int Subtract(int first, int second)
    {
        return first - second;
    }

    public int Multiply(int first, int second)
    {
        return first * second;
    }

    public (int Result, int Remainder) Divide(int first, int second)
    {
        if (second == 0)
        {
            throw new DivideByZeroException();
        }
        
        var result = first / second;
        var remainder = first % second;
        return (result, remainder);
    }
}
```

It is a bit different than the one we saw in the introduction but it has mostly the same functionality.

Because we are good developers we wrote unit tests for this calculator:

```csharp
public class CalculatorTests
{
    [Theory]
    [InlineData(5, 5, 10)]
    public void Test_Add(int first, int second, int expected)
    {
        // Arrange
        var sut = new Calculator();

        // Act
        var result = sut.Add(first, second);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(5, 5, 0)]
    public void Test_Subtract(int first, int second, int expected)
    {
        // Arrange
        var sut = new Calculator();
        
        // Act
        var result = sut.Subtract(first, second);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, 1, 1)]
    public void Test_Multiply(int first, int second, int expected)
    {
        // Arrange
        var sut = new Calculator();
        
        // Act
        var result = sut.Multiply(first, second);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, 1, 1, 0)]
    public void Test_Divide(int first, int second, int expected, int remainder)
    {
        // Arrange
        var sut = new Calculator();
        
        // Act
        var result = sut.Divide(first, second);

        // Assert
        result.Result.Should().Be(expected);
        result.Remainder.Should().Be(remainder);
    }

    [Fact]
    public void Test_Divide_ByZero()
    {
        // Arrange
        var sut = new Calculator();

        // Act
        var result = () => sut.Divide(1, 0);
        
        // Assert
        result.Should().Throw<DivideByZeroException>();
    }
}
```

Running code coverage collection for these tests reveals that our code is 100% covered. 

![](/img/unit/mutation-coverage.png)

Amazing right? _Well..._

## Running our first mutation test

In order to run mutation tests we first need to install the Stryker.NET command line too.
It can be installed either as a project specific tool or as a global tool. In this example we will install it as a global tool.

```commandline
dotnet tool install -g dotnet-stryker
```

Now all we need to do is to be on the test project director of whichever tests we want to run mutation tests against and run:

```commandline
dotnet stryker
```

Stryker.NET will kick in and do its thing. In the end it will generate a report in the for of a site.

![](/img/unit/stryker-results.png)

Before we look at the report there are a few interesting metrics in the console.

- 12 mutants created: This means that Stryker.NET was able to create 12 different variations of our codebase to run our existing tests against
- Killed 7: This means that 7 of the mutations made our tests fail which is good because they should fail when the code changes
- Survived 5: This means that 5 of the mutations didn't manage to make our tests fail. When our tests pass on mutated code it means that they aren't good
- The final mutation score is 58.33%: Mutation score is the percentage of killed mutants divided by the total number of mutants multiplied by 100. The higher the percentage the better

Let's take a look at the Stryker.NET report

![](/img/unit/stryker-1.png)

![](/img/unit/stryker-2.png)

In the generated report, Stryker let's us know exactly which mutation survived and why it did. 
In the example above, the mutant survived because in our test, we only test subtraction that leads to the result `0`.
This leads to Stryker applied one of its mutations which is to replace `first - second` with `0` and the test still passes, when it shouldn't.

We can fix that by adding an extra test or test case that checks for a different number as a result of the subtraction. 

For example `10-9=1`:

```csharp
[Theory]
[InlineData(5, 5, 0)]
// highlight-next-line
[InlineData(10, 9, 1)]
public void Test_Subtract(int first, int second, int expected)
```

This will fix our problem:

![](/img/unit/stryker-3.png)

Note here that our code coverage is still 100% but we now have a more robust set of tests.
