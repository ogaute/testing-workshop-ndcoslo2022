---
description: Let's write unit tests for a simple integer calculator
---

# The calculator

## The reasoning

The calculator as a concept lends itself very nicely as a good starting point for unit testing.
It is simple, isolated, most people are familiar with the concept and have almost certainly used one before.

What we will be doing is testing that the calculator class is behaving the way we expect it to behave.

You can find the class in the `TestingWorkshop` solution under `1.UnitTesting/1.Introduction/AmazingCalculator/IntCalculator.cs`.

```csharp title="IntCalculator.cs"
public class IntCalculator
{
    public int Add(int a, int b) => a + b;

    public int Subtract(int a, int b) => a - b;

    public int Multiply(int a, int b) => a * b;

    public double Divide(int a, int b) => a / (double)b;
}
```

## Test cases

Each calculator method can be invoked with multiple parameters and each parameter combination can have a different outcome.
These scenarios is what we call "Test cases".

Some of the common "happy paths" could be:

- The `Add` method adds two positive integers correctly
- The `Add` method adds two negative integers correctly

There are however some "unhappy paths" that we should also test for:

- The `Multiply` method returns 0 when any number is multiplied by 0
- The `Divide` method throws a DivideByZero exception when division by 0 is attempted

A very common trap people can fall into when writing unit tests is to assume that just because happy paths are covered then their code is fine.
The devil is in the details however so you need to think outside of the box.
