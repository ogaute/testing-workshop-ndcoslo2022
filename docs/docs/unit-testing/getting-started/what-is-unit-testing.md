---
description: Let's understand what is unit testing
---

# What is unit testing?

Unit testing is an automated testing technique that focuses on validating the behavior of the smallest piece of code that can be logically isolated in a system. 
This piece of code can be anything from a class, to a method, to a property or even a single line of code.
One of the core ideas of a unit test is that you assume your external systems behave the way you expect them to behave.
For example, if your code talks to a database, or makes any network call, internal or external, or even reads something from the file system, 
then you need to isolate that part from your code while unit testing it and assume that this part will behave in a deterministic reliable way. 
This is where substitutes such as stubs, mocks and fakes come into the picture. In general, well unit tested code is regarded as good quality code because in order to have code that is unit testable you need to have in place some good software principles.

## But why unit test?

The idea is simple. Due to the scope of the test and the isolated aspect, our tests try to prove that individual parts of our system are correct.
If we have enough unit tests in place for individual parts then the sum of those parts create a well tested system.
It is effectively a self documented contract between what you want our code to do an what it actually does.

## The Test Pyramid

The test is a concept that helps us visualise the different layers of testing we can have in a system.
There are different variations of the pyramid but the main idea is that the lower you are in the pyramid, 
the more isolation you have in your test, the faster the tests will execute and the more of those types of tests you have.
As you go higher you start having lower running tests with more system integration. 

Another metric that some choose to include is that the lower you are in the pyramid the cheaper the tests to write and maintain and as you go higher, the tests also get more expensive.

Here is an example of the test pyramid:

![](/img/unit/pyramid-1.png)

_Keep in mind that the test pyramid applies only on automated tests._
