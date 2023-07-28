# TP24 Receivables API

This is a very simple api implementation for the TP24 Coding challenge. The solution contains two projects. 1. The solution and 2, the test project. The tests only exist for the controller and nothing else. As it wasnt needed to test the shape of the data objects or the main programe.

I only had around 2/2.5 hours to tackle this, so there are a lot of rough edges/edge cases that aren't coveredd. ITs also not the most complete API, and could be improved by implementing more complex endpoints or filtering, query parameters allowing to search for open receivables for a given debter etc.

There could e a good case to imrpove the logging and error handling and currently there is miniam logging using Serilog. The error handling is all handles using simple try/catch blocks and the built-in/default status codes.

There are two api endpoints in the solution:

1. `api/receivables`
2. `api/receivables/summary`

This is very simple, and only handles very simple requests that match the basic request provided by TP24:

```json
[
  {
    "Reference": "INV-1234",
    "CurrencyCode": "USD",
    "IssueDate": "2023-07-27",
    "OpeningValue": 1000.0,
    "PaidValue": 500.0,
    "DueDate": "2023-08-27",
    "ClosedDate": "2023-09-01",
    "Cancelled": false,
    "DebtorName": "ABC Corp",
    "DebtorReference": "ABC-123",
    "DebtorAddress1": "123 Main St",
    "DebtorTown": "New York",
    "DebtorState": "NY",
    "DebtorZip": "10001",
    "DebtorCountryCode": "US",
    "DebtorRegistrationNumber": "123456"
  },
  {
    "Reference": "INV-5678",
    "CurrencyCode": "EUR",
    "IssueDate": "2023-07-28",
    "OpeningValue": 2000.0,
    "PaidValue": 1500.0,
    "DueDate": "2023-08-28",
    "DebtorName": "XYZ Ltd",
    "DebtorReference": "XYZ-456"
  }
]
```

## Assumptions

- The project is implemented using C#.NET and ASP.NET Core.
-The data for each receivable is provided in a JSON array in the request body.
- The data for each receivable follows the same format as the provided example payload in the technical test document.
- The receivable data must contain the required fields:
  - `Reference`
  - `CurrencyCode`
  - `IssueDate`
  - `OpeningValue`
  - `PaidValue`
  - `DueDate`
  - `DebtorName`
  - `DebtorReference`

- The following fields are assumed to be optional:
  - `ClosedDate`
  - `Cancelled`
  - `DebtorAddress1`
  - `DebtorAddress2`
  - `DebtorTown`
  - `DebtorState`
  - `DebtorZip`
  - `DebtorCountryCode`
  - `DebtorRegistrationNumber`
  
- If the request body is empty or contains no valid receivable data, the API will return a `400 Bad Request` response.
- If the request contains duplicate references for receivables that already exist in the system, the API will skip those duplicate receivables and return a `409 Conflict` response along with the list of skipped references.
- The API will log warnings for any duplicate references received.
- If there is an error while processing the receivable data, the API will return a `500 Internal Server Error` response.
- The project uses Serilog for logging, and the logs will be written to the console.
- The project includes unit tests implemented using `NUnit` and `Moq`.
- The test project is separate and referenced by the main project.
- The unit tests cover various scenarios, including valid data submission, invalid data submission, and duplicate references.
- The project includes a `.NET Test Explorer` configuration for running tests within Visual Studio Code.
- The project has been set up to use a `.NET Core` version compatible with at least `.NET 7.0`.

## How to Run

- Clone the repository to your local machine.
- Open the solution file in Visual Studio Code or any other compatible IDE.
- Make sure you have the necessary .NET SDK and runtime installed on your machine.
- Build the solution to restore NuGet packages and compile the code.
- Open a terminal and navigate to the project folder (tp24) to run the application using dotnet run.
- To run the unit tests, navigate to the test project folder (tests) in the terminal and use dotnet test.

## API Endpoints

- POST /api/receivables
- Submit receivables data.

### Request Body

The request body should contain an array of Receivable objects, following the format of the provided example payload.

the endpoint: `POST: /api/receivables` with the following body will push a receivable to the `database`

```json
[
  {
    "Reference": "INV-1234",
    "CurrencyCode": "USD",
    "IssueDate": "2023-07-27",
    "OpeningValue": 1000.0,
    "PaidValue": 500.0,
    "DueDate": "2023-08-27",
    "ClosedDate": "2023-09-01",
    "Cancelled": false,
    "DebtorName": "ABC Corp",
    "DebtorReference": "ABC-123",
    "DebtorAddress1": "123 Main St",
    "DebtorTown": "New York",
    "DebtorState": "NY",
    "DebtorZip": "10001",
    "DebtorCountryCode": "US",
    "DebtorRegistrationNumber": "123456"
  },
  {
    "Reference": "INV-5678",
    "CurrencyCode": "EUR",
    "IssueDate": "2023-07-28",
    "OpeningValue": 2000.0,
    "PaidValue": 1500.0,
    "DueDate": "2023-08-28",
    "DebtorName": "XYZ Ltd",
    "DebtorReference": "XYZ-456"
  }
]
```

the following response will get returned:

### Add new Receivable Response

- If the request contains valid data and there are no duplicate references, the API will return a 201 Created response.

  ```json
    {
      "reference": "INV-001",
      "currentDate": "2023-07-28T15:52:16.776948Z",
      "value": 2500
    }
  ```

- If the request body is empty or contains no valid receivable data, the API will return a 400 Bad Request response.

  ```json
    {
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "Bad Request",
    "status": 400,
    "traceId": "00-2d941c34991c88b19294c52e7daf6721-808017623057d97a-00"
    }
  ```

- If the request contains duplicate references for receivables that already exist in the system, the API will skip those duplicate receivables and return a 409 Conflict response along with the list of skipped references.

  ```json
    {
    "reference": "INV-001,INV-002",
    "currentDate": "2023-07-28T15:53:56.046344Z",
    "value": 2500
    }
  ```

### Summary Response

`GET /api/receivables/summary` Retrieve summary statistics about the stored receivables data.

The API will return summary statistics about the stored receivables data, including the value of open and closed invoices.

this will look like this:

```json
{
    "totalOpenInvoices": 2500,
    "totalClosedInvoices": 0,
    "currentTime": "2023-07-28T16:49:54.058776+01:00"
}
```
