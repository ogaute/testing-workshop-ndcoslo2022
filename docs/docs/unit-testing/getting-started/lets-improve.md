---
description: Building on what we already have
---

# Let's improve our test

## Improving naming

### Naming test classes

`Tests.cs` is a really bad name for a class. The class name should immediately tell you (at a high level) which part of the application is being tested.
Instead of using `Tests.cs`, we should use a pattern.

My preferred pattern for naming test classes is `<classname>Tests.cs`. 

This means that in our scenario, the class name becomes `IntCalculatorTests.cs`. Now I immediately know that this test class contains tests for the class `IntCalculator.cs` without having to open it.  

### Naming test methods

Likewise `Test` is a really bad name for a test. 
The name of a test should tell you everything you need to know about what this test is doing. 
At a glance you should know which method is tested and what it is being tested for.

My preferred pattern for naming tests is `<methodname>_Should<do-something>_When<condition-is-met>`.

With that in mind the new name should be `Add_ShouldAddTwoNumbers_WhenBothOfThemArePositiveIntegers`.

This makes our new class look like this:

```csharp title="IntCalculatorTests.cs"
[Fact]
public void Add_ShouldAddTwoNumbers_WhenBothOfThemArePositiveIntegers()
{
    var result = new IntCalculator().Add(1, 2);
    if(result != 3)
    {
        throw new Exception("Result wasn't 3");
    }
}
```

### Mixing things up

The line `var result = new IntCalculator().Add(1, 2);` is problematic. It might not look like it now but consider the following.

The class we instantiate now needs dependencies and the method we are using has more parameters. You can see how this can get out of hand.

To make the code more readable and more manageable we can split the class we are testing by the method it is invoking. We will also call it `sut` which stands for "System under test". 
This gives us a clear indication at which class is supposed to be tested in this test class.

So now we are here:

```csharp title="IntCalculatorTests.cs"
[Fact]
public void Add_ShouldAddTwoNumbers_WhenBothOfThemArePositiveIntegers()
{
    var sut = new IntCalculator();

    var result = sut.Add(1, 2);
    if(result != 3)
    {
        throw new Exception("Result wasn't 3");
    }
}
```

### Assertions

Checking the parameter and throwing an exception is perfectly valid but it doesn't look or read nicely. There is a better way to deal with asserting test results.

By default, the xUnit library has the `Assert` class which contains tons of static methods that we can use to assert test data.

In this case we can use the `Assert.Equal` method to check if the expected result matches the actual result.

So now our class looks like this:

```csharp title="IntCalculatorTests.cs"
[Fact]
public void Add_ShouldAddTwoNumbers_WhenBothOfThemArePositiveIntegers()
{
    var sut = new IntCalculator();

    var result = sut.Add(1, 2);
    
    Assert.Equal(3, result);
}
```

Now we are looking really good but we can be even better.

### Fluent Assertions

Using the `Assert` class is technically fine but it has two drawbacks. 

- When tests fail, the failure message isn't that descriptive, making it harder for us to see what went wrong.
- Having to invoke a static method doesn't read nicely.

We can solve both of these problems by bringing in an excellent Nuget package called [FluentAssertions](https://github.com/fluentassertions/fluentassertions).

You can install that either by the `dotnet add package` command or via your IDE's Nuget client.

Once installed we can use the `.Should()` extensions on any type which allows us to fluently describe what this method should do.

For our case the test becomes:

```csharp title="IntCalculatorTests.cs"
[Fact]
public void Add_ShouldAddTwoNumbers_WhenBothOfThemArePositiveIntegers()
{
    var sut = new IntCalculator();

    var result = sut.Add(1, 2);
    
    result.Should().Be(3);
}
```

Comparing our final method with the first one shows how far we've come. Now at a glance we can see:

- Which class is being tested
- Which method is being tested
- Which result is being asserted against which expected value
