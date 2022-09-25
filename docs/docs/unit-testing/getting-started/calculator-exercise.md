---
description: Your turn to write the rest of the tests
---

# Exercise: Test the calculator

Let's get our hands dirty. It is now your turn to apply what we learned until now.

## The test cases

Here are a few test case names that I would like to to write the test for.

- `Add_ShouldReturnZero_WhenAnOppositePositiveAndNegativeNumberAreAdded` | Example: -5 + 5 = 0
- `Subtract_ShouldSubtractTwoNumbers_WhenTheNumbersAreIntegers` | Example: 7 - 5 = 2
- `Multiply_ShouldMultiplyTwoNumbers_WhenTheNumbersArePositiveIntegers` | Example: 6 * 9 = 54
- `Multiply_ShouldReturnZero_WhenOneOfTheNumbersIsZero` | Example: 7 * 0 = 0
- `Divide_ShouldDivideTwoNumbers_WhenNumbersAreDivisible` | Example: 10 / 2 = 5
- `Divide_ShouldReturnTheFirstNumber_WhenNumberIsDividedByOne` | Example: 7 / 1 = 7

:::tip

Feel free to call me over to give you a hand 🤚

:::

## Solutions

You can expand any individual block to see my solution to each exercise. You are still strongly advised to try and write the methods yourself.  

<details>

<summary>Add_ShouldReturnZero_WhenAnOppositePositiveAndNegativeNumberAreAdded</summary>

```csharp
[Fact]
public void Add_ShouldReturnZero_WhenAnOppositePositiveAndNegativeNumberAreAdded()
{
    // Arrange
    var calculator = new IntCalculator();

    // Act
    var result = calculator.Add(5, -5);
    
    // Assert
    result.Should().Be(0);
}
```

</details>

<details>

<summary>Subtract_ShouldSubtractTwoNumbers_WhenTheNumbersAreIntegers</summary>

```csharp
[Fact]
public void Subtract_ShouldSubtractTwoNumbers_WhenTheNumbersAreIntegers()
{
    // Arrange
    var calculator = new IntCalculator();

    // Act
    var result = calculator.Subtract(7, 5);

    // Assert
    result.Should().Be(2);
}
```

</details>

<details>

<summary>Multiply_ShouldMultiplyTwoNumbers_WhenTheNumbersArePositiveIntegers</summary>

```csharp
[Fact]
public void Multiply_ShouldMultiplyTwoNumbers_WhenTheNumbersArePositiveIntegers()
{
    // Arrange
    var calculator = new IntCalculator();

    // Act
    var result = calculator.Multiply(6, 9);
    
    // Assert
    result.Should().Be(54);
}
```

</details>

<details>

<summary>Multiply_ShouldReturnZero_WhenOneOfTheNumbersIsZero</summary>

```csharp
[Fact]
public void Multiply_ShouldReturnZero_WhenOneOfTheNumbersIsZero()
{
    // Arrange
    var calculator = new IntCalculator();

    // Act
    var result = calculator.Multiply(7, 0);
    
    // Assert
    result.Should().Be(0);
}
```

</details>

<details>

<summary>Divide_ShouldDivideTwoNumbers_WhenNumbersAreDivisible</summary>

```csharp
[Fact]
public void Divide_ShouldDivideTwoNumbers_WhenNumbersAreDivisible()
{
    // Arrange
    var calculator = new IntCalculator();

    // Act
    var result = calculator.Divide(10, 2);
    
    // Assert
    result.Should().Be(5);
}
```

</details>

<details>

<summary>Divide_ShouldReturnTheFirstNumber_WhenNumberIsDividedByOne</summary>

```csharp
[Fact]
public void Divide_ShouldReturnTheFirstNumber_WhenNumberIsDividedByOne()
{
    // Arrange
    var calculator = new IntCalculator();

    // Act
    var result = calculator.Divide(7, 1);
    
    // Assert
    result.Should().Be(7);
}
```

</details>
