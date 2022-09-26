using FluentAssertions;
using NSubstitute;
using Xunit;

namespace EdgeCases.Tests.Unit;

public class GreeterTests
{
    private readonly Greeter _sut;
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

    public GreeterTests()
    {
        _sut = new Greeter(_dateTimeProvider);
    }
    
    [Fact]
    public void GenerateGreetText_ReturnsGoodMorning_WhenItsMorning()
    {
        // Arrange
        _dateTimeProvider.Now.Returns(new DateTimeOffset(2022, 1, 1, 7, 0, 0, TimeSpan.Zero));

        // Act
        var message = _sut.GenerateGreetText();

        // Assert
        message.Should().Be("Good morning");
    }
    
    [Fact]
    public void GenerateGreetText_ReturnsGoodAfternoon_WhenItsAfternoon()
    {
        // Arrange
        _dateTimeProvider.Now.Returns(new DateTimeOffset(2022, 1, 1, 13, 0, 0, TimeSpan.Zero));

        // Act
        var message = _sut.GenerateGreetText();

        // Assert
        message.Should().Be("Good afternoon");
    }
    
    [Fact]
    public void GenerateGreetText_ReturnsGoodEvening_WhenItsEvening()
    {
        // Arrange
        _dateTimeProvider.Now.Returns(new DateTimeOffset(2022, 1, 1, 20, 0, 0, TimeSpan.Zero));

        // Act
        var message = _sut.GenerateGreetText();

        // Assert
        message.Should().Be("Good evening");
    }
}
