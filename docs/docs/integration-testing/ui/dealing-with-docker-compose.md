---
description: Let's see how we can start and stop Docker for testing 
---

# Running Docker Compose

Since we won't be using Testcontainers to run our tests we need a way to run the integration testing specific docker-compose.yml file.
We will be doing that using a Nuget package called `Ductus.FluentDocker`. It should already be installed in the `Customers.WebApp.Tests.Integration` project.

This package allows us to use a fluent API to control docker from our tests. The core idea is the same as with Testcontainers.
When tests start running, we will start the `docker-compose.integration.yml` file and when they are done we will stop.

## Implementing the code

All of our code will be going into the `TestingContext.cs` since that's the class controlling the lifecycle of our tests.

First we need to create a path to our docker compose file:

```csharp
private static readonly string DockerComposeFile = Path.Combine(Directory.GetCurrentDirectory(), (TemplateString)"../../../docker-compose.integration.yml");
```

And then we need to create an `ICompositeService` using FluentDocker's `Builder` class.

```csharp
public const string AppUrl = "https://localhost:7780";

private readonly ICompositeService _dockerService = new Builder()
    .UseContainer()
    .UseCompose()
    .FromFile(DockerComposeFile)
    .RemoveOrphans()
    .WaitForHttp("test-app", AppUrl)
    .Build();
```

The `_dockerService` will now use our `docker-compose.integration.yml` and will wait until `test-app` is available before it moves on when we invoke the `Start()` method,
ensuring that our service and database is running before any tests run.

Then all we need to do is call the `_dockerService.Start();` method in the `InitializeAsync` and the `_dockerService.Dispose();` method in the `DisposeAsync` method of the `TestingContext`.

And that's it. All the setup needed for Docker and its execution is done and we can move to taking a loot at how we can run an in-memory browser for our tests.
