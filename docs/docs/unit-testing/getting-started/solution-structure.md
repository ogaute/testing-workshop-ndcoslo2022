---
description: Structuring our solution and our projects
---

# Structuring our solution

## Project naming

You might remember from earlier that we named our test project `AmazingCalculator.Tests.Unit`. This was very intentional.

Instead of giving an arbitrary name to your test project, we want to give it a meaningful one. Here is why we chose that name:

- The first part (`AmazingCalculator`) tells us which project this test project is targeting. If the name was  `AmazingCalculator.Api` then that's how our test project's name would start.
- The second part (`Tests`) tells us that this is a test project
- The last part (`Unit`) tells us that this project contains unit tests. That name could be `Integration` or `Acceptance` depending on what types of tests the project contains.

So just by the project name we know that this is a test project that contains unit tests for the project named `AmazingCalculator`.

## Solution structure

As your solution becomes bigger, having main projects and test projects in the same directory can become really cluttered and distracting.

An elegant solution is to create two top-level folders

- `src` - Containing all your main codebase projects
- `tests` - Containing all your test projects

The process of creating this structure should be as follows:

1. Create a top level folder named `src`
2. Create a top level folder named `tests`
3. In your IDE, create a solution folder named `src`
4. In your IDE, create a solution folder named `tests`
5. Create (or move) your projects in each appropriate folder

:::tip

Creating the folders beforehand is very important. If you don't do that, the solution will just assume those folders are virtual and won't take into account the real file-system folders.

:::
