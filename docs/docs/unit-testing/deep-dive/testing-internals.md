---
description: Writing tests for internal items can be challenging
---

# Testing internals

## The problem

It is not uncommon, especially for library authors, to make several of their classes or methods as `internal`.
The reason for doing so is simple. Some classes or methods might not need to be exposed outside of the assembly it lives in.
This makes them be excluded from the contract between the library author and the consumer and it means that the author can change them without worrying about whether the consumer is using them.
Another reason could be to simply provide a better developer experience. Why expose a method that you don't expect the consumer to use?

Using the code from our previous example, if the Greeter class wasn't supposed to be exposed it would look like this:

```csharp
internal class Greeter
{
    private readonly IDateTimeProvider _dateTimeProvider;

    internal Greeter(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    internal string GenerateGreetText()
    {
        var dateTimeNow = _dateTimeProvider.Now;
        return dateTimeNow.Hour switch
        {
            >= 5 and < 12 => "Good morning",
            >= 12 and < 18 => "Good afternoon",
            _ => "Good evening"
        };
    }
}
```

Here is where this becomes problematic. 
In order to unit test an assembly you need to refer to it in the test project and access the classes and methods you want to test.
However, since some of the members you want to test are marked as `internal`, we can't access them.

## The solution(s)

There are multiple solutions to this problem. They all lead to the same experience and ultimately code generated.

### The `[InternalsVisibleTo]` attribute

The main way to make your internals visible to a different project is to drop a C# class file in your assembly and define the `[InternalsVisibleTo]` attribute on the assembly level.

```csharp title="InternalsVisible.cs"
using System.Runtime.CompilerServices;
[assembly:InternalsVisibleTo("EdgeCases.Tests.Unit")]
```

The text parameter of the `InternalsVisibleTo` attribute is the name of the assembly you want to expose your internals to.
With that attribute added, the error go away.

However, if you are also using a mocking framework you should also have the following assembly expose its internals:

```csharp
[assembly:InternalsVisibleTo("DynamicProxyGenAssembly2")]
```

`DynamicProxyGenAssembly2` is an assembly generated during the execution of tests when internals need to be mocked. All mocking frameworks will use that name.

### The `csproj` file

In general, having to create a class file just to make your internals visible to the test project, can feel a bit out of place.
A better idea is to move this logic into the csproj.

The first way we can do that is by using the `AssemblyAttribute` functionality of the `csproj`.

```xml
<ItemGroup>
    <AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleToAttribute">
        <_Parameter1>EdgeCases.Tests.Unit</_Parameter1>
    </AssemblyAttribute>
</ItemGroup>
```

Adding this snippet in our `csproj` will make MSBuild to drop a compile-time generated line to the `EdgeCases.AssemblyInfo.cs` file that lives in the `obj` folder, giving the same experience.

However there is an even simpler way.

Simply use the `InternalsVisibleTo` instruction to achieve the same thing.

```xml
<ItemGroup>
    <InternalsVisibleTo Include="EdgeCases.Tests.Unit"/>
</ItemGroup>
```

This is a cleaner and more modern way of doing exactly the same thing as above.

Now the last thing we can do to make this even more generic is replace the project name with a parameter.

```xml
<ItemGroup>
    <InternalsVisibleTo Include="$(AssemblyName).Tests.Unit" />
    <InternalsVisibleTo Include="DynamicProxyGenAssembly2" />
</ItemGroup>
```

We can now have the exact same snippet in our `csproj`, irrelevant from the project name, and be able to reuse it in a template to make internals visible to unit test projects.
