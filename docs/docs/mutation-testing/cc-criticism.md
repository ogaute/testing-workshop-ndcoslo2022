---
description: Code coverage doesn't tell the whole story
---

# Code coverage criticism

## The problem

As we mentioned already, measures the number of lines of source code executed during a given test suite for a program.
This can get more specific and give us insights for specific methods and branches but in general it focuses on lines of code.

It is generally accepted that a project with a high percentage of code coverage is a "well tested" project. This however is **NOT** the case.

Consider the following. I write a unit test that covers a method completely but I never assert the result. 
The test will pass and I will get the lines marked as "covered" but it's meaningless. 
This is also true when a method is tested with one set of parameters, but it's lacking another set which could make it fail.

All that doesn't mean that code coverage isn't useful. It just means that it should only be _one_ of the metrics we should use to ensure that we wrote well tested code.

So what is the solution? Well let's take a look at **Mutation Testing**.
