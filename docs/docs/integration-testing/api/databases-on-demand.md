---
description: Using Docker to create databases when we need them
---

# Creating databases on demand

One of the biggest, if not the biggest, problems we need to deal with when we are running integration tests is having a database in place for the tests to run against.
This datastore needs to be controlled for those tests to run and satisfy specific criteria such as containing specific data for the tests to run.

Managing and maintaining a database that runs at all times just for the integration tests to run against, is not a scalable option.
Wouldn't it be better if we could create an isolated database only when the tests run and then shut it down once the tests pass or fail?

Well what a coincidence! That's exactly what we'll learn how to do.

## Introducing Docker

Docker is a containerization technology that allows us to run services in a controlled and self-contained fashion.
We can run anything with it, from web services, to reverse proxies, to databases. In fact this is how I've been running the Postgres database that I've been using until now.
Docker containers are lightweight so we can create and destroy them on demand very easily. 
Such a concept lends itself really nicely for our integration testing usecase.

## Running Docker Containers with C#

The first thing we need to do is to find a way to control Docker with C#. 
There are a couple of main ways and over the course of this workshop we will see both. We will start however with the simplest one for our usecase.

We will use a Nuget package called `Testcontainers`.

```commandline
dotnet add package Testcontainers
```

We can now use the `TestcontainersContainer` class in our code to define the container that we need.
Since the `CustomerApiFactory` is responsible for managing the lifetime of our tests, we will add the container creation and shutdown code there.

### Preparing the WAF

First we need to make the `CustomerApiFactory` implement the `IAsyncLifetime`. 
This will allow us to create the test containers on test startup for all the tests using the `InitializeAsync` method and shut it down with the `DisposeAsync` method.

```csharp title="CustomerApiFactory.cs"
public class CustomerApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
```

```csharp title="CustomerApiFactory.cs"
public Task InitializeAsync() => Task.CompletedTask;

public new Task DisposeAsync() => Task.CompletedTask;
```

### Defining the Testcontainer

As a reminder, this is how we have defined our database in the docker-compose.yml file that we used to run it.

```yaml
image: postgres:latest
restart: always
environment:
  - POSTGRES_USER=workshop
  - POSTGRES_PASSWORD=changeme
  - POSTGRES_DB=mydb
ports:
  - '5432:5432'
```

To define the same thing using Testcontainers we need to created a `TestcontainersContainer` object and use the `TestcontainersBuilder<TestcontainersContainer>()`'s fluent extensions to define those items.

For example to define the `image: postgres:latest` we need to call the `WithImage` method.

```csharp
private readonly TestcontainersContainer _dbContainer =
    new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("postgres:latest")
        .WithEnvironment("POSTGRES_USER", "workshop")
        .WithEnvironment("POSTGRES_PASSWORD", "changeme")
        .WithEnvironment("POSTGRES_DB", "mydb")
        .WithPortBinding(5555, 5432)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
        .Build();
```

The `WithWaitStrategy` ensures that the port is ready before the tests continue. This makes sure the tests won't run against a database container that hasn't started yet.

Now to start the container we need to call the `StartAsync` method of the object in the `InitializeAsync` method and the `StopAsync` method in the `DisposeAsync` one. 

```csharp
public async Task InitializeAsync() => await _dbContainer.StartAsync();

public new async Task DisposeAsync() => await _dbContainer.StopAsync();
```

But wait a second. How can we configure our API to use the new connection to this ephemeral database?

### Re-wiring the database connection

Re-configuring the web application is extremely easy due to the `ConfigureWebHost` that we have in the custom WebApplicationFactory.

We can simply call the `builder.ConfigureTestServices` method and manipulate the service container in any way that we want for our testing.
In our case we want to remove the existing `IDbConnectionFactory` and add one that points to our Docker database.

We can do that with a couple of line of code:

```csharp
builder.ConfigureTestServices(services =>
{
    services.RemoveAll(typeof(IDbConnectionFactory));
    services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory("Server=localhost;Port=5555;Database=mydb;User ID=workshop;Password=changeme;"));
});
```

Now if we run our tests you can see that all of them pass, without having a database running beforehand and the database created for the tests is deleted immediately after the tests run.

![](/img/integration/tests-pass.png)

![](/img/integration/test-specific-db.png)

And that's it! We now create a database on demand just for out tests and we tear it down after we are done.

### Using Database-specific Test containers

Testcontainers can help us run containers very easily as we've already seen but they also have specific code for very common container images such as database.
These pre-defined containers can streamline our code a lot.

For example in our previous example, we hardcoded the port `5555` both in the container definition and the connection string.
However, we don't know for a fact that this port will always be free on our machine. We could write code that deals with that but we don't need to.

We can now change our `TestcontainersContainer` to be of type `TestcontainerDatabase` and use a builder of type `TestcontainersBuilder<PostgreSqlTestcontainer>()`.
This allows us to use the `WithDatabase` extension method which accepts a `PostgreSqlTestcontainerConfiguration` object.

Now all the code we need to define the Postgres database looks like this:

```csharp
private readonly TestcontainerDatabase _dbContainer =
    new TestcontainersBuilder<PostgreSqlTestcontainer>()
        .WithDatabase(new PostgreSqlTestcontainerConfiguration
        {
            Database = "mydb",
            Username = "workshop",
            Password = "changeme"
        }).Build();
```

And what's even better, we no longer need to hardcode and connection strings or ports. Testcontainers will manage all that for us.

```csharp
builder.ConfigureTestServices(services =>
{
    services.RemoveAll(typeof(IDbConnectionFactory));
    services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(_dbContainer.ConnectionString));
});
```

Now simply run the tests and watch them pass without you needing to have a database in place.
