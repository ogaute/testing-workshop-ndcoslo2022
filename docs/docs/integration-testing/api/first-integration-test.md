---
description: Let's write our first integration test for the API
---

# Our first integration test

Let's begin by writing our first integration test. 
As a prerequisite, keep in mind that both the Database and the API are currently running on our workstation.

As a refresher, here are the command that I will be running
```commandline
docker compose up
dotnet run -c Release
```

## Project structure

The project structure that we followed until now for unit testing will continue to be followed for integration testing.
This includes the `src` and `tests` folders, the naming convention of the test project, 
the naming convention and folder structure of test classes and also the naming of tests.

## The test

Our first test will be a very simple one. We are going to test that when we creating a user with valid details, then the customer is created.
The name for this test will be `Create_CreatesCustomer_WhenDetailsAreValid`.

The structure of the test will be the exact same as in the unit test to start with.

```csharp title="CustomerControllerTests.cs"
[Fact]
public async Task Create_CreatesCustomer_WhenDetailsAreValid()
{
    // Arrange

    // Act

    // Assert
        
}
```

### Arrange

Previously we've used the arrange section to mock things out and created all the data needed. This is still the case in integration testing but there is no need to mock anything.
Since the API is already running we will just need to create our `HttpClient` and initialize our request and expected response object.

```csharp
// Arrange
var client = new HttpClient
{
    BaseAddress = new Uri("https://localhost:5001")
};

var request = new CustomerRequest
{
    Email = "nick@chapsas.com",
    FullName = "Nick Chapsas",
    DateOfBirth = new DateTime(1993, 01, 01),
    GitHubUsername = "nickchapsas"
};
```

These are the main two components we will be using for the test. The `HttpClient` will be used to call the running API. 
The `CustomerRequest` class is part of our API contracts. We even have access to the `CustomerResponse` API contract to assert against.
It is not uncommon for projects to export their API contracts to a dedicated project. 
In this case it doesn't matter because we will need a reference to the API project directly later.

### Act

The action we will perform is simple. We will simply call the API through the HttpClient.
We _could_ use the `PostAsync` method of the `HttpClient` but thankfully we have a helper method from `Microsoft` that will make sending the JSON object itself as the request body very simple.

```csharp
// Act
var response = await client.PostAsJsonAsync("customers", request);
```

### Assert

In integration tests, we tend to assert against more things than unit testing. This is mainly due to the fact that more things are being tested.

In this specific example we need to validate the following:

- That the response status code is `201` `Created`
- That the response body matches what we expect it to be
- That the `Location` header matches the `GET` location of the customer resource

The `StatusCode` and the `Headers` are both parameters of the `HttpResponseMessage` object that the `PostAsJsonAsync` method will return.

The `CustomerResponse` can be retrieved from the `Content` object of the response. There are multiple method provided by .NET to read the body.
Since we know that the result should be a JSON object we will use the `ReadFromJsonAsync<T>` method.

```csharp
// Assert
response.StatusCode.Should().Be(HttpStatusCode.Created);

var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
customerResponse.Should().BeEquivalentTo(request);

response.Headers.Location.Should().Be($"https://localhost:5001/customers/{customerResponse!.Id}");
```

And at it's core, that's it! However there is a _small_ problem. 

Taking a look at the database reveals that problem:

![](/img/integration/db-result.png)

The items created as part of our integration tests remain in the database. 
Depending on which database we are using and how we are dealing with it, this can be a big problem.

There are multiple ways to deal with this problem in integration testing but let's start with the simplest one.
