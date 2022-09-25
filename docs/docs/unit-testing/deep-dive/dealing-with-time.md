---
description: Why time in tests can be problematic
---

# Dealing with time

## The problem

Sometimes code is time sensitive. Consider the following method:

```csharp title="Greeter.cs"
public string GenerateGreetText()
{
    var dateTimeNow = DateTime.Now;
    return dateTimeNow.Hour switch
    {
        >= 5 and < 12 => "Good morning",
        >= 12 and < 18 => "Good afternoon",
        _ => "Good evening"
    };
}
```

This method uses the current time of your workstation to determine whether it is morning, afternoon or evening and produce a message.

Attempting to write unit tests for this method would be impossible. If you had 1 test per usecase then only one test out of 3 would pass at one time.

## The solution

In order to solve this problem we need to make the date time provider mockable. To achieve that let's create an interface.

```csharp
public interface IDateTimeProvider
{
    DateTime Now { get; }
}
```

The implementation will simply point to the same property invocation:

```csharp
public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
}
```

We can now write a unit test that mocks the `IDateTimeProvider` that we just created and returns a given date per test.

## Exercise

Implement 3 tests:

- `GenerateGreetText_ShouldReturnGoodMorning_WhenItsMorning`
- `GenerateGreetText_ShouldReturnGoodAfternoon_WhenItsAfternoon`
- `GenerateGreetText_ShouldReturnGoodEvening_WhenItsEvening`

Expand any of the code blocks below to see the solution to the exercise but you are highly encouraged to implement the tests on your own.

<details>
<summary>GenerateGreetText_ShouldReturnGoodMorning_WhenItsMorning</summary>

```csharp
[Fact]
public void GenerateGreetText_ShouldReturnGoodMorning_WhenItsMorning()
{
    // Arrange
    _dateTimeProvider.Now.Returns(new DateTime(2022, 1, 1, 9, 0, 0));
    
    // Act
    var message = _greeter.GenerateGreetText();

    // Assert
    message.Should().Be("Good morning");
}
```
</details>

<details>
<summary>GenerateGreetText_ShouldReturnGoodAfternoon_WhenItsAfternoon</summary>

```csharp
[Fact]
public void GenerateGreetText_ShouldReturnGoodAfternoon_WhenItsAfternoon()
{
    // Arrange
    _dateTimeProvider.Now.Returns(new DateTime(2022, 1, 1, 15, 0, 0));
    
    // Act
    var message = _greeter.GenerateGreetText();

    // Assert
    message.Should().Be("Good afternoon");
}
```
</details>

<details>
<summary>GenerateGreetText_ShouldReturnGoodEvening_WhenItsEvening</summary>

```csharp
[Fact]
public void GenerateGreetText_ShouldReturnGoodEvening_WhenItsEvening()
{
    // Arrange
    _dateTimeProvider.Now.Returns(new DateTime(2022, 1, 1, 20, 0, 0));
    
    // Act
    var message = _greeter.GenerateGreetText();

    // Assert
    message.Should().Be("Good evening");
}
```
</details>
