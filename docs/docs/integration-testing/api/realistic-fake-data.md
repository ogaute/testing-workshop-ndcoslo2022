---
description: Creating realistic fake data for our tests
---

# Realistic fake data

When writing tests, especially integration tests, being able to create fake data at scale is a key requirement. 
Testing for 5-6 use-cases might be fine, but what happens when you have 20,50, 200 or 500 tests?

You could keep using the same data but this can get tedious. It can also mask flaws in your code.

This is where [Bogus](https://github.com/bchavez/Bogus) comes in. Bogus is a simple fake data generator.
It uses a fluent API and it has data generators for tons of different data types and domains. 

<details>
<summary>Expand to see the full list of things Bogus supports</summary>

### Bogus API Support
* **`Address`**
    * `ZipCode` - Get a zipcode.
    * `City` - Get a city name.
    * `StreetAddress` - Get a street address.
    * `CityPrefix` - Get a city prefix.
    * `CitySuffix` - Get a city suffix.
    * `StreetName` - Get a street name.
    * `BuildingNumber` - Get a building number.
    * `StreetSuffix` - Get a street suffix.
    * `SecondaryAddress` - Get a secondary address like 'Apt. 2' or 'Suite 321'.
    * `County` - Get a county.
    * `Country` - Get a country.
    * `FullAddress` - Get a full address like Street, City, Country.
    * `CountryCode` - Get a random ISO 3166-1 country code.
    * `State` - Get a random state state.
    * `StateAbbr` - Get a state abbreviation.
    * `Latitude` - Get a Latitude.
    * `Longitude` - Get a Longitude.
    * `Direction` - Generates a cardinal or ordinal direction. IE: Northwest, South, SW, E.
    * `CardinalDirection` - Generates a cardinal direction. IE: North, South, E, W.
    * `OrdinalDirection` - Generates an ordinal direction. IE: Northwest, Southeast, SW, NE.
* **`Commerce`**
    * `Department` - Get a random commerce department.
    * `Price` - Get a random product price.
    * `Categories` - Get random product categories.
    * `ProductName` - Get a random product name.
    * `Color` - Get a random color.
    * `Product` - Get a random product.
    * `ProductAdjective` - Random product adjective.
    * `ProductMaterial` - Random product material.
    * `Ean8` - Get a random EAN-8 barcode number.
    * `Ean13` - Get a random EAN-13 barcode number.
* **`Company`**
    * `CompanySuffix` - Get a company suffix. "Inc" and "LLC" etc.
    * `CompanyName` - Get a company name.
    * `CatchPhrase` - Get a company catch phrase.
    * `Bs` - Get a company BS phrase.
* **`Database`**
    * `Column` - Generates a column name.
    * `Type` - Generates a column type.
    * `Collation` - Generates a collation.
    * `Engine` - Generates a storage engine.
* **`Date`**
    * `Past` - Get a `DateTime` in the past between `refDate` and `yearsToGoBack`.
    * `PastOffset` - Get a `DateTimeOffset` in the past between `refDate` and `yearsToGoBack`.
    * `Soon` - Get a `DateTime` that will happen soon.
    * `SoonOffset` - Get a `DateTimeOffset` that will happen soon.
    * `Future` - Get a `DateTime` in the future between `refDate` and `yearsToGoForward`.
    * `FutureOffset` - Get a `DateTimeOffset` in the future between `refDate` and `yearsToGoForward`.
    * `Between` - Get a random `DateTime` between `start` and `end`.
    * `BetweenOffset` - Get a random `DateTimeOffset` between `start` and `end`.
    * `Recent` - Get a random `DateTime` within the last few days.
    * `RecentOffset` - Get a random `DateTimeOffset` within the last few days.
    * `Timespan` - Get a random `TimeSpan`.
    * `Month` - Get a random month.
    * `Weekday` - Get a random weekday.
* **`Finance`**
    * `Account` - Get an account number. Default length is 8 digits.
    * `AccountName` - Get an account name. Like "savings", "checking", "Home Loan" etc..
    * `Amount` - Get a random amount. Default 0 - 1000.
    * `TransactionType` - Get a transaction type: "deposit", "withdrawal", "payment", or "invoice".
    * `Currency` - Get a random currency.
    * `CreditCardNumber` - Generate a random credit card number with valid Luhn checksum.
    * `CreditCardCvv` - Generate a credit card CVV.
    * `BitcoinAddress` - Generates a random Bitcoin address.
    * `EthereumAddress` - Generate a random Ethereum address.
    * `RoutingNumber` - Generates an ABA routing number with valid check digit.
    * `Bic` - Generates Bank Identifier Code (BIC) code.
    * `Iban` - Generates an International Bank Account Number (IBAN).
* **`Hacker`**
    * `Abbreviation` - Returns an abbreviation.
    * `Adjective` - Returns a adjective.
    * `Noun` - Returns a noun.
    * `Verb` - Returns a verb.
    * `IngVerb` - Returns a verb ending with -ing.
    * `Phrase` - Returns a phrase.
* **`Images`**
    * `DataUri` - Get a SVG data URI image with a specific width and height.
    * `PicsumUrl` - Get an image from the https://picsum.photos service.
    * `PlaceholderUrl` - Get an image from https://placeholder.com service.
    * `LoremFlickrUrl` - Get an image from https://loremflickr.com service.
    * `LoremPixelUrl` - Creates an image URL with http://lorempixel.com. Note: This service is slow. Consider using PicsumUrl() as a faster alternative.
        * `Abstract` - Gets an abstract looking image.
        * `Animals` - Gets an image of an animal.
        * `Business` - Gets a business looking image.
        * `Cats` - Gets a picture of a cat.
        * `City` - Gets a city looking image.
        * `Food` - Gets an image of food.
        * `Nightlife` - Gets an image with city looking nightlife.
        * `Fashion` - Gets an image in the fashion category.
        * `People` - Gets an image of humans.
        * `Nature` - Gets an image of nature.
        * `Sports` - Gets an image related to sports.
        * `Technics` - Get a technology related image.
        * `Transport` - Get a transportation related image.
* **`Internet`**
    * `Avatar` - Generates a legit Internet URL avatar from twitter accounts.
    * `Email` - Generates an email address.
    * `ExampleEmail` - Generates an example email with @example.com.
    * `UserName` - Generates user names.
    * `UserNameUnicode` - Generates a user name preserving Unicode characters.
    * `DomainName` - Generates a random domain name.
    * `DomainWord` - Generates a domain word used for domain names.
    * `DomainSuffix` - Generates a domain name suffix like .com, .net, .org
    * `Ip` - Gets a random IPv4 address string.
    * `Port` - Generates a random port number.
    * `IpAddress` - Gets a random IPv4 IPAddress type.
    * `IpEndPoint` - Gets a random IPv4 IPEndPoint.
    * `Ipv6` - Generates a random IPv6 address string.
    * `Ipv6Address` - Generate a random IPv6 IPAddress type.
    * `Ipv6EndPoint` - Gets a random IPv6 IPEndPoint.
    * `UserAgent` - Generates a random user agent.
    * `Mac` - Gets a random mac address.
    * `Password` - Generates a random password.
    * `Color` - Gets a random aesthetically pleasing color near the base RGB. See [here](http://stackoverflow.com/questions/43044/algorithm-to-randomly-generate-an-aesthetically-pleasing-color-palette).
    * `Protocol` - Returns a random protocol. HTTP or HTTPS.
    * `Url` - Generates a random URL.
    * `UrlWithPath` - Get an absolute URL with random path.
    * `UrlRootedPath` - Get a rooted URL path like: /foo/bar. Optionally with file extension.
* **`Lorem`**
    * `Word` - Get a random lorem word.
    * `Words` - Get an array of random lorem words.
    * `Letter` - Get a character letter.
    * `Sentence` - Get a random sentence of specific number of words.
    * `Sentences` - Get some sentences.
    * `Paragraph` - Get a paragraph.
    * `Paragraphs` - Get a specified number of paragraphs.
    * `Text` - Get random text on a random lorem methods.
    * `Lines` - Get lines of lorem.
    * `Slug` - Slugify lorem words.
* **`Name`**
    * `FirstName` - Get a first name. Getting a gender specific name is only supported on locales that support it.
    * `LastName` - Get a last name. Getting a gender specific name is only supported on locales that support it.
    * `FullName` - Get a full name, concatenation of calling FirstName and LastName.
    * `Prefix` - Gets a random prefix for a name.
    * `Suffix` - Gets a random suffix for a name.
    * `FindName` - Gets a full name.
    * `JobTitle` - Gets a random job title.
    * `JobDescriptor` - Get a job description.
    * `JobArea` - Get a job area expertise.
    * `JobType` - Get a type of job.
* **`Phone`**
    * `PhoneNumber` - Get a phone number.
    * `PhoneNumberFormat` - Gets a phone number based on the locale's phone_number.formats[] array index.
* **`Rant`**
    * `Review` - Generates a random user review.
    * `Reviews` - Generate an array of random reviews.
* **`System`**
    * `FileName` - Get a random file name.
    * `DirectoryPath` - Get a random directory path (Unix).
    * `FilePath` - Get a random file path (Unix).
    * `CommonFileName` - Generates a random file name with a common file extension.
    * `MimeType` - Get a random mime type.
    * `CommonFileType` - Returns a commonly used file type.
    * `CommonFileExt` - Returns a commonly used file extension.
    * `FileType` - Returns any file type available as mime-type.
    * `FileExt` - Gets a random extension for the given mime type.
    * `Semver` - Get a random semver version string.
    * `Version` - Get a random `Version`.
    * `Exception` - Get a random `Exception` with a fake stack trace.
    * `AndroidId` - Get a random GCM registration ID.
    * `ApplePushToken` - Get a random Apple Push Token.
    * `BlackBerryPin` - Get a random BlackBerry Device PIN.
* **`Vehicle`**
    * `Vin` - Generate a vehicle identification number (VIN).
    * `Manufacturer` - Get a vehicle manufacture name. IE: Toyota, Ford, Porsche.
    * `Model` - Get a vehicle model. IE: Camry, Civic, Accord.
    * `Type` - Get a vehicle type. IE: Minivan, SUV, Sedan.
    * `Fuel` - Get a vehicle fuel type. IE: Electric, Gasoline, Diesel.
* **`Random`/`Randomizer`**
    * `Number` - Get an int from 0 to max.
    * `Digits` - Get a random sequence of digits.
    * `Even` - Returns a random even number.
    * `Odd` - Returns a random odd number.
    * `Double` - Get a random double, between 0.0 and 1.0.
    * `Decimal` - Get a random decimal, between 0.0 and 1.0.
    * `Float` - Get a random float, between 0.0 and 1.0.
    * `Byte` - Generate a random byte between 0 and 255.
    * `Bytes` - Get a random sequence of bytes.
    * `SByte` - Generate a random sbyte between -128 and 127.
    * `Int` - Generate a random int between MinValue and MaxValue.
    * `UInt` - Generate a random uint between MinValue and MaxValue.
    * `ULong` - Generate a random ulong between -128 and 127.
    * `Long` - Generate a random long between MinValue and MaxValue.
    * `Short` - Generate a random short between MinValue and MaxValue.
    * `UShort` - Generate a random ushort between MinValue and MaxValue.
    * `Char` - Generate a random char between MinValue and MaxValue.
    * `Chars` - Generate a random chars between MinValue and MaxValue.
    * `String` - Get a string of characters of a specific length.
    * `String2` - Get a string of characters with a specific length drawing characters from `chars`.
    * `Hash` - Return a random hex hash. Default 40 characters, aka SHA-1.
    * `Bool` - Get a random boolean.
    * `ArrayElement<T>` - Get a random array element.
    * `ArrayElement` - Get a random array element.
    * `ArrayElements<T>` - Get a random subset of an array.
    * `ListItem<T>` - Get a random list item.
    * `ListItems<T>` - Get a random subset of a List.
    * `CollectionItem<T>` - Get a random collection item.
    * `ReplaceNumbers` - Replaces symbols with numbers.
    * `ReplaceSymbols` - Replaces each character instance in a string.
    * `Replace` - Replaces symbols with numbers and letters. # = number, ? = letter, * = number or letter.
    * `ClampString` - Clamps the length of a string between min and max characters.
    * `Enum<T>` - Picks a random Enum of T. Works only with Enums.
    * `Shuffle<T>` - Shuffles an IEnumerable source.
    * `Word` - Returns a single word or phrase in English.
    * `Words` - Gets some random words and phrases in English.
    * `WordsArray` - Get a range of words in an array (English).
    * `Guid` - Get a random GUID.
    * `Uuid` - Get a random GUID. Alias for Randomizer.Guid().
    * `RandomLocale` - Returns a random locale.
    * `AlphaNumeric` - Returns a random set of alpha numeric characters 0-9, a-z.
    * `Hexadecimal` - Generates a random hexadecimal string.
    * `WeightedRandom<T>` - Returns a selection of T[] based on a weighted distribution of probability.

</details>

_Bogus also supports multiple Locales._

## Using Bogus in our tests

First let's add the Bogus Nuget package either using the Nuget client of our IDE or the `dotnet add package` command.

```commandline
dotnet add package Bogus
```

We can now introduce the `Faker<T>` in our `CustomerControllerTests.cs` test class.

```csharp
private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>();
```

The faker class uses a fluent API to define rules for items such as properties that we want generated.
Since all of our properties are related to a Person, we will use the `faker.Person` object.

```csharp
private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>()
      .RuleFor(x => x.Email, f => f.Person.Email)
      .RuleFor(x => x.FullName, f => f.Person.FullName)
      .RuleFor(x => x.DateOfBirth, f => f.Person.DateOfBirth.Date)
      .RuleFor(x => x.GitHubUsername, f => f.Person.UserName.Replace(".", "").Replace("-", "").Replace("_", ""));
```

We can now call the `_customerGenerator.Generate()` method to generate a fake customer with realistic data.
Each `Generate` invocation will generate a completely different customer.

Since we will need to setup the GitHub users with the new fake usernames we will move the `GitHubApiServer` as a field in our class.

We can now replace this:

```csharp
var request = new CustomerRequest
{
    Email = "nick@chapsas.com",
    FullName = "Nick Chapsas",
    DateOfBirth = new DateTime(1993, 01, 01),
    GitHubUsername = "nickchapsas"
};
```

With this:

```csharp
var request = _customerGenerator.Generate();
_gitHubApiServer.SetupUser(request.GitHubUsername);
```

In some cases such as the `Create_ReturnsBadRequest_WhenTheEmailIsInvalid` test we might need to change the rules for a property.
In this case we want to make the test use the fake customer generator but return a "bad" email.

To do that we need to `clone` the generator object because if we re-configure it without cloning, the new rules will persist for the rest of the tests, making them fail.

```csharp
var request = _customerGenerator.Clone()
      .RuleFor(x => x.Email, "nick")
      .Generate();
```

### Exercise: Update all the tests to use the Customer Generator

Update all the tests to use the `_customerGenerator` instead of hand written customers.
