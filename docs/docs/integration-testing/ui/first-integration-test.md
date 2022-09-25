---
description: Let's use what we've learned to write our first UI integration test
---

# Our first UI integration test 

Now that we have all the tools we need, let's start writing integration tests

## Folder and file structure

First let's create a folder called `Pages` and a folder `Customer` inside that folder. 
This is done to mirror the folder structure of the `Customers.WebApp` project and help us see at a glance what's being tested.

Our tests will be separated in classes just like our pages, but due to the collection definition in xUnit, they will all share their context.

## First test: Creating a customer

Our first test will be `Create_CreatesCustomer_WhenDataIsValid` and it will follow the same method structure as all of our other tests.

We first need to create the `AddCustomerTests.cs` class in which the tests will live.

```csharp title="AddCustomerTests.cs"
[Collection("Test collection")]
public class AddCustomerTests
{
    private readonly TestingContext _testingContext;
    
    public AddCustomerTests(TestingContext testingContext)
    {
        _testingContext = testingContext;
    }
}
```

And then create the main structure for the test

```csharp title="AddCustomerTests.cs"
[Fact]
public async Task Create_CreatesCustomer_WhenDataIsValid()
{
    // Arrange

    // Act

    // Assert
        
    // Cleanup
    
}
```

### Arrange

In the Arrange section we need to:

- Create a new Playwright browser page and navigate to the right page
- Create any data we need by calling the database directly or using the interface
- Setup any request or expected data objects
- Setup the fake GitHub API user

First let's create the new page and navigate to `/add-customer`.

```csharp
var page = await _testingContext.Browser.NewPageAsync();
await page.GotoAsync($"{TestingContext.AppUrl}/add-customer");

_testingContext.GitHubApiServer.SetupUser("nickchapsas");
```

### Act

In the Act section we will make all the actions needed in the page to make the thing that the test is testing for, happen.
In this specific case we will fill in all the data and click the submit button.

> We will use the exact same code we used in the previous section

```csharp title="AddCustomerTests.cs"
var fullNameInput = await page.QuerySelectorAsync("#fullname");
await fullNameInput!.FillAsync("Nick Chapsas");

var emailInput = await page.QuerySelectorAsync("#email");
await emailInput!.FillAsync("nick@chapsas.com");

var githubUsernameInput = await page.QuerySelectorAsync("#github-username");
await githubUsernameInput!.FillAsync("nickchapsas");

var dateOfBirthInput = await page.QuerySelectorAsync("#dob");
await dateOfBirthInput!.FillAsync("1993-09-22");

var submitBtn = await page.QuerySelectorAsync("#create-customer-form > button");
await submitBtn!.ClickAsync();
```

### Assert

There are a couple of ways to assert that the action happened successfully. 
We can either navigate the listing page or the "get by id" page and validate the data there.
However this might not always be possible, so creating a database connection, querying for the data and asserting the results is also a viable alternative.

Let's go back to the `TestingContext` and add setup the `IDbConnectionFactory` needed to create the database connection.

First let's add the `IDbConnectionFactory` as a property:

```csharp
public IDbConnectionFactory Database { get; private set; }
```

And then let's initialize a connection in the `InitializeAsync` method:

```csharp
public async Task InitializeAsync()
{
    // highlight-start
    Database = new NpgsqlConnectionFactory(
        "Server=localhost;Port=5435;Database=mydb;User ID=workshop;Password=changeme;");
    // highlight-end
    
    GitHubApiServer.Start(9850);
    _dockerService.Start();
 
    _playwright = await Playwright.CreateAsync();
    var browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
    {
        SlowMo = 1000,
        Headless = false
    });
    
    Browser = await browser.NewContextAsync(new BrowserNewContextOptions
    {
        IgnoreHTTPSErrors = true
    });
}
```

Now we have two options. We either write the raw queries using `Dapper` and map the results to the `CustomerDto` and validate against that
or we use the `CustomerRepository` that already exists in the Web App project and use that to make all our database actions.

The pragmatic approach here is to use the `CustomerRepository` but you can follow whichever approach you want in this case.

Let's go ahead and add a field of type `ICustomerRepository` in the `AddCustomerTests` class and initialize it using the `IDbConnectionFactory`.

```csharp
private readonly TestingContext _testingContext;
// highlight-next-line
private readonly ICustomerRepository _customerRepository;

public AddCustomerTests(TestingContext testingContext)
{
    _testingContext = testingContext;
    // highlight-next-line
    _customerRepository = new CustomerRepository(_testingContext.Database);
}
```

