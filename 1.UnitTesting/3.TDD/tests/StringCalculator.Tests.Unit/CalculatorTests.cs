using FluentAssertions;
using Xunit;

namespace StringCalculator.Tests.Unit;

public class CalculatorTests
{
    [Fact]
    public void Add_AddTwoNumbers_WhenSeparatedByCommas()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var sum = calculator.Add("1,2");

        // Assert
        sum.Should().Be(3);
    }
    
    [Fact]
    public void Add_AddOneNumber_WhenSeparatedByCommas()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var sum = calculator.Add("1");

        // Assert
        sum.Should().Be(1);
    }
    
    [Fact]
    public void Add_AddsNoNumbers_WhenStringIsEmpty()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var sum = calculator.Add("");

        // Assert
        sum.Should().Be(0);
    }

    [Fact]
    public void Add_ShouldAddAnUnknownAmountOfNumber()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var sum = calculator.Add("1,2,3");

        // Assert
        sum.Should().Be(6);
    }
    
    [Fact]
    public void Add_ShouldAddAnUnknownAmountOfNumbers_WhenCommaOrNewLineIsPresent()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var sum = calculator.Add("1\n2,3");

        // Assert
        sum.Should().Be(6);
    }
    
    [Fact]
    public void Add_ShouldAddAnUnknownAmountOfNumbers_CustomDelimiterIsUsed()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var sum = calculator.Add("//@\n2@3");

        // Assert
        sum.Should().Be(5);
    }
}
