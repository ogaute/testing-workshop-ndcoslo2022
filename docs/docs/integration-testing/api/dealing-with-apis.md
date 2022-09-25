---
description: Using third-party APIs in our system can be a problem
---

# Dealing with third-party APIs

## The problem with APIs

It is very common for services to use either internal or external third-party APIs as part of their functionality.
For example, in our case, we are using the [GitHub API](https://docs.github.com/en/rest) to validate that the GitHub username provided in `Create` and `Update` is valid.

The problem with that is that third-part APIs might be unavailable for one reason or another or they might have a credit based system where they can only be called X amount of times per hour.
If we have a large test suite, we could exhaust all our credits running our tests. We don't want that.

For integration testing it is enough to run our tests and have our system call an API that responds exactly like the original API, without it being that API.
So what we need is to run something that accepts requests just like the GitHub API and responds just like the GitHub API.

To do that we will use a package called **WireMock.NET**.

## Introducing WireMock.NET

[WireMock.NET](https://github.com/WireMock-Net/WireMock.Net) is a library that allows us to create in-memory versions of the APIs our application would normally call.
These APIs can be configured to respond in specific ways when a request that matches a specific URL, path, header, cookie or body content.

Our goal here is to replace the GitHub API dependency during our integration test execution with an in-memory version the API that runs as part of our test suite and is used by our integration tests to respond in a way we want it to.

### Let's build a fake API

Time for us to use WireMock.NET to build a fake API. This is more of an introduction to WireMock and how it works rather than the exact code we will be using for integration testing.

First let's create a simple empty console application. 
You can either use your IDE's project creation functionality or the `dotnet new command` in the `src` folder:

```commandline
dotnet new console -o GitHub.Api.Fake
```

Let's clear the `Program.cs`'s contents and start by creating the WireMock server object.

```csharp
var wiremockServer = WireMockServer.Start();
```

This is all you need to create an start the fake API. Now let's get the randomly assigned port and wait for a key to be pressed before the server is disposed.

```csharp title="Program.s"
var wiremockServer = WireMockServer.Start();

Console.WriteLine($"WireMock.NET is now running on: {wiremockServer.Url}");

Console.ReadKey();
wiremockServer.Dispose();
```

And that's it! Running this app creates an API which you can call.
However at this point no endpoints are configured so calling it will return a `404 NotFound` with the following response body:

```json
{
    "Status": "No matching mapping found"
}
```

Since what we're trying to do is create a fake GitHub API that returns a GitHub user when their username is provided, let's configure this server to do the same.

My GitHub username is Elfocrash and you can get my GitHub user details by calling [https://api.github.com/users/Elfocrash](https://api.github.com/users/Elfocrash). 

The response looks like this:

```json
{
  "login": "Elfocrash",
  "id": 8199968,
  "node_id": "MDQ6VXNlcjgxOTk5Njg=",
  "avatar_url": "https://avatars.githubusercontent.com/u/8199968?v=4",
  "gravatar_id": "",
  "url": "https://api.github.com/users/Elfocrash",
  "html_url": "https://github.com/Elfocrash",
  "followers_url": "https://api.github.com/users/Elfocrash/followers",
  "following_url": "https://api.github.com/users/Elfocrash/following{/other_user}",
  "gists_url": "https://api.github.com/users/Elfocrash/gists{/gist_id}",
  "starred_url": "https://api.github.com/users/Elfocrash/starred{/owner}{/repo}",
  "subscriptions_url": "https://api.github.com/users/Elfocrash/subscriptions",
  "organizations_url": "https://api.github.com/users/Elfocrash/orgs",
  "repos_url": "https://api.github.com/users/Elfocrash/repos",
  "events_url": "https://api.github.com/users/Elfocrash/events{/privacy}",
  "received_events_url": "https://api.github.com/users/Elfocrash/received_events",
  "type": "User",
  "site_admin": false,
  "name": "Nick Chapsas",
  "company": null,
  "blog": "https://nickchapsas.com",
  "location": "London, UK",
  "email": null,
  "hireable": null,
  "bio": "I just like making stuff | Microsoft MVP",
  "twitter_username": "nickchapsas",
  "public_repos": 48,
  "public_gists": 2,
  "followers": 5752,
  "following": 0,
  "created_at": "2014-07-18T09:32:23Z",
  "updated_at": "2022-08-10T14:00:41Z"
}
```

Now let's configure WireMock to return the same response object with the same status code and content type headers when the path `/users/Elfocrash` is called.

The setup format is pretty simple. We use the `Given` method on the `wiremockServer` to define request criteria and the `RespondWith` method to define the response.

```csharp
wiremockServer.Given().RespondWith();
```

We can configure the path by adding the following in the `Given` method.

```csharp
Request.Create().WithPath("/users/Elfocrash").UsingGet()
```

And we can configure the response by adding the following in the `RespondsWith` method:

```csharp
Response.Create()
        .WithStatusCode(200)
        .WithHeader("Content-Type", "application/json; charset=utf-8")
        .WithBody(@"{
    ""login"": ""Elfocrash"",
    ""id"": 8199968,
    ""node_id"": ""MDQ6VXNlcjgxOTk5Njg="",
    ""avatar_url"": ""https://avatars.githubusercontent.com/u/8199968?v=4"",
    ""gravatar_id"": """",
    ""url"": ""https://api.github.com/users/Elfocrash"",
    ""html_url"": ""https://github.com/Elfocrash"",
    ""followers_url"": ""https://api.github.com/users/Elfocrash/followers"",
    ""following_url"": ""https://api.github.com/users/Elfocrash/following{/other_user}"",
    ""gists_url"": ""https://api.github.com/users/Elfocrash/gists{/gist_id}"",
    ""starred_url"": ""https://api.github.com/users/Elfocrash/starred{/owner}{/repo}"",
    ""subscriptions_url"": ""https://api.github.com/users/Elfocrash/subscriptions"",
    ""organizations_url"": ""https://api.github.com/users/Elfocrash/orgs"",
    ""repos_url"": ""https://api.github.com/users/Elfocrash/repos"",
    ""events_url"": ""https://api.github.com/users/Elfocrash/events{/privacy}"",
    ""received_events_url"": ""https://api.github.com/users/Elfocrash/received_events"",
    ""type"": ""User"",
    ""site_admin"": false,
    ""name"": ""Nick Chapsas"",
    ""company"": null,
    ""blog"": ""https://nickchapsas.com"",
    ""location"": ""London, UK"",
    ""email"": null,
    ""hireable"": null,
    ""bio"": ""I just like making stuff | Microsoft MVP"",
    ""twitter_username"": ""nickchapsas"",
    ""public_repos"": 48,
    ""public_gists"": 2,
    ""followers"": 5752,
    ""following"": 0,
    ""created_at"": ""2014-07-18T09:32:23Z"",
    ""updated_at"": ""2022-08-10T14:00:41Z""
}")
```

And that's it! Run and call the API at `/users/Elfocrash`.
Now the fake GitHub API responds with the same content, status code and headers that the real one does.

_Some of the headers have been omitted for the sake of brevity. You can add them as you need them._

### Creating the fake `GitHubApiServer` 

The approach we are going to follow is very similar to the approach we followed with Testcontainers.
The goal is to run the WireMock.NET instance during integration test startup in the WebApplicationFactory,
configure the web server to use WireMock.NET instead of the GitHub API and then shut it down when the tests are done.

First let's start by creating a class that will contain all the logic related to the "mock" GitHub API server that WireMock.NET will be powering.
Since this class need to be carefully disposed, we will also implement the `IDisposable` interface. 

```csharp title="GitHubApiServer.cs"
public class GitHubApiServer : IDisposable
{
    public void Dispose()
    {
        
    }
}
```

Now let's add the `WireMockServer`, a property to get the server URL, a `Start` method and implement the `Dispose` method.

```csharp title="GitHubApiServer.cs"
public class GitHubApiServer : IDisposable
{
    private WireMockServer _server;

    public string Url => _server.Url!;

    public void Start()
    {
        _server = WireMockServer.Start();
    }
    
    public void Dispose()
    {
        _server.Stop();
        _server.Dispose();
    }
}
```

Now the only thing that's left to implement is a `SetupUser` method that sets up a GitHub user by username.


```csharp
public void SetupUser(string username)
{
    _server.Given(Request.Create()
        .WithPath($"/users/{username}")
        .UsingGet())
        .RespondWith(Response.Create()
            .WithBody(GenerateGitHubUserResponseBody(username))
            .WithHeader("content-type", "application/json; charset=utf-8")
            .WithStatusCode(200));
}

private static string GenerateGitHubUserResponseBody(string username)
{
    return $@"{{
  ""login"": ""{username}"",
  ""id"": 67104228,
  ""node_id"": ""MDQ6VXNlcjY3MTA0MjI4"",
  ""avatar_url"": ""https://avatars.githubusercontent.com/u/67104228?v=4"",
  ""gravatar_id"": """",
  ""url"": ""https://api.github.com/users/{username}"",
  ""html_url"": ""https://github.com/{username}"",
  ""followers_url"": ""https://api.github.com/users/{username}/followers"",
  ""following_url"": ""https://api.github.com/users/{username}/following{{/other_user}}"",
  ""gists_url"": ""https://api.github.com/users/{username}/gists{{/gist_id}}"",
  ""starred_url"": ""https://api.github.com/users/{username}/starred{{/owner}}{{/repo}}"",
  ""subscriptions_url"": ""https://api.github.com/users/{username}/subscriptions"",
  ""organizations_url"": ""https://api.github.com/users/{username}/orgs"",
  ""repos_url"": ""https://api.github.com/users/{username}/repos"",
  ""events_url"": ""https://api.github.com/users/{username}/events{{/privacy}}"",
  ""received_events_url"": ""https://api.github.com/users/{username}/received_events"",
  ""type"": ""User"",
  ""site_admin"": false,
  ""name"": null,
  ""company"": null,
  ""blog"": """",
  ""location"": null,
  ""email"": null,
  ""hireable"": null,
  ""bio"": null,
  ""twitter_username"": null,
  ""public_repos"": 0,
  ""public_gists"": 0,
  ""followers"": 0,
  ""following"": 0,
  ""created_at"": ""2020-06-18T11:47:58Z"",
  ""updated_at"": ""2020-06-18T11:47:58Z""
}}";
}
```

And that's it! You can go even more granular and configure more things on the response body, depending on what you need, 
but since we only care about the status code in our API we will leave it here. The reason why we won't implement the 404 NotFound 
for user that are not configured is because WireMock will return 404 for those unimplemented users anyway so we don't need to.

### Replacing the real server with the fake one

We can now use the `GitHubApiServer` to start the server and use the `ConfigureTestServices` method to replace the old GitHub BaseURL with our own.

First let's great a constant username for a valid GitHub user.

```csharp
public const string ValidGithubUser = "validuser";
```

Then let's create the instance of the GitHubApiServer:

```csharp
private readonly GitHubApiServer _gitHubApiServer = new ();
```

Since there can only be one named client with the same name in ASP.NET Core, instead of removing the already registered HttpClient we can just add our own on top of it.
This action will replace the previous one.

```csharp
services.AddHttpClient("GitHub", httpClient =>
{
    httpClient.BaseAddress = new Uri(_gitHubApiServer.Url);
    httpClient.DefaultRequestHeaders.Add(
        HeaderNames.Accept, "application/vnd.github.v3+json");
    httpClient.DefaultRequestHeaders.Add(
        HeaderNames.UserAgent, $"Workshop-{Environment.MachineName}");
});
```

The only thing left to do is to update the `InitializeAsync` and `DisposeAsync` methods to include the start, setup and stop of the fake GitHub API server.

```csharp
public async Task InitializeAsync()
{
    _gitHubApiServer.Start();
    _gitHubApiServer.SetupUser(ValidGithubUser);
    await _dbContainer.StartAsync();
}

public new async Task DisposeAsync()
{
    await _dbContainer.StopAsync();
    _gitHubApiServer.Dispose();
}
```

Now let's run our test and...

![](/img/integration/failed.png)

...they failed! Which it fine because we are using GitHub username `nickchapsas` but we configured the valid user to be called `validuser`.

It's up to you to call the valid user whatever you want. I'll just update the name to `nickchapsas` and run the tests again and...

![](/img/integration/pass.png)

...voila! All tests pass without calling the real GitHub API but the one we run. Fully isolated and fully controlled integration tests.

### Exercise: Write two integration tests

Since we have the "happy path" implemented, let's implement the two unhappy paths.

1. `Create_ReturnsBadRequest_WhenGitHubUserDoesNotExist`
2. `Create_ReturnsInternalServerError_WhenGitHubIsThrottled`

In the case of `NotFound` you don't need to setup the `NotFound` user. This is the response body of the `BadRequest` when a GitHub user does not exist:

```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "traceId": "0HMKR48DE4O5U:00000004",
    "errors": {
        "Customer": [
            "There is no GitHub user with username nickchapsas"
        ]
    }
}
```

For the `InternalServerError` test, GitHub will respond with a `403 rate limit exceeded` on throttled requests with response body:

```json
{
    "message": "API rate limit exceeded for <your-ip-address>. (But here's the good news: Authenticated requests get a higher rate limit. Check out the documentation for more details.)",
    "documentation_url": "https://docs.github.com/rest/overview/resources-in-the-rest-api#rate-limiting"
}
```

#### Solutions

Only expand the solutions after you've tried to solve the problem yourself. Practice makes perfect and you only learn by doing.

<details>
<summary>Create_ReturnsBadRequest_WhenGitHubUserDoesNotExist</summary>

```csharp
[Fact]
public async Task Create_ReturnsBadRequest_WhenGitHubUserDoesNotExist()
{
    // Arrange
    var request = new CustomerRequest
    {
        Email = "nick@chapsas.com",
        FullName = "Nick Chapsas",
        DateOfBirth = new DateTime(1993, 01, 01),
        GitHubUsername = "missing"
    };

    // Act
    var response = await _client.PostAsJsonAsync("customers", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

    var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
    problemDetails!.Errors["Customer"].Should().Equal("There is no GitHub user with username missing");
}
```

</details>

<details>
<summary>Create_ReturnsInternalServerError_WhenGitHubIsThrottled</summary>

```csharp
[Fact]
public async Task Create_ReturnsInternalServerError_WhenGitHubIsThrottled()
{
    // Arrange
    var request = new CustomerRequest
    {
        Email = "nick@chapsas.com",
        FullName = "Nick Chapsas",
        DateOfBirth = new DateTime(1993, 01, 01),
        GitHubUsername = "throttled"
    };

    // Act
    var response = await _client.PostAsJsonAsync("customers", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
}
```

</details>

### Quick refactoring

Refactoring our tests and structure as we go is part of the development process. 
The main thing I am not a fan of is that the `CustomerApiFactory` class is responsible for setting up "good" and `throttled` GitHub users.
This should not be its responsibility. It should be the test class or the test itself that sets up the user it wants to call.

Let's refactor!

First we will expose the `GitHubApiServer` through a property with a public getter. This means that we will delete the old field.

```csharp
public GitHubApiServer GitHubApiServer { get; } = new();
```

We can also move the two constants for the valid and throttled GitHub users to the `CustomerControllerTests` class directly.

This means we can now use the GitHubApiServer in our CustomerControllerTests class and Setup the users in the constructor.

```csharp title="CustomerControllerTests.cs"
public CustomerControllerTests(CustomerApiFactory customerApiFactory)
{
    _client = customerApiFactory.CreateClient(new WebApplicationFactoryClientOptions
    {
        BaseAddress = new Uri("https://localhost:5001")
    });
    
    customerApiFactory.GitHubApiServer.SetupUser(ValidGithubUser);
    customerApiFactory.GitHubApiServer.SetupThrottledUser(ThrottledUser);
}
```

And that's it. All our tests should now still pass but things live in more appropriate places.
