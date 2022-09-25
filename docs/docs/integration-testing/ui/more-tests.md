---
description: Let's write tests for the remaining use-cases
---

# More complex use-cases

## The "unhappy" path

Now that the main happy path for customer creation is covered, let's go ahead an write a test for an "unhappy" path.

We have quite a few options:

- Invalid Full name, doesn't create customer and return appropriate error message
- Invalid Email, doesn't create customer and return appropriate error message
- Invalid GitHub Username, doesn't create customer and return appropriate error message
- Invalid Date of Birth, doesn't create customer and return appropriate error message

Ultimately all of them should be covered, however for the purposes of the workshop we will only cover one in this section and one during your exercise.

Let's start with the `Create_ShowsErrorMessage_WhenEmailIsInvalid` test. First and foremost let's see what happens on the website when the email is invalid:

![](/img/integration/invalidemail.png)

So as we can see an error message is displayed after we unfocus from the email input with an invalid email format.
There are multiple ways to tackle this but let's go with a simple approach.

Let's write a test that:

1. Navigates to the `/add-customer` page
2. Inputs an invalid email and unfocuses from the field
3. Check that the "Invalid email format" has appeared in the error list

> An alternative approach would be to fill in all the data with an invalid email, click submit and check that we are still at 
> `/add-customer` and no customer was created in the database.

First let's create a test named `Create_ShowsErrorMessage_WhenEmailIsInvalid`:

```csharp
[Fact]
public async Task Create_ShowsErrorMessage_WhenEmailIsInvalid()
{
    // Arrange

    // Act

    // Assert

    // Cleanup
    
}
```

### Arrange

Arrange will need to create the Playwright page and navigate to `/add-customer`. 
Since we won't be making any calls, we won't be generating a customer and we don't be setting up a GitHub user.

```csharp
var page = await _testingContext.Browser.NewPageAsync();
await page.GotoAsync($"{TestingContext.AppUrl}/add-customer");
```

### Act

In the action we will first fill in a fake email using the email element selector and then we will move to the next element
which is the github username input, and focus into it. This is enough to trigger the client-side validation.

> _Keep in mind that you would also have to check the server-side validation with a separate test_

```csharp
var emailInput = await page.QuerySelectorAsync("#email");
await emailInput!.FillAsync("notanemail");

var githubUsernameInput = await page.QuerySelectorAsync("#github-username");
await githubUsernameInput!.FocusAsync();
```

### Assert

In the assert section we need to get the contents of the `validation-message` list and make sure it contains `Invalid email format`.

![](/img/integration/invalidlist.png)

Since this list can contain multiple items, we will use `QuerySelectorAllAsync` instead of the `QuerySelectorAsync` and ensure that the needed test is contained in one of the list items.

```csharp
var validationListItems = await page.QuerySelectorAllAsync("#create-customer-form > ul > li");

var matched = false;
foreach (var validationListItem in validationListItems)
{
    var errorText = await validationListItem.InnerTextAsync();
    if (errorText.Equals("Invalid email format"))
    {
        matched = true;
        break;
    }
}

matched.Should().BeTrue();
```

### Cleanup

Last but not least we need to make sure the page is closed:

```csharp
await page.CloseAsync();
```

<details>
<summary>The full test</summary>

```csharp
[Fact]
public async Task Create_ShowsErrorMessage_WhenEmailIsInvalid()
{
    // Arrange
    var page = await _testingContext.Browser.NewPageAsync();
    await page.GotoAsync($"{TestingContext.AppUrl}/add-customer");
    
    // Act
    var emailInput = await page.QuerySelectorAsync("#email");
    await emailInput!.FillAsync("notanemail");
    
    var githubUsernameInput = await page.QuerySelectorAsync("#github-username");
    await githubUsernameInput!.FocusAsync();

    // Assert
    var validationListItems = await page.QuerySelectorAllAsync("#create-customer-form > ul > li");

    var matched = false;
    foreach (var validationListItem in validationListItems)
    {
        var errorText = await validationListItem.InnerTextAsync();
        if (errorText.Equals("Invalid email format"))
        {
            matched = true;
            break;
        }
    }

    matched.Should().BeTrue();

    // Cleanup
    await page.CloseAsync();
}
```

</details>

## Exercise: The rest of the tests

Now it is your turn to practice. 
There are many tests that can be written for our website and you should take it upon your self to practice them even if they are not in the list below.
Remember, practice makes perfect and you only learn by doing.

Write the following tests:

- `Get_ReturnsCustomer_WhenCustomerExists`
- `GetAll_ContainsCustomer_WhenCustomerExists`
- `Update_UpdatesCustomer_WhenDataIsValid`
- `Delete_DeletesCustomer_WhenCustomerExists`

:::tip
If you need to Accept a browser-level dialog (like the one that appears when you delete a customer),
you can use the following piece of code:

```csharp
page.Dialog += (_, dialog) => dialog.AcceptAsync();
```
:::

Challenge yourselves to find more actions to test and only expand the solutions below if you are stuck and you need a little help.

### Solutions

<details>
<summary>Get_ReturnsCustomer_WhenCustomerExists</summary>

