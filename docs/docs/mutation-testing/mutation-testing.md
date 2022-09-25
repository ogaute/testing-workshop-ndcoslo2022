---
description: Why changing our code to test it is a good idea?
---

# What is mutation testing?

## Definition

Wikipedia (surprisingly) has a great definition for Mutation Testing.

> Mutation testing (or mutation analysis or program mutation) is used to design new software tests and evaluate the quality of existing software tests. 
> Mutation testing involves modifying a program in small ways. 
> Each mutated version is called a mutant and tests detect and reject mutants by causing the behavior of the original version to differ from the mutant. 
> This is called killing the mutant. Test suites are measured by the percentage of mutants that they kill. New tests can be designed to kill additional mutants. 
> Mutants are based on well-defined mutation operators that either mimic typical programming errors (such as using the wrong operator or variable name) or force the creation of valuable tests (such as dividing each expression by zero). 
> The purpose is to help the tester develop effective tests or locate weaknesses in the test data used for the program or in sections of the code that are seldom or never accessed during execution.

## In practice

The great thing about mutation testing is that you don't need to change anything in your existing test code.
All you need to do is install Stryker.NET as a global (or project specific) tool and run it on the terminal.
The tool will take your project code and your tests and create the appropriate mutants in order to see how many can be killed and how many will survive.
After that it is up to you to add more test cases and improve your tests.

## Types of changes though?

The Stryker.NET supports an extensive number of mutations. Expand the collapsed segment below to see the full list.

<details>

<summary>All mutations</summary>

### Arithmetic Operators (_arithmetic_)
| Original | Mutated | 
|----------|---------|
| `+`      | `-`     |
| `-`      | `+`     |
| `*`      | `/`     |
| `/`      | `*`     |
| `%`      | `*`     |

### Equality Operators (_equality_)
| Original | Mutated | 
|----------|---------|
| `>`      | `<`     |
| `>`      | `>=`    |
| `>=`     | `<`     |
| `>=`     | `>`     |
| `<`      | `>`     |
| `<`      | `<=`    |
| `<=`     | `>`     |
| `<=`     | `<`     |
| `==`     | `!=`    |
| `!=`     | `==`    |

### Logical Operators (_logical_)
| Original                  | Mutated                   |
|---------------------------|---------------------------|
| `&&`                      | <code>&#124;&#124;</code> |
| <code>&#124;&#124;</code> | `&&`                      |
| `^`                       | `==`                      |

### Boolean Literals (_boolean_)
| Original                  | Mutated                     | 
|---------------------------|-----------------------------|
| `true`                    | `false`                     |
| `false`                   | `true`                      |
| `!person.IsAdult()`       | `person.IsAdult()`          |
| `if(person.IsAdult())`    | `if(!person.IsAdult())`     |
| `while(person.IsAdult())` | `while(!person.IsAdult())`  |

### Assignment Statements (_assignment_)
| Original             | Mutated              |
|----------------------|----------------------|
| `+=`                 | `-=`                 |
| `-=`                 | `+=`                 |
| `*=`                 | `/=`                 |
| `/=`                 | `*=`                 |
| `%=`                 | `*=`                 |
| `<<=`                | `>>=`                |
| `>>=`                | `<<=`                |
| `&=`                 | <code>&#124;=</code> |
| `&=`                 | `^=`                 |
| <code>&#124;=</code> | `&=`                 |
| <code>&#124;=</code> | `^=`                 |
| `^=`                 | <code>&#124;=</code> |
| `^=`                 | `&=`                 |

### Collection initialization (_initializer_)
| Original                                 | Mutated                         | 
|------------------------------------------|---------------------------------|
| `new int[] { 1, 2 };`                    | `new int[] { };`                |
| `int[] numbers = { 1, 2 };`              | `int[] numbers = { };`          |
| `new List<int> { 1, 2 };`                | `new List<int> { };`            |
| `new Collection<int> { 1, 2 };`          | `new Collection<int> { };`      |
| `new Dictionary<int, int> { { 1, 1 } };` | `new Dictionary<int, int> { };` |

### Removal mutators (_statement_, _block_)
| Original                                | Mutated                                               | 
|-----------------------------------------|-------------------------------------------------------|
| `void Function() { Age++; }`            | `void Function() {} (block emptied)`                  |
| `int Function() { Age++; return Age; }` | `void Function() { return default; } (block emptied)` |
| `return;`                               | `removed`                                             |
| `return value;`                         | `removed`                                             |
| `break;`                                | `removed`                                             |
| `continue;`                             | `removed`                                             |
| `goto;`                                 | `removed`                                             |
| `throw;`                                | `removed`                                             |
| `throw exception;`                      | `removed`                                             |
| `yield return value;`                   | `removed`                                             |
| `yield break;`                          | `removed`                                             |
| `MyMethodCall();`                       | `removed`                                             |

### Unary Operators (_unary_)
| Original    | Mutated     |
|-------------|-------------|
| `-variable` | `+variable` |
| `+variable` | `-variable` |
| `~variable` | `variable`  |

### Update Operators (_update_)
| Original      | Mutated      | 
|---------------|--------------|
| `variable++`	 | `variable--` |
| `variable--`	 | `variable++` |
| `++variable`	 | `--variable` |
| `--variable`	 | `++variable` |

### Checked Statements (_checked_)
| Original         | Mutated |
|------------------|---------|
| `checked(2 + 4)` | `2 + 4` |

## Linq Methods (_linq_)
| Original              | Mutated               |
|-----------------------|-----------------------|
| `SingleOrDefault()`   | `Single()`            |
| `Single()`            | `SingleOrDefault()`   |
| `FirstOrDefault()`    | `First()`             |
| `First()`             | `FirstOrDefault()`    |
| `Last()`              | `First()`             |
| `All()`               | `Any()`               |
| `Any()`               | `All()`               |
| `Skip()`              | `Take()`              |
| `Take()`              | `Skip()`              |
| `SkipWhile()`         | `TakeWhile()`         |
| `TakeWhile()`         | `SkipWhile()`         |
| `Min()`               | `Max()`               |
| `Max()`               | `Min()`               |
| `Sum()`               | `Max()`               |
| `Count()`             | `Sum()`               |
| `Average()`           | `Min()`               |
| `OrderBy()`           | `OrderByDescending()` |
| `OrderByDescending()` | `OrderBy()`           |
| `ThenBy()`            | `ThenByDescending()`  |
| `ThenByDescending()`  | `ThenBy()`            |
| `Reverse()`           | `AsEnumerable()`      |
| `AsEnumerable()`      | `Reverse()`           |
| `Union()`             | `Intersect()`         |
| `Intersect()`         | `Union()`             |
| `Concat()`            | `Except()`            |
| `Except()`            | `Concat()`            |

### String Literals and Constants (_string_)
| Original       | Mutated               |
|----------------|-----------------------|
| `"foo"`        | `""`                  |
| `""`           | `"Stryker was here!"` |
| `$"foo {bar}"` | `$""`                 |
| `@"foo"`       | `@""`                 |
| `string.Empty` | `"Stryker was here!"` |

### Bitwise Operators (_bitwise_)
| Original            | Mutated             |
|---------------------|---------------------|
| `<<`                | `>>`                |
| `>>`                | `<<`                |
| `&`                 | <code>&#124;</code> |
| <code>&#124;</code> | `&`                 |
| `a^b`               | `~(a^b)`            |


</details>

