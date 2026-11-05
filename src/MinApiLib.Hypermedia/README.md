[![license](https://img.shields.io/badge/License-MIT-purple.svg)](../../LICENSE)
[![version](https://img.shields.io/nuget/vpre/MinApiLib.Hypermedia)](https://www.nuget.org/packages/MinApiLib.Hypermedia)

# MinApiLib.Hypermedia

Add links to responses when the client asks for them with the `Accept` header. Other clients keep receiving the plain response.

## Installation

```bash
dotnet add package MinApiLib.Hypermedia
```

## Usage

Register the services and apply the filter:

```csharp
global using MinApiLib.Hypermedia;
// ...
builder.Services.AddHypermedia();
// ...
app.MapEndpoints()
   .WithHypermedia();
```

Create a provider for each response type. `AddHypermedia()` finds them in the calling assembly and registers them as scoped services, so they can inject dependencies:

```csharp
public record struct Response(int Id, string Name);

public class ResponseHypermediaProvider : HypermediaProvider<Response>
{
    protected override IEnumerable<HypermediaLink> GetLinksFor(Response @object)
    {
        yield return new HypermediaLink("self", "/things/" + @object.Id, "GET");
        yield return new HypermediaLink("update", "/things/" + @object.Id, "PUT");
        yield return new HypermediaLink("delete", "/things/" + @object.Id, "DELETE");
        yield return new HypermediaLink("things", "/things", "GET");
    }
}

public record struct Request([FromRoute] int Id);

public record GetThing() : GetHandler<Request>("/things/{id}")
{
    protected override IResult Handle(Request request)
        => Results.Ok(new Response(request.Id, "John Doe"));
}
```

Send the hypermedia type in the `Accept` header to get the links:

```bash
$ curl -H "Accept: application/vnd.hypermedia+json" "http://localhost:5000/things/1"
{"value":{"id":1,"name":"John Doe"},"links":[{"href":"http://localhost:5000/things/1","rel":"self","method":"GET"},{"href":"http://localhost:5000/things/1","rel":"update","method":"PUT"},{"href":"http://localhost:5000/things/1","rel":"delete","method":"DELETE"},{"href":"http://localhost:5000/things","rel":"things","method":"GET"}]}
```

## Configuration

To use another media type, or to scan another assembly:

```csharp
builder.Services.AddHypermedia(typeof(Response).Assembly, contentType: "application/vnd.my.format+json");
```

To enable links only on some endpoints, skip the call on the group and apply the filter in each one:

```csharp
protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
    => builder.WithHypermedia();
```

## Behavior

- The response uses the hypermedia content type only when the `Accept` header lists it, including in a list of types. A `q=0` excludes it.
- Links are added to successful responses with a value, from any handler: `Results.Ok`, `Results.Created`, `Results.Json`, or a typed `TResponse`. Status code and headers such as `Location` are kept.
- Responses without a value, error responses, and values without a matching provider are returned unchanged. The client gets the plain value, with no error.
- Relative links are completed with the request scheme, host and path base. Absolute `http` and `https` links are kept as they are.
- Endpoints with the filter send `Vary: Accept`, so caches don't mix both representations.

## Matching providers

A provider matches with an `is T` check on the returned value, and the first match wins. So a provider of a base class also applies to its derived classes: don't derive a response from another one that has a provider of its own.

For a list, use the interface:

```csharp
public class ItemsHypermediaProvider : HypermediaProvider<IEnumerable<ResponseItem>>
{
    protected override IEnumerable<HypermediaLink> GetLinksFor(IEnumerable<ResponseItem> @object)
    {
        yield return new HypermediaLink("self", "/things", "GET");
    }
}
```

## OpenAPI

`ProducesHypermedia<T>()` declares the wrapped response in the endpoint metadata, with the configured content type:

```csharp
builder
    .ProducesHypermedia<Response>(StatusCodes.Status200OK)
    .Produces<Response>(StatusCodes.Status200OK);
```

`ApiExplorer` keeps one declaration per status code and the last one wins, so in this example the OpenAPI document shows only the plain `Response`. Documenting both representations requires a transformer or filter in your OpenAPI generator.
