---
description: Practice makes perfect. Let's give TDD a go
---

# Let's practice TDD

Let's write some code using TDD. We will do that using a _Code kata_. Katas are exercises that allow us to practice coding practices to get better at applying them and using them.

The one we will use in this case is the **String Calculator** Kata.

:::tip
Each section includes the answer to the exercise but only reveal if you are stuck. Practice makes perfect so give it a go.
:::

## The String Calculator Kata

### 1. Create a simple String calculator with the following method signature

```csharp
int Add(string numbers)
```

The method can take up to two numbers, separated by commas, and will return their sum.
For example `<empty-string>` or `1` or `1,2` as inputs. (For an empty string it should return 0)

---

### 2. Allow the `Add` method to handle an unknown amount of numbers

---

### 3. Allow the `Add` method to handle new lines between numbers (on top of commas)

- The following input is ok: `1\n2,3` (will equal 6)
- The following input is NOT ok: `1,\n` (no need to prove it - just clarifying)

---

### 4. Support different delimiters

To change the default delimiter, the beginning of the string will contain a separate like that looks like this: `//[delimiter]\n[numbers…]`.

For example `//;\n1;2` should return `3` where the default delimiter is `;`.
The first line is optional and its absence should still default to `,`. All existing scenarios should still be supported.

---

### 5. Calling `Add` with a negative number will throw a `NegativesNotAllowedException` with the negative number(s) in the message.

---

### 6. Numbers bigger than 1000 should be ignored, so adding 2 + 1001 = 2

---

### 7. Delimiters can be of any length with the following format: `//[delimiter]\n` for example `//[***]\n1***2***3` should return 6

---
 
### 8. Allow multiple delimiters like this: `//[delim1][delim2]\n` for example `//[*][%]\n1*2%3` should return 6.

---

### 9. Make sure you can also handle multiple delimiters with length longer than one char