We can now use the `GetAsync` method to get the user that we just created by id or the `GetAllAsync` method and isolate it assuming it is the only one.

In order to get the ID of the newly created customer we need to read the `href` attribute value of the `here` text on the successful creation page:

![](/img/integration/get-id.png)

Since this is a "safer" approach, because it doesn't assume that this is the only element in the database, we will go ahead with that.

```csharp
var customerLink = await page.QuerySelectorAsync("body > div.page > main > article > p:nth-child(3) > a");
var href = await customerLink!.GetAttributeAsync("href");
var customerIdText = href!.Replace("/customer/", string.Empty);
var customerId = Guid.Parse(customerIdText);
```

The code above will give up the id of the customer we just created and we can now use that to get the user for the database and asset their properties.

```csharp
var createdCustomer = await _customerRepository.GetAsync(customerId);

createdCustomer.Should().NotBeNull();
createdCustomer!.FullName.Should().Be("Nick Chapsas");
createdCustomer.Email.Should().Be("nick@chapsas.com");
createdCustomer.GitHubUsername.Should().Be("nickchapsas");
createdCustomer.DateOfBirth.Should().Be(new DateTime(1993, 9, 22));
```

### Cleanup

There are two things we need to handle during cleanup.
First and foremost we need to close the page of the test we just executed. It isn't needed anymore.

Then, we have to delete the customer we just created.
Since we are running the tests sequentially we need to clean the data as part of our test.
That being said, we are writing these tests in an isolated manner so technically we could delete at the end or don't delete at all, 
but it a good practice in general to clean up to eliminate any chance of data interfering with each other in unexpected ways.

```csharp
// Cleanup
await page.CloseAsync();
await _customerRepository.DeleteAsync(customerId);
```

And that's it! We can now run the test and watch it pass!

![](/img/integration/createtest.gif)

## Using Bogus

The last thing left to do is to use Bogus to replace the manual creation of data. The approach is exactly the same as before.

First we need to create the generator:

```csharp
private readonly Faker<CustomerDto> _customerGenerator = new Faker<CustomerDto>()
    .RuleFor(x => x.Id, Guid.NewGuid)
    .RuleFor(x => x.Email, f => f.Person.Email)
    .RuleFor(x => x.FullName, f => f.Person.FullName)
    .RuleFor(x => x.DateOfBirth, f => f.Person.DateOfBirth.Date)
    .RuleFor(x => x.GitHubUsername, f => f.Person.UserName.Replace(".", "").Replace("-", "").Replace("_", ""));
```

> You can also use the `Customer` type found under `Models` but you need to handle the `DateOnle`->`DateTime` type match with a special condition.

And then we need to update our test to create a customer and use it:

```csharp
[Fact]
public async Task Create_CreatesCustomer_WhenDataIsValid()
{
    // Arrange
    var page = await _testingContext.Browser.NewPageAsync();
    await page.GotoAsync($"{TestingContext.AppUrl}/add-customer");

    var customer = _customerGenerator.Generate();
    _testingContext.GitHubApiServer.SetupUser(customer.GitHubUsername);
    
    // Act
    var fullNameInput = await page.QuerySelectorAsync("#fullname");
    await fullNameInput!.FillAsync(customer.FullName);
    
    var emailInput = await page.QuerySelectorAsync("#email");
    await emailInput!.FillAsync(customer.Email);
    
    var githubUsernameInput = await page.QuerySelectorAsync("#github-username");
    await githubUsernameInput!.FillAsync(customer.GitHubUsername);
    
    var dateOfBirthInput = await page.QuerySelectorAsync("#dob");
    await dateOfBirthInput!.FillAsync(customer.DateOfBirth.ToString("yyyy-MM-dd"));
    
    var submitBtn = await page.QuerySelectorAsync("#create-customer-form > button");
    await submitBtn!.ClickAsync();

    // Assert
    var customerLink = await page.QuerySelectorAsync("body > div.page > main > article > p:nth-child(3) > a");
    var href = await customerLink!.GetAttributeAsync("href");
    var customerIdText = href!.Replace("/customer/", string.Empty);
    var customerId = Guid.Parse(customerIdText);

    var createdCustomer = await _customerRepository.GetAsync(customerId);

    createdCustomer.Should().BeEquivalentTo(customer, options => options.Excluding(x => x.Id));

    // Cleanup
    await page.CloseAsync();
    await _customerRepository.DeleteAsync(customerId);
}
```

And that's it! We now have a full automated UI-based integration test in place with its own docker-specific database, fake GitHub API and service!

Let's go ahead and write more tests!
