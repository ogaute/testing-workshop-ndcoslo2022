---
description: Let's setup our test suite for UI testing
---

# Running the app for testing

## The `docker-compose.integration.yml` file

Before we even take a look at the integration tests themselves, we need to take a look at what will drive all those tests in the first place, 
and that is the `docker-compose.integration.yml` file. This file will be used by our tests to run the database and the web app so we can run integration tests against it.

For the Web App we will use an image built for the tests using the Dockerfile that can be found in `Customers.WebApp`. 
It is mostly stock, except it has extra configuration so we can run the API with an SSL certificate.

```dockerfile title="Dockerfile"
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
# Run this to generate it: dotnet dev-certs https -ep cert.pfx -p Test1234!
COPY ["cert.pfx", "/https/cert.pfx"]
ENTRYPOINT ["dotnet", "Customers.WebApp.dll"]
```

With that in place we can start creating the skeleton of the `docker-compose.integration.yml` file.

First let's add the Postgres database that we will be using for the tests:

```yaml title="docker-compose.integration.yml"
version: '3.9'

services:

  test-db:
    image: postgres:latest
    restart: always
    environment:
      - POSTGRES_USER=workshop
      - POSTGRES_PASSWORD=changeme
      - POSTGRES_DB=mydb
    healthcheck:
      test: [ "CMD-SHELL", "pg_isready" ]
      interval: 2s
      timeout: 5s
      retries: 10
    ports:
      - '5435:5432'
```

This looks largely the same as our regular `docker-compose.yml` with the only difference being that we have a `healthcheck` in place.
This health check will run after the docker container has started running and will run the `pg_isready` command ever 2 second for a total of 10 times to ensure that the database is ready to be used before we proceed.
This is done so we don't start the web app against a database that can't handle it, causing the app to fail on startup.

Now we can add the Web App service and configure it to use that healthcheck to ensure that the app container won't start unless the database is ready. 

```yml
test-app:
    build: ../../src/Customers.WebApp
    ports:
      - "7780:443"
      - "7779:80"
    environment:
      - ASPNETCORE_URLS=https://+:443;http://+:80
      - ASPNETCORE_Kestrel__Certificates__Default__Password=Test1234!
      - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/cert.pfx
      - ASPNETCORE_Environment=Production
    depends_on: 
      test-db:
        condition: service_healthy
```

As you can see, the URLs, the certificate details and the environment that we want the app to run with as passed as environment variables.
ASP.NET Core accepts them because `ASPNETCORE_` is a known prefix that we can use to pass this configuration.

But wait a second. How will we configure the new GitHub API URL and the new database connection string if we can't change the code of the Web App.
It's actually pretty simple. We can leverage the same "Environment Variable as config" mechanism of ASP.NET Core to pass our own variables.

We can do that by using the `Configuration.AddEnvironmentVariables` method and use as a parameter the prefix of our choice.

```csharp title="Program.cs"
builder.Configuration.AddEnvironmentVariables("CustomersWebApp_");
```

This will ensure that only environment variables that start with `CustomersWebApp_` will be loaded.

In order for ASP.NET Core to acknowledge them and treat them as application configuration we need to follow the standard underscore-based nesting structure of ASP.NET Core.

For example in order to configure this item:

```json
{
  "Database": {
    "ConnectionString": "Server=localhost;Port=5432;Database=mydb;User ID=workshop;Password=changeme;"
  }
}
```

We need to pass an environment variable named `Database__ConnectionString`. Note that those are 2 underscores, not one.
To make this work with our Dockerfile and environment loading the final name needs to be `CustomersWebApp_Database__ConnectionString`.

With that in mind we can add the following 2 lines in the `docker-compose.integration.yml` file to configure the test database and the fake GitHub API:

```yaml
- CustomersWebApp_Database__ConnectionString=Server=test-db;Port=5432;Database=mydb;User ID=workshop;Password=changeme;
- CustomersWebApp_GitHub__ApiBaseUrl=http://host.docker.internal:9850
```

This makes the final `docker-compose.integration.yml` as follows: 

```yaml
version: '3.9'

services:

  test-app:
    build: ../../src/Customers.WebApp
    ports:
      - "7780:443"
      - "7779:80"
    environment:
      - ASPNETCORE_URLS=https://+:443;http://+:80
      - ASPNETCORE_Kestrel__Certificates__Default__Password=Test1234!
      - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/cert.pfx
      - ASPNETCORE_Environment=Production
      - CustomersWebApp_Database__ConnectionString=Server=test-db;Port=5432;Database=mydb;User ID=workshop;Password=changeme;
      - CustomersWebApp_GitHub__ApiBaseUrl=http://host.docker.internal:9850
    depends_on: 
      test-db:
        condition: service_healthy

  test-db:
    image: postgres:latest
    restart: always
    environment:
      - POSTGRES_USER=workshop
      - POSTGRES_PASSWORD=changeme
      - POSTGRES_DB=mydb
    healthcheck:
      test: [ "CMD-SHELL", "pg_isready" ]
      interval: 2s
      timeout: 5s
      retries: 10
    ports:
      - '5435:5432'
```
