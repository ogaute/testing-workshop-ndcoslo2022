---
description: The API that we'll be using for our deep dive into Unit testing
---

# The Foreign Exchange API

For the following "Deep Dive" section we will be using the ForeignExchange.Api project which you can find in `1.UnitTesting\2.DeepDive\src\ForeignExchange.Api`.

## Running the API

In order to run this API you need to have Docker running on your machine. Once Docker is running you need to spin up the Postgres databases needed by the API.

To do that, open a terminal in the `ForeignExchange.Api` directory and run:

```commandline
docker compose up
```

Now that the database is running you can either run the API from your IDE directly or use:

```commandline
dotnet run
```

## Calling the API

The API has a single endpoint which provides a FX quote given a base and a quote currency and an amount of money.

Supported currencies: GBP, USD, EUR, AUD, CAD

```http request
GET https://localhost:5001/forex/quotes/{baseCurrency}/{quoteCurrency}/{amount:decimal}
```

- `baseCurrency`: The currency that you want to convert from
- `quoteCurrency`: The currency that you want to convert into
- `amount`: The amount of money you want to convert

### Example request

Request:
```http request
GET https://localhost:5001/forex/quotes/USD/GBP/150
```

Response:
```json
{
  "baseCurrency": "USD",
  "quoteCurrency": "GBP",
  "baseAmount": 150,
  "quoteAmount": 128.45542650
}
```

When the currency you are trying to use is not supported you will receive a 404 response:
```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    "title": "Not Found",
    "status": 404,
    "traceId": "00-5daef038211467a2a66949fc14b46f60-db38e2d156ec2aa2-00"
}
```


When your `baseCurrency` is the same as your `quoteCurrency` then you're getting a 400 response:
```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "traceId": "0HMKMDGHMEBN3:00000004",
    "errors": {
        "Currency": [
            "You cannot convert currency GBP to itself"
        ]
    }
}
```

When the amount you are trying to convert is less or equal to 0 then you're getting a 400 response:
```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "traceId": "0HMKMDL2JLUI3:00000002",
    "errors": {
        "Amount": [
            "You can only convert a positive amount of money"
        ]
    }
}
```
