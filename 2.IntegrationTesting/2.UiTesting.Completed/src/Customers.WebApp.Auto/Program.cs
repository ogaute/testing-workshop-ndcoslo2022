using Microsoft.Playwright;

IPlaywright playwright = await Playwright.CreateAsync();

IBrowser browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
{
    SlowMo = 1000,
    Headless = false
});

IBrowserContext browserContext = await browser.NewContextAsync(new BrowserNewContextOptions
{
    IgnoreHTTPSErrors = true
});

IPage page = await browserContext.NewPageAsync();

await page.GotoAsync("https://localhost:5001/add-customer");

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

playwright.Dispose();
