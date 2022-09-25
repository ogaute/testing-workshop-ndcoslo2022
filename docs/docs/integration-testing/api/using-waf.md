---
description: Having to run the API manually is painful. Let's fix it
---

# Running the API smarter

## The problem

Even though the approach that we've seen until now works well enough and it does actually properly function as an integration testing approach,
it suffers from a few problems. 

- The API needs to be running at all times so the tests can target it
- A database needs to be running at all times so the API that is being called by the tests can target it

Let's start from the first problem

## Running the API

Historically, one of the biggest pains when it comes to integration testing is to actually run the API just for the tests and tear it down.
Technically we can still implement it for our usecase but we really don't need to. 

Microsoft has noticed that this was a problem that many people had for a very long time and they created the `WebApplicationFactory`.

## The WebApplicationFactory

The `WebApplicationFactory` is a class that allows us to create a `TestServer` that runs our API. 
This API is only accessible in-memory so a regular HttpClient or a browser can't access it. 
The only way to access it is to use an `HttpClient` specifically created to call this in-memory application.
This make is an excellent candidate for integration testing and it is by far the best way to write them.

Let's see how we can use it for our own existing test.

## Using the WebApplicationFactory

First let's shut down the running API. We don't need it anymore.

To use the `WebApplicationFactory` we need a few project-level changes.

1. In the `csproj` of the test project we need to change the `Project Sdk` from `Microsoft.NET.Sdk` to `Microsoft.NET.Sdk.Web`
2. We need to add the `Microsoft.AspNetCore.Mvc.Testing` Nuget package

New we can simply add the `WebApplicationFactory` as a field in our test class:

```csharp
private readonly WebApplicationFactory<> _waf = new();
```

As you might have noticed the `WebApplicationFactory` need a generic parameter. This parameter is an assembly scanning marker.
It can be any type inside the project we want to run. We could expose the `Program.cs` as an internal to the integration tests project but the preferred approach is to create an assembly marker.
An assembly marker is a simple empty type (usually an interface) that can be used by multiple things for assembly scanning.

```csharp
public interface IApiMarker {}
```

Now our WebApplicationFactory field can use this marker and it looks like this:

```csharp
private readonly WebApplicationFactory<IApiMarker> _waf = new();
```

Now we can replace the existing `HttpClient` initialization code to use an `HttpClient` generated from the `CreateClient` call of the WebApplicationFactory.

```csharp
private readonly WebApplicationFactory<IApiMarker> _waf = new();

private readonly HttpClient _client;
private readonly List<Guid> _idsToDelete = new();

public CustomerControllerTests()
{
    _client = _waf.CreateClient(new WebApplicationFactoryClientOptions
    {
        BaseAddress = new Uri("https://localhost:5001")
    });
}
```

_Note: We can even pass custom `WebApplicationFactoryClientOptions` for our client. This is optional but we are using it just to see how it would look like._

Now we can run the exact same test code and our test will pass:

![](/img/integration/waf-test.png)

This is still a completely valid integration test which calls the database and cleans the data up in the end.
