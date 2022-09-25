---
description: Your turn to write the rest of the tests
---

# Creating the test project

## What is a test project?

A test project is simply a C# class library project that contains some special packages to make testing easier for us.

Those packages usually include:

- The test execution library
- A mocking library
- An assertions library (optional)
- A code coverage collector (optional)

## Creating the project

There are a couple of ways to create a test project. You can either opt-in to use one of the templates that use one of the three most popular test execution libraries or you can create your own project.

For the sake of fully understanding what we are doing here, I will show you how to create a test project from scratch. To be IDE independent we are doing to do this using purely commands.

Open your favourite Terminal and ensure that you are into `1.UnitTesting/1.Introduction`.

Here are the steps we want to follow:

- Create a class library project called `AmazingCalculator.Tests.Unit`
- Move into the newly created directory
- Delete the absolutely useless `Class1.cs` (that Microsoft should stop created by default)
- Install the `Microsoft.NET.Test.Sdk` nuget package
- Install the `xunit` nuget package
- Install the `xunit.runner.visualstudio` nuget package

And that's it. You have the bare minimum required to run your unit tests. Don't forget to include your newly created project in your solution.

Here are the command that can perform all the actions above.

```commandline
dotnet new classlib -o AmazingCalculator.Tests.Unit
cd .\AmazingCalculator.Tests.Unit
rm .\Class1.cs
dotnet add package Microsoft.NET.Test.Sdk
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
```

Note that all of the above will happen automatically if you use the `xunit` template directly.
Do do that simple use the following command.

```commandline
dotnet new xunit -o AmazingCalculator.Tests.Unit
```
