---
description: The API we will be using for integration tests
---

# The Customers API

We will be writing integration tests for the following Customers API. This API is used to create customers or users in a system.
It uses a Postgres database to store the users and it is also calling the GitHub API to validate that the provided GitHub Username is valid.

In order to make calling this API easier, I have included a Postman collection in the `Assets` folder under `2.IntegrationTesting`.

To import the collection simply use the Import button in postman and either select or drag and drop the `json` collection file.

## Running the API

To run the API make sure you are in the `Customers.Api` directory and run `docker compose -f docker-compose-full.yml up`.
This will run everything including the database needed for the API to function.

If you want to run just the database, so you can run the API independently as we are working on it, simply do `docker compose up`. 
This approach is **HIGHLY RECOMMENDED** since we will be working on running our own version of the API as part of the testing we will be performing.

## The endpoints

When the API is running, you can access the Swagger page that contains all the requests and responses at: [https://localhost:5001/swagger/index.html](https://localhost:5001/swagger/index.html)

### Creating a customer

```http request
POST http://localhost:5001/customers
```

Request body:
```json
{
  "gitHubUsername": "nickchapsas",
  "fullName": "Nick Chapsas",
  "email": "nick@chapsas.com",
  "dateOfBirth": "1990-01-01T13:00:00.000Z"
}
```

Example response:
```json
{
    "id": "f3cde51e-9b98-48a1-b6b7-870edece7e6b",
    "gitHubUsername": "nickchapsas",
    "fullName": "Nick Chapsas",
    "email": "nick@chapsas.com",
    "dateOfBirth": "1990-01-01T13:00:00"
}
```

### Getting all customers

```http request
GET http://localhost:5001/customers
```

Example response:
```json
{
    "customers": [
        {
            "id": "f3cde51e-9b98-48a1-b6b7-870edece7e6b",
            "gitHubUsername": "nickchapsas",
            "fullName": "Nick Chapsas",
            "email": "nick@chapsas.com",
            "dateOfBirth": "1990-01-01T13:00:00"
        }
    ]
}
```

### Getting a customer by id

```http request
GET http://localhost:5001/customers/{guid}
```

Example response:
```json
{
    "id": "f3cde51e-9b98-48a1-b6b7-870edece7e6b",
    "gitHubUsername": "nickchapsas",
    "fullName": "Nick Chapsas",
    "email": "nick@chapsas.com",
    "dateOfBirth": "1990-01-01T13:00:00"
}
```

### Updating a customer

```http request
PUT http://localhost:5001/customers/{guid}
```

Request body:
```json
{
    "gitHubUsername": "nickchapsas",
    "fullName": "Nick Chapsas",
    "email": "nick@chapsas.com",
    "dateOfBirth": "1990-02-01T13:00:00"
}
```

Example response:
```json
{
    "id": "f3cde51e-9b98-48a1-b6b7-870edece7e6b",
    "gitHubUsername": "nickchapsas",
    "fullName": "Nick Chapsas",
    "email": "nick@chapsas.com",
    "dateOfBirth": "1990-02-01T13:00:00"
}
```

### Deleting a customer

```http request
DELETE http://localhost:5001/customers/{guid}
```
