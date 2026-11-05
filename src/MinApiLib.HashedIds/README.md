[![license](https://img.shields.io/badge/License-MIT-purple.svg)](../../LICENSE)
[![version](https://img.shields.io/nuget/vpre/MinApiLib.HashedIds)](https://www.nuget.org/packages/MinApiLib.HashedIds)

# MinApiLib.HashedIds

Expose integer ids as short hashes in routes and JSON, using the [Hashids](https://hashids.org/) library.

## Installation

```bash
dotnet add package MinApiLib.HashedIds
```

## Usage

Configure the hasher when the application starts:

```csharp
global using MinApiLib.HashedIds;
// ...
builder.Services.AddHashedIds(op =>
{
    op.Passphrase = builder.Configuration["HashedIds:Passphrase"];
    op.MinHashLength = 8; // optional
});
```

`AddHashedIds` throws if the passphrase is null, and using a `HashedId` before calling it throws an `InvalidOperationException`. Keep the passphrase and the minimum length stable: changing them changes public ids, and breaks the ones clients have stored.

Use `HashedId` in requests and responses. It converts implicitly from and to `int`:

```csharp
public record struct Request([FromRoute] HashedId Id);
public record struct Response(HashedId Id, string Name);

public record GetThing() : GetHandler<Request>("/things/{id}")
{
    protected override IResult Handle(Request request)
    {
        int id = request.Id;
        return Results.Ok(new Response(id, "John Doe"));
    }
}
```

```bash
$ curl "http://localhost:5000/things/Wd"
{"id":"Wd","name":"John Doe"}
```

For `long` keys, use `HashedLongId`, which works the same way.

## Invalid values

An invalid value in a route, a query string or a JSON body gets a `400` before the handler runs. In the `Development` environment, ASP.NET Core shows this `400` with its exception page.

A **missing** JSON property isn't an invalid value: it gets the default, `0`. To reject it, mark the property `required`:

```csharp
public class RequestBody
{
    public required HashedId OwnerId { get; set; }
}
```

`[Required]` has no effect, because `HashedId` is a non-nullable value type. A valid hash can still point to an entity that doesn't exist, so look it up in the handler.

## With Entity Framework

Cast to `int` in queries and key lookups:

```csharp
var thing = await db.Things.FirstOrDefaultAsync(t => t.Id == (int)request.Id, cancellationToken);
var other = await db.Things.FindAsync(new object[] { (int)request.Id }, cancellationToken);
```

Assigning an `int` to a `HashedId` property works inside a `Select` projection. Filter and sort before it, on the entity's columns:

```csharp
db.Things.OrderBy(t => t.Name).Select(t => new ResponseItem { Id = t.Id, Name = t.Name });
```

## With Swagger

Map the type so it's documented as a string. With Swashbuckle 10:

```csharp
builder.Services.AddSwaggerGen(c => c.MapType<HashedId>(() => new OpenApiSchema { Type = JsonSchemaType.String }));
```

On Swashbuckle 6, use `new OpenApiSchema { Type = "string" }`.

## Limitations

- **It isn't a security mechanism.** Hashids obfuscates ids, without encrypting them. Keep checking access to each resource.
- Negative values have no hash.
- The hasher is static and shared by the whole process, because model binding and JSON conversion use it without access to dependency injection. One process can only use one configuration, and the last call to `AddHashedIds` wins.

## Upgrading

An invalid hashed id in a JSON body used to reach the handler as `-1`. Now the request is rejected with a `400`.
