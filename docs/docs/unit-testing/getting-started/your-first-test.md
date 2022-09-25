---
description: Time for us to write our first unit tests
---

# Your first test(s)

## The simplest usecase

It's time for you to write your first test 🥳 

The test case we want to test for is that when two positive integers are added then the expected result is returned.

## The naive but perfectly valid approach

First we need to create our test method. 
In that method we will write code that matches the above case.

```csharp title="Tests.cs"
[Fact]
public void Test()
{
    var result = new IntCalculator().Add(1, 2);
    if(result != 3)
    {
        throw new Exception("Result wasn't 3");
    }
}
```

This is a perfectly valid test. 

We are creating an instance of `IntCalculator`, we are invoking the `Add` method with appropriate parameters and then we check if the value is what we expected and if it isn't then we throw an exception.

There are however tools and practices that make the code above more readable and maintainable.

Let's start from top to bottom.
