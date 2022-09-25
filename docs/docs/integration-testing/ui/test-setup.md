---
description: Let's setup our test suite for UI testing
---

# Bootstrapping our tests

## The test context

Since we will be dealing with Docker, the fake GitHub API and the tool that will be making browser actions in our tests, 
we need to build a class that will contain all that logic. In the API testing section we had the WebApplicationFactory.
For our Web App testing we will have the `TestingContext`. It will largely play a similar role in terms of starting and stopping Docker components and the GitHub API as well as the in-memory browser.

Let's create this class and for now only include the WireMock.NET Server.

```csharp
public class TestingContext : IAsyncLifetime
{
    public GitHubApiServer GitHubApiServer { get; } = new();

    public async Task InitializeAsync()
    {
        GitHubApiServer.Start(9850);
    }

    public async Task DisposeAsync()
    {
        GitHubApiServer.Dispose();
    }
}
```

As you can see we are still following the same `IAsyncLifetime` approach to have things happen during test startup and teardown.
The only difference with our previous approach is that the `GitHubApiServer` accepts a port as part of the `Start` method to ensure that the server starts on the same port every time.

## The test execution flow

Because we will be doing UI testing, it will be significantly easier to run our tests sequentially. 
This prevents any potential clashes that could make our life harder. It is possible to run those tests in parallel but you need to be very careful with preventing database-level clashes.

To ensure that tests will be running sequentially even on separate classes we will need to create a Collection Fixture.

```csharp
[CollectionDefinition("Test collection")]
public class SharedTestCollection : ICollectionFixture<TestingContext>
{
    
}
```

This is all we need this class to contain. It acts as a marker and we passed the `TestingContext` class as a parameter to ensure that it is reused between all tests.

Now we simply decorate our test classes with `[Collection("Test collection")]` and we can inject the `TestingContext` class in the constructor.

This is what the `CreateCustomerTests.cs` class containing our tests for creating a customer will look like to start with:

```csharp
[Collection("Test collection")]
public class CreateCustomerTests
{
    private readonly SharedTestContext _testContext;    

    public CreateCustomerTests(SharedTestContext testContext)
    {
        _testContext = testContext;
    }    
}
```

We are now ready to introduce the new way of running Docker Compose for testing.
