---
description: Integration tests can create data. Let's deal with that.
---

# Dealing with leftover data

## Cleanup

Cleanup, in one form or another, is an essential part of integration testing. We don't want to let our integration tests to leave data behind.
There are a few ways to deal with this problem.

What might come in mind is to create a connection against the database and delete the item by id once our test is completed its assertion.
This is actually a great idea and a very common technique. However, our case is a bit special.

Our API is a CRUD API, which means that it contains a `DELETE` endpoint to delete resources. 
We can use that in our integration tests to delete the created resource.

```csharp
// Cleanup
await client.DeleteAsync($"customers/{customerResponse.Id}");
```

With this being the final section in our integration test, we can run it and see that the test passes and there is no data left in the database. 

### Removing the cleanup part from the test

Now you might be thinking, "Oh great, now my test has to worry about cleaning data up too". This is a fair concern to have.
It's part of your test suite to cleanup the data but not necessarily that specific test's responsibility. Let's clean that up.

Instead of cleaning the data on each test we can make two changes in our test class.

First we can remove the `HttpClient` from being method specific and move it to a class field:

```csharp
public class CustomerControllerTests
{
    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri("https://localhost:5001")
    };
    
    ...
```

Now, under that, let's create a list that stores all the ids of the items we created and want to delete:

```csharp
private readonly List<Guid> _idsToDelete = new();
```

Then in our Cleanup section, instead of deleting the item we can just add its id in the list:

```csharp
// Cleanup
_idsToDelete.Add(customerResponse.Id);
```

Now here is where the magic comes in. We will change our test class to implement the `IAsyncLifetime` interface. 
This is a special xUnit interface that will allow us to asynchronously run code before and after a test run.
This will force us to implement two methods: `InitializeAsync` and `DisposeAsync`.

For now we won't care about `InitializeAsync` and simply make it return `Task.CompletedTask`.

```csharp
public Task InitializeAsync() => Task.CompletedTask;
```

In the `DisposeAsync`, however, we will add a loop that goes through the ids we want to delete and calls the `DeleteAsync` method of the client.

```csharp
public async Task DisposeAsync()
{
    foreach (var id in _idsToDelete)
    {
        await _client.DeleteAsync($"customers/{id}");
    }
}
```

And that's it! Now the cleanup code is outside our main code and we can keep adding items in that List, knowing that at the end of the test execution it will be cleaned up.

Here is the full code of the `CustomerControllerTests` class:

```csharp
public class CustomerControllerTests : IAsyncLifetime
{
    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri("https://localhost:5001")
    };

    private readonly List<Guid> _idsToDelete = new();

    [Fact]
    public async Task Create_CreatesCustomer_WhenDetailsAreValid()
    {
        // Arrange
        var request = new CustomerRequest
        {
            Email = "nick@chapsas.com",
            FullName = "Nick Chapsas",
            DateOfBirth = new DateTime(1993, 01, 01),
            GitHubUsername = "nickchapsas"
        };

        // Act
        var response = await _client.PostAsJsonAsync("customers", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        customerResponse.Should().BeEquivalentTo(request);

        response.Headers.Location.Should().Be($"https://localhost:5001/customers/{customerResponse!.Id}");
        
        // Cleanup
        _idsToDelete.Add(customerResponse.Id);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        foreach (var id in _idsToDelete)
        {
            await _client.DeleteAsync($"customers/{id}");
        }
    }
}
```
