---
description: Let's see how we can run an in-memory browser for our tests
---

# Introduction to Browser testing

Since we are writing **top-down tests** we need to have tests that interact with the browser since that's the "top" for our users.

This is where a product called [Playwright](https://playwright.dev) comes in.

## An automatic flow

Before we start using Playwright for our testing, let's first write our first automatic flow with it against our real application.

## Installation

First, let's install Playwright at the `Customers.WebApp.Auto` project under `2.UiTesting/src`.
You can install it as a Nuget package either by using your IDE's Nuget client or by running:

```csharp
dotnet add package Microsoft.Playwright
```

Now build the project either using your IDE or by running:

```commandline
dotnet build
```

This will create a few files under the `Customers.WebApp.Auto\bin\Debug\net6.0` folder.
What we need to do is run the file called `playwright.ps1`. You will need to have powershell installed to run it.
PowerShell is cross platform and can be installed [here](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell?view=powershell-7.2).

```commandline
pwsh bin/Debug/netX/playwright.ps1 install
```

Once that's done, you're ready to start writing your first flow.

An alternative to the approach above is to create a console app that only contains the following line, and run it:

```csharp
Microsoft.Playwright.Program.Main(new string[] {"install" });
```

## Scenario: Creating a customer 
 
Our goal here is to use Playwright, to automate the creation of a customer.
This is the view we are going to automate:

![](/img/integration/create-customer-age.png)

We need to:

1. Navigate to `/add-customer`
2. Provide a valid full name
3. Provide a valid email
4. Provide a valid GitHub username
5. Provide a valid date of birth
6. Click the submit button

### Creating the infrastructure

Let's start by the infrastructure code needed. First we need to create an instance of Playwright itself:

```csharp
IPlaywright playwright = await Playwright.CreateAsync();
```

We can then create an instance of the Browser:

```csharp
IBrowser browser = await playwright.Chromium.LaunchAsync();
```

However, by default the browser will run headless which means we won't actually see a browser window.
It will also make the actions as fast as it can. To prevent that we will pass a `BrowserTypeLaunchOptions` object to the `LaunchAsync` 
method to add `SlowMo` and turn Headless off.

```csharp
IBrowser browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
{
    SlowMo = 1000,
    Headless = false
});
```

Before we create our `IPage` object we need to create an `IBrowserContext`. This will allow us to run in an isolated context of the 
browser without sharing data such as cookies and it also allows us to ignore invalid HTTPS certificates which can be a pain during testing:

```csharp
IBrowserContext browserContext = await browser.NewContextAsync(new BrowserNewContextOptions
{
    IgnoreHTTPSErrors = true
});
```

We can now finally create our `IPage` object which is what we will be using to interact with the website page:

```csharp
IPage page = await browserContext.NewPageAsync();
```

Let's start by navigating to the `/add-customer` page:

```csharp
await page.GotoAsync("https://localhost:5001/add-customer");
```

Let's just try that out first before we move further. Let's add a `Dispose` call so the browser is properly disposed at the end of the execution and run the console app.

```csharp
playwright.Dispose();
```

```commandline
dotnet run
```

![](/img/integration/show-browser.gif)

### Interacting with the page

Now that we have the page running let's go ahead and fill in and submit the data. Any interaction will be driven by HTML DOM element selectors. 

For example this is what the HTML for the fullname field looks like:

![](/img/integration/fullname.png)

This item can be selected in many different ways from "give me the first `input` in the page" to give me the element with `id="fullname"`.
Selecting won't always be this easy but here is a handy tip.

Browsers such as Google Chrome allow you to Copy the Selector for every element. Simply right click to the element you want to select and copy the selector.

![](/img/integration/selector.png)

This returns `#fullname` which is the selector we need to use.

We can use this selector with the `QuerySelectorAsync` method of the `page` object to select the input element.

```csharp
var fullNameInput = await page.QuerySelectorAsync("#fullname");
```

Now that we have the element selected we can use the `FillAsync` method to set the value:

```csharp
await fullNameInput!.FillAsync("Nick Chapsas");
```

Let's run the console app again and see what's going to happen this time:

![](/img/integration/fullname-fill.gif)

With that working let's fill in the rest of the fields:

```csharp
var emailInput = await page.QuerySelectorAsync("#email");
await emailInput!.FillAsync("nick@chapsas.com");

var githubUsernameInput = await page.QuerySelectorAsync("#github-username");
await githubUsernameInput!.FillAsync("nickchapsas");

var dateOfBirthInput = await page.QuerySelectorAsync("#dob");
await dateOfBirthInput!.FillAsync("1993-09-22");
```

The last thing we need is to click the submit button.
Our approach is the same. Get the selector, use it go get the element and use the `ClickAsync` method of the element.

```csharp
var submitBtn = await page.QuerySelectorAsync("#create-customer-form > button");
await submitBtn!.ClickAsync();
```

And that's it! We can now create a customer with an automated Playwright flow!
 
![](/img/integration/createflow.gif)

We can now integrate Playwright in our test suite.

## Test suite integration

It's time to take everything we've learned and use it for our integration test suite.

First, let's create the `IPlaywright` option and the `IBrowserContext` object in the `TestingContext` class.

```csharp
private readonly IPlaywright _playwright;

public IBrowserContext Browser { get; private set; }
```

`Browser` is public because it will be used by the test classes to invoke the `NewPageAsync` method and create a page to run the tests on.

Now simply update the `InitializeAsync` method to include the Browser launching:

```csharp
public async Task InitializeAsync()
{
    GitHubApiServer.Start(9850);
    _dockerService.Start();
    
    // highlight-start
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
    // highlight-end
}
```

And the `DisposeAsync` method to dispose the browser and playwright:

```csharp
public async Task DisposeAsync()
{
    // highlight-start
    await Browser.DisposeAsync();
    _playwright.Dispose();
    // highlight-end
    _dockerService.Dispose();
    GitHubApiServer.Dispose();
}
```

And that's it! Now it's time to take everything we've learned and write integration tests with it!
