using Bogus;
using Customers.Api.Repositories;
using Customers.WebApp.Data;
using Customers.WebApp.Repositories;
using FluentAssertions;
using Xunit;

namespace Customers.WebApp.Tests.Integration.Pages.Customer;

[Collection("Test collection")]
public class AddCustomerTests
{
    private readonly TestingContext _testingContext;
    private readonly ICustomerRepository _customerRepository;
    private readonly Faker<CustomerDto> _customerGenerator = new Faker<CustomerDto>()
        .RuleFor(x => x.Id, Guid.NewGuid)
        .RuleFor(x => x.Email, f => f.Person.Email)
        .RuleFor(x => x.FullName, f => f.Person.FullName)
        .RuleFor(x => x.DateOfBirth, f => f.Person.DateOfBirth.Date)
        .RuleFor(x => x.GitHubUsername, f => f.Person.UserName.Replace(".", "").Replace("-", "").Replace("_", ""));
    
    public AddCustomerTests(TestingContext testingContext)
    {
        _testingContext = testingContext;
        _customerRepository = new CustomerRepository(_testingContext.Database);
    }

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

        createdCustomer.Should().BeEquivalentTo(customer, x => x.Excluding(p => p.Id));

        // Cleanup
        await page.CloseAsync();
        await _customerRepository.DeleteAsync(customerId);
    }
    
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
}