```csharp title="GetCustomerTests.cs"
[Fact]
public async Task Get_ReturnsCustomer_WhenCustomerExists()
{
    // Arrange
    var page = await _testingContext.Browser.NewPageAsync();
    
    var customer = _customerGenerator.Generate();
    _testingContext.GitHubApiServer.SetupUser(customer.GitHubUsername);
    await _customerRepository.CreateAsync(customer);

    // Act
    await page.GotoAsync($"{TestingContext.AppUrl}/customer/{customer.Id}");

    // Assert
    var fullNameParagraph = await page.QuerySelectorAsync("#fullname-field");
    var fullName = await fullNameParagraph!.InnerTextAsync();

    var emailParagraph = await page.QuerySelectorAsync("#email-field");
    var email = await emailParagraph!.InnerTextAsync();
    
    var githubUsernameParagraph = await page.QuerySelectorAsync("#github-username-field");
    var githubUsername = await githubUsernameParagraph!.InnerTextAsync();
    
    var dateOfBirthParagraph = await page.QuerySelectorAsync("#dob-field");
    var dateOfBirth = await dateOfBirthParagraph!.InnerTextAsync();

    fullName.Should().Be(customer.FullName);
    email.Should().Be(customer.Email);
    githubUsername.Should().Be(customer.GitHubUsername);
    dateOfBirth.Should().Be(customer.DateOfBirth.ToString("dd/MM/yyyy"));
    
    // Cleanup
    await page.CloseAsync();
    await _customerRepository.DeleteAsync(customer.Id);
}
```

</details>

<details>
<summary>GetAll_ContainsCustomer_WhenCustomerExists</summary>

```csharp title="GetAllCustomersTests.cs"
[Fact]
public async Task GetAll_ContainsCustomer_WhenCustomerExists()
{
    // Arrange
    var page = await _testingContext.Browser.NewPageAsync();
    
    var customer = _customerGenerator.Generate();
    _testingContext.GitHubApiServer.SetupUser(customer.GitHubUsername);
    await _customerRepository.CreateAsync(customer);

    // Act
    await page.GotoAsync($"{TestingContext.AppUrl}/customers");

    // Assert
    var tableRows = await page.QuerySelectorAsync("body > div.page > main > article > table > tbody > tr");

    var rowItems = await tableRows!.QuerySelectorAllAsync("td");

    var fullName = await rowItems[0].InnerTextAsync();
    var email = await rowItems[1].InnerTextAsync();
    var githubUsername = await rowItems[2].InnerTextAsync();
    var dateOfBirth = await rowItems[3].InnerTextAsync();

    fullName.Should().Be(customer.FullName);
    email.Should().Be(customer.Email);
    githubUsername.Should().Be(customer.GitHubUsername);
    dateOfBirth.Should().Be(customer.DateOfBirth.ToString("dd/MM/yyyy"));
    
    // Cleanup
    await page.CloseAsync();
    await _customerRepository.DeleteAsync(customer.Id);
}
```

</details>

<details>
<summary>Update_UpdatesCustomer_WhenDataIsValid</summary>

```csharp title="UpdateCustomerTests.cs"
[Fact]
public async Task Update_UpdatesCustomer_WhenDataIsValid()
{
    // Arrange
    var page = await _testingContext.Browser.NewPageAsync();

    var customer = _customerGenerator.Generate();
    var newCustomer = _customerGenerator.Generate();
    _testingContext.GitHubApiServer.SetupUser(customer.GitHubUsername);
    await _customerRepository.CreateAsync(customer);
    
    await page.GotoAsync($"{TestingContext.AppUrl}/update-customer/{customer.Id}");
    
    // Act
    var fullNameInput = await page.QuerySelectorAsync("#fullname");
    await fullNameInput!.FillAsync(newCustomer.FullName);
    
    var submitBtn = await page.QuerySelectorAsync("#update-customer-form > button");
    await submitBtn!.ClickAsync();

    // Assert
    var updatedCustomer = await _customerRepository.GetAsync(customer.Id);

    updatedCustomer!.FullName.Should().Be(newCustomer.FullName);

    // Cleanup
    await page.CloseAsync();
    await _customerRepository.DeleteAsync(customer.Id);
}
```

</details>

<details>
<summary>Delete_DeletesCustomer_WhenCustomerExists</summary>

```csharp title="DeleteCustomerTests.cs"
[Fact]
public async Task Delete_DeletesCustomer_WhenCustomerExists()
{
    // Arrange
    var page = await _testingContext.Browser.NewPageAsync();
    
    var customer = _customerGenerator.Generate();
    _testingContext.GitHubApiServer.SetupUser(customer.GitHubUsername);
    await _customerRepository.CreateAsync(customer);
    
    await page.GotoAsync($"{TestingContext.AppUrl}/customers");
    page.Dialog += (_, dialog) => dialog.AcceptAsync();

    // Act
    var deleteBtn = await page.QuerySelectorAsync("body > div.page > main > article > table > tbody > tr > td:nth-child(5) > button.btn.btn-danger");
    await deleteBtn!.ClickAsync();
    
    // Assert
    var customerExists = await _customerRepository.GetAsync(customer.Id);
    customerExists.Should().BeNull();
    
    // Cleanup
    await page.CloseAsync();
}
```

</details>
