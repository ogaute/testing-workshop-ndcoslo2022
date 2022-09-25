---
description: Creating our own WebApplicationFactory
---

# Our own WebApplicationFactory

The WebApplicationFactory has some great functionality out of the box but when we are doing real life testing there are several things that need to be customized.
This can be done in each individual test class but a better approach is to create our own custom WebApplicationFactory and customize it in a single place.

## Creating the custom WebApplicationFactory

All we need to do to create the custom WebApplicationFactory is to create a class for our new factory and inherit from our WebApplicationFactory.

```csharp
public class CustomerApiFactory : WebApplicationFactory<IApiMarker>
{    
}
```

Simple as that. Now we have a custom WAF that we can modify in any way we want. 
For example, it is very common to remove all logging providers from the API we are running for the tests because they don't really need to log anything.

To do that we can override the `ConfigureWebHost` method and clear the providers by calling the `ConfigureLogging` method.

```csharp
public class CustomerApiFactory : WebApplicationFactory<IApiMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });
    }
}
```

Now to use it in our test class we will change our approach a bit. 
Instead of instantiating in the class itself we will have the class implement `IClassFixture<CustomerApiFactory>`.
That way, the lifetime of the `CustomerApiFactory` will be one per collection execution which means it can be shared by all tests.
To instantiate it now we simply inject it from the constructor.

This is what the class looks like now:

```csharp
public class CustomerControllerTests : IClassFixture<CustomerApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly List<Guid> _idsToDelete = new();

    public CustomerControllerTests(CustomerApiFactory customerApiFactory)
    {
        _client = customerApiFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost:5001")
        });
    }

...
```

This allows us to have full control over the creation and cleanup of both the test collection and each individual test.
