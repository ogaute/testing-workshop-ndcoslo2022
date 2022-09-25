---
description: Time for you to practice by writing the rest of the tests
---

# Writing the rest of the tests

With all that said, let's write our second test and then hand it over to you to write the rest of them.

## Testing that some data exists

The second test we need to write is the `Get_ReturnsCustomer_WhenCustomerExists`. 
For this test we need to create a customer as part of `Arrange`, try to retrieve them as part of `Act` and then validate that they are returned in the response in `Assert`.
We also have to add the customer id in the list so it can get deleted.

<details>
<summary>Get_ReturnsCustomer_WhenCustomerExists</summary>

```csharp
[Fact]
public async Task Get_ReturnsCustomer_WhenCustomerExists()
{
    // Arrange
    var request = new CustomerRequest
    {
        Email = "nick@chapsas.com",
        FullName = "Nick Chapsas",
        DateOfBirth = new DateTime(1993, 01, 01),
        GitHubUsername = "nickchapsas"
    };

    var createCustomerHttpResponse = await _client.PostAsJsonAsync("customers", request);
    var createdCustomer = await createCustomerHttpResponse.Content.ReadFromJsonAsync<CustomerResponse>();

    // Act
    var response = await _client.GetAsync($"customers/{createdCustomer!.Id}");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();

    customerResponse.Should().BeEquivalentTo(createdCustomer);
    
    // Cleanup
    _idsToDelete.Add(customerResponse!.Id);
}
```

</details>

## Exercise: Write the following tests

The tests we need are:

- `Create_ReturnsBadRequest_WhenTheEmailIsInvalid` (The object used for bad requests is `ValidationProblemDetails`)
- `GetAll_ReturnsAllCustomers_WhenCustomersExist` (One customer is enough. The object used is `GetAllCustomersResponse`)
- `Get_ReturnsNotFound_WhenCustomerDoesNotExist`
- `Update_UpdatesCustomerDetails_WhenDetailsAreValid`
- `Delete_DeletesCustomer_WhenCustomerExists`
- `Delete_ReturnsNotFound_WhenCustomerDoesNotExist`

### Solutions

Only expand the solutions if you are stuck. You are **highly encouraged** to try and write the tests yourself. 
Practice makes perfect and you only learn by doing.

<details>
<summary>Create_ReturnsBadRequest_WhenTheEmailIsInvalid</summary>

```csharp
[Fact]
public async Task Create_ReturnsBadRequest_WhenTheEmailIsInvalid()
{
    // Arrange
    var request = new CustomerRequest
    {
        Email = "nick",
        FullName = "Nick Chapsas",
        DateOfBirth = new DateTime(1993, 01, 01),
        GitHubUsername = "nickchapsas"
    };

    // Act
    var response = await _client.PostAsJsonAsync("customers", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

    var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
    problemDetails!.Errors["Email"].Should().Equal("nick is not a valid email address");
}
```

</details>

<details>
<summary>GetAll_ReturnsAllCustomers_WhenCustomersExist</summary>

```csharp
[Fact]
public async Task GetAll_ReturnsAllCustomers_WhenCustomersExist()
{
    // Arrange
    var request = new CustomerRequest
    {
        Email = "nick@chapsas.com",
        FullName = "Nick Chapsas",
        DateOfBirth = new DateTime(1993, 01, 01),
        GitHubUsername = "nickchapsas"
    };
    
    var createCustomerHttpResponse = await _client.PostAsJsonAsync("customers", request);
    var createdCustomer = await createCustomerHttpResponse.Content.ReadFromJsonAsync<CustomerResponse>();

    // Act
    var response = await _client.GetAsync("customers");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var customerResponse = await response.Content.ReadFromJsonAsync<GetAllCustomersResponse>();

    customerResponse!.Customers.Should().ContainEquivalentOf(createdCustomer)
        .And.HaveCount(1);

    // Cleanup
    _idsToDelete.Add(createdCustomer!.Id);
}
```

</details>


<details>
<summary>Get_ReturnsNotFound_WhenCustomerDoesNotExist</summary>

```csharp
[Fact]
public async Task Get_ReturnsNotFound_WhenCustomerDoesNotExist()
{
    // Arrange
    var customerId = Guid.NewGuid();

    // Act
    var response = await _client.GetAsync($"customers/{customerId}");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
}
```

</details>


<details>
<summary>Update_UpdatesCustomerDetails_WhenDetailsAreValid</summary>

```csharp
[Fact]
public async Task Update_UpdatesCustomerDetails_WhenDetailsAreValid()
{
    // Arrange
    var createRequest = new CustomerRequest
    {
        Email = "nick@chapsas.com",
        FullName = "Nick Chapsas",
        DateOfBirth = new DateTime(1993, 01, 01),
        GitHubUsername = "nickchapsas"
    };
    
    var createCustomerHttpResponse = await _client.PostAsJsonAsync("customers", createRequest);
    var createdCustomer = await createCustomerHttpResponse.Content.ReadFromJsonAsync<CustomerResponse>();
    
    var updateRequest = new CustomerRequest
    {
        Email = "chapsas@nick.com",
        FullName = "Nick Chapsas",
        DateOfBirth = new DateTime(1993, 01, 01),
        GitHubUsername = "nickchapsas"
    };

    // Act
    var response = await _client.PutAsJsonAsync($"customers/{createdCustomer!.Id}", updateRequest);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    
    var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
    customerResponse.Should().BeEquivalentTo(updateRequest);
    
    // Cleanup
    _idsToDelete.Add(customerResponse!.Id);
}
```

</details>


<details>
<summary>Delete_DeletesCustomer_WhenCustomerExists</summary>

```csharp
[Fact]
public async Task Delete_DeletesCustomer_WhenCustomerExists()
{
    // Arrange
    var createRequest = new CustomerRequest
    {
        Email = "nick@chapsas.com",
        FullName = "Nick Chapsas",
        DateOfBirth = new DateTime(1993, 01, 01),
        GitHubUsername = "nickchapsas"
    };
    
    var createCustomerHttpResponse = await _client.PostAsJsonAsync("customers", createRequest);
    var createdCustomer = await createCustomerHttpResponse.Content.ReadFromJsonAsync<CustomerResponse>();

    // Act
    var response = await _client.DeleteAsync($"customers/{createdCustomer!.Id}");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
}
```

</details>


<details>
<summary>Delete_ReturnsNotFound_WhenCustomerDoesNotExist</summary>

```csharp
[Fact]
public async Task Delete_ReturnsNotFound_WhenCustomerDoesNotExist()
{
    // Arrange
    var customerId = Guid.NewGuid();

    // Act
    var response = await _client.DeleteAsync($"customers/{customerId}");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
}
```

</details>
