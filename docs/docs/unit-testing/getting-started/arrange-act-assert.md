---
description: The holy trinity of unit test structure
---

# Arrange, Act, Assert

So here is where we left off

```csharp title="IntCalculatorTests.cs"
[Fact]
public void Add_ShouldAddTwoNumbers_WhenBothOfThemArePositiveIntegers()
{
    var sut = new IntCalculator();

    var result = sut.Add(1, 2);
    
    result.Should().Be(3);
}
```

This is a good structure for a test and as you can probably see is loosely has 3 distinct levels.

1. The "setup" level were we create some data that we will use in our test
2. The "action" level were we are using the method we are testing to get a result
3. The "assertion" level were we ensure that the result data matches what we expect

Every test, with only a few exceptions, will have all three level. 

These levels have come to be known as Arrange, Act, Assert and it is a common practice to add a comment with each respective word in the test to show where one starts and where it ends.

With that in mind our test now looks like this:

```csharp title="IntCalculatorTests.cs"
[Fact]
public void Add_ShouldAddTwoNumbers_WhenBothOfThemArePositiveIntegers()
{
    // Arrange
    var sut = new IntCalculator();

    // Act
    var result = sut.Add(1, 2);
    
    // Assert
    result.Should().Be(3);
}
```

Every test we will write from now on will follow this structure.
