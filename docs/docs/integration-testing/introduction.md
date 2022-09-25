---
description: Let's understand what is Integration testing
---

# What is Integration Testing?

Integration testing is a software testing technique in which different units, modules or components are combined and tested as a group.
Compared to unit testing, there are two main differences.

- The scope
- How we deal with external dependencies

The scope of integration testing is wider than unit testing, but it's still smaller than system tests or end-to-end tests. 
When it comes to dependencies, we don't mock or assume that this part of the system behaves correctly. 
We still integrate with it, but we usually choose to do so in a controlled and deterministic manner. For example,
integration tests that need to call a database will call a database that is running locally with a specific amount of data in it.
When it needs to call an API, it will call that API or an API that is configured to respond like the original API.

## Types of integration testing

There are multiple types of integration testing depending on how we approach the writing of the test.
Those types are:

- **Bottom-up**: Lower modules are tested first. Then we go higher and higher until we test all the way until the surface. The surface is usually UI layer.
- **Top-down**: Higher modules are tested first. Then we go lower and lower until we test all the way to the bottom. The bottom is usually a database or some data store.
- **Sandwich**: A hybrid approach of the two above. Tests start from one level higher and one level lower and they meet in the middle.
- **Big bang**: All modules are tested simultaneously.

After seeing all approaches in practice over the years, I can confidently say that we should only focus on writing top-down integration tests.

### Why we will focus in Top-down tests

Top-down tests are great because they are testing in a way that our user also experience our software. 
This means that we can write integration tests in the way that we'd expect the user to interact with the system and simulate their experience.

### Overlap with unit tests

Your integration tests can also overlap your unit tests. That is expected and totally fine.

If we take a look at the testing pyramid diagram again:

![](/img/unit/pyramid-1.png)

As we can see by the volume that the integration tests layer occupies, the amount of integration tests that we'll have is less than the amount of unit tests.
This is because, in general, unit tests will test more edge cases in an isolated manner and let integration tests cover the more general happy and unhappy paths.

Some people choose to only of in for integration tests because they thing that integration tests give more value.
These people are not technically wrong and this approach can totally work but let's remember that integration tests are not only 
more expensive to write and maintain but also slower to run since they need to talk to databases and other external systems.
That's why we usually choose a middle ground between all the different types of testing. In theory we could write one end-to-end tests 
and be fully covered, but running thousands of end to end tests might take hours to days in big projects and that's just not feasible.
