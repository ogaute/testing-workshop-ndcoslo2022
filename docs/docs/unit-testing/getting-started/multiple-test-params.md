---
description: Making our tests more flexible
---

# One test, multiple test cases

## The problem with `[Fact]`

As you might have noticed until now, we've been using the `[Fact]` attribute to create tests that are detected by xUnit.

There is however a pretty interesting problem. In several of the tests, the only thing that changes are the parameters that the test is invoked with. 
The code of the test itself stays the same. This has the drawback of a lot of duplicated code. xUnit has a solution for that.

Instead of creating a test with the `[Fact]` attribute we can use the `[Theory]` attribute. 
The test will still be discovered by xUnit but now we have the option to provide multiple parameters at once.

## InlineData

One of our options is to use the `[InlineData]` attribute to specify just a few test cases in a single test.
`[InlineData]` accepts as many parameters as the method of our test.

For example if we change this test's signature from this:

```csharp
public void Subtract_ShouldSubtractTwoNumbers_WhenTheNumbersAreIntegers()
```

To this:

```csharp
public void Subtract_ShouldSubtractTwoNumbers_WhenTheNumbersAreIntegers(int a, int b, int final)
```

Then we can add an `[InlineData]` attribute with 3 parameters like this:

```csharp
[InlineData(5, 3, 2)]
public void Subtract_ShouldSubtractTwoNumbers_WhenTheNumbersAreIntegers(int a, int b, int final)
```

In this example 5 is mapped to `a`, 3 is mapped to `b` and 2 is mapped to `final`. 
Then we simply use those parameters in our test and our test is now converted to N tests where N is the amount of `[InlineData]` attributes I apply to the method.

For example, the following is 3 individual tests, all using the same code:

```csharp
[Theory]
[InlineData(5, 3, 2)]
[InlineData(5, 5, 0)]
[InlineData(3, 5, -2)]
public void Subtract_ShouldSubtractTwoNumbers_WhenTheNumbersAreIntegers(int a, int b, int final)
{
    // Arrange
    var calculator = new IntCalculator();

    // Act
    var result = calculator.Subtract(a, b);

    // Assert
    result.Should().Be(final);
}
```

## MemberData

`[InlineData]` is a great way to parameterize your tests but what if you want your test cases to be extracted somewhere else so they don't clutter your main test?
This is where the `[MemberData]` attribute comes in.

First we need to create a new method that returns `IEnumerable<object?[]>`. The implementation of this method is completely up to us. 
The most common usecase is to just move our test cases in an in memory array so our example from above will look like this:

```csharp
public static IEnumerable<object[]> SubtractData => new List<object[]>
{
    new object[] { 5, 3, 2 },
    new object[] { 5, 5, 0 },
    new object[] { 3, 5, -2 }
};
```

However, if you wanted to have a file where you have your test cases in and write some code that loads them on test execution, you totally can. I don't recommend it in this case, but it is an option.

In order for our test to use the extracted cases all we need to do is use the `[MemberData]` attribute and point ot our property by name like this:

```csharp
[Theory]    
[MemberData(nameof(SubtractData))]
public void Subtract_ShouldSubtractTwoNumbers_WhenTheNumbersAreIntegers(int a, int b, int final)
```

## ClassData

Similar to `[MemberData]`, the `[ClassData]` attribute takes the same concept and it abstract it into a class instead of a method.

To do that we first need to create a new class that implements `IEnumerable<object?[]>`:

```csharp
public class SubtractData : IEnumerable<object[]>
```

Due to the implementation of the `IEnumerable` interface we will need to implement two methods.

- `public IEnumerator<object?[]> GetEnumerator()`
- `IEnumerator IEnumerable.GetEnumerator()`

`IEnumerator IEnumerable.GetEnumerator()` will simply point to the `public IEnumerator<object?[]> GetEnumerator()` method like this:

```csharp
IEnumerator IEnumerable.GetEnumerator()
{
    return GetEnumerator();
}
```

The `public IEnumerator<object?[]> GetEnumerator()` will now need to return the data in the same way that the `[MemberData]` version did. 
In this case I am going to use the `yield` keyword. It will result to the following implementation.

```csharp
public IEnumerator<object?[]> GetEnumerator()
{
    yield return new object[] { 5, 3, 2 };
    yield return new object[] { 5, 5, 0 };
    yield return new object[] { 3, 5, -2 };
}
```

The full implementation of `SubtractData.cs` is the following:

```csharp title="SubtractData.cs"
public class SubtractData : IEnumerable<object?[]>
{
    public IEnumerator<object?[]> GetEnumerator()
    {
        yield return new object[] { 5, 3, 2 };
        yield return new object[] { 5, 5, 0 };
        yield return new object[] { 3, 5, -2 };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
```

We can now apply the `[ClassData]` attribute to the test method and point to the type of the `SubtractData` class:

```csharp
[Theory]
[ClassData(typeof(SubtractData))]
public void Subtract_ShouldSubtractTwoNumbers_WhenTheNumbersAreIntegers(int a, int b, int final)
```

And that's it. Now our data will be loaded from the SubtractData class.
