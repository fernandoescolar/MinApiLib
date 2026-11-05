[![license](https://img.shields.io/badge/License-MIT-purple.svg)](../../LICENSE)
[![version](https://img.shields.io/nuget/vpre/MinApiLib.OperationCanceled)](https://www.nuget.org/packages/MinApiLib.OperationCanceled)

# MinApiLib.OperationCanceled

Middleware that answers with a `499` status code (Client Closed Request) when the client cancels a request, instead of letting the `OperationCanceledException` surface as an unhandled error.

## Installation

```bash
dotnet add package MinApiLib.OperationCanceled
```

## Usage

```csharp
global using MinApiLib.OperationCanceled;
// ...
app.CatchOperationCanceled();
```

Add it before any middleware whose cancellations it should catch. Then pass the `CancellationToken` to your async calls, or throw when it's canceled:

```csharp
protected override async Task<IResult> HandleAsync(Request request, CancellationToken cancellationToken)
{
    cancellationToken.ThrowIfCancellationRequested();

    var thing = await db.Things.FindAsync(new object[] { request.Id }, cancellationToken);
    // ...
}
```

To use another status code:

```csharp
app.CatchOperationCanceled(options => options.StatusCode = 444);
```

## Behavior

- **It only handles cancellations of aborted requests,** when `HttpContext.RequestAborted` is canceled. Any other `OperationCanceledException`, such as a timeout in an outgoing call, is a server problem: it isn't caught, and it surfaces as usual.
- The cancellation is logged at `Information` level, and the response body is `Client Closed Request`.
- If the response has already started, its status can't change, so the middleware only logs the cancellation.

## Upgrading

The middleware used to answer `499` to every `OperationCanceledException`, even with the client still connected.
