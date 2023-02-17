# MinApiLib.HashedIds

[![license](https://img.shields.io/badge/License-MIT-purple.svg)](../../LICENSE)
[![version](https://img.shields.io/nuget/vpre/MinApiLib.HashedIds)](https://www.nuget.org/packages/MinApiLib.HashedIds)

This package contains extensions to use hashed ids in your endpoints. It uses the [Hashids](https://hashids.org/) library.

## Installation

You can install this package using the NuGet package manager:

```bash
Install-Package MinApiLib.HashedIds
```

Or using the .NET CLI:

```bash
dotnet add package MinApiLib.HashedIds
```

## Usage

To use hashed ids, you can add the `AddHashedIds` method in your `Program.cs` file:

```csharp
global using MinApiLib.HashedIds;
// ...
builder.Services.AddHashedIds(op => op.Passphrase = "my secret passphrase");
```

Then, you can use the `HashedId` type in your endpoints:

```csharp
public record struct Response(HashedId Id, string Name);

public record Get() : GetEndpoint("/things")
{
    public IResult Handle()
        => Results.Ok(new Response(1, "John Doe"));
}
```

And test it with curl:

```bash
$ curl "http://localhost:5000/things"
{"id":"Wd","name":"John Doe"}%
```

You can also use the `HashedId` type in your requests and responses:

```csharp
public record struct Request(HashedId Id, CancellationToken CancellationToken);

public record struct Response(HashedId Id, string Name);

public record GetThing() : GetEndpoint<Request>("/things/{id}")
{
    protected override Task<IResult> OnHandleAsync(Request request, CancellationToken cancellationToken)
    {
        int id = request.Id;
        var response = new Response(id, "John Doe");
        return Task.FromResult(Results.Ok(response));
    }
}
```

And test it:

```bash
$ curl "http://localhost:5000/things/Wd"
{"id":"Wd","name":"John Doe"}%
```