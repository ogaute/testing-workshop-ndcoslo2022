---
description: Let's take a look at what we can keep from the API approach
---

# The same, but different

The basic principles of what we are going to be doing stay the same. We need to run the Web App, replace the database with the Docker specific one and replace the GitHub API with the WireMock one.

Since the WebApplicationFactory was creating an in-memory version of our application and we need to test the app through the UI, using WAF will be impossible.
We will still run a database in Docker that our application will use during testing but Docker will also run our application.

Let's take a look at what we can keep from the API approach.

## What stays the same

We will still use the exact same `GitHubApiServer.cs` to run the fake GitHub API using WireMock.

However, this is where the similarities end. We will no longer be using the `WebApplicationFactory` class and we won't be using Testcontainers to run our docker containers.

## What will be different

There will be two main differences in terms of test execution. 

First difference will be how we approach running the Web App that will be tested.
Instead of using the WebApplicationFactory class, we will instead create a `docker-compose.yml` file dedicated to our integration tests.
During test execution, we will run this file to create the database and run the Web App and then we will run our tests against that.

The second and biggest difference is how we will invoke the actions for our Web App. 
Since we won't be making HttpClient API calls, we will need to use a technique that runs our Web App's UI and takes actions to ensure that the appropriate actions have the correct results.
