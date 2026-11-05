[![license](https://img.shields.io/badge/License-MIT-purple.svg)](../../LICENSE)
[![version](https://img.shields.io/nuget/vpre/MinApiLib.Hypermedia)](https://www.nuget.org/packages/MinApiLib.Hypermedia)

# MinApiLib.Hypermedia

This package contains extensions to use hypermedia in your projects.

## Installation

You can install this package using the NuGet package manager:

```bash
Install-Package MinApiLib.Hypermedia
```

Or using the .NET CLI:

```bash
dotnet add package MinApiLib.Hypermedia
```

## Usage

To use hypermedia, you need to add the following code in your `Program.cs`:

```csharp
global using MinApiLib.Hypermedia;
// ...
builder.Services.AddHypermedia();
// ...
app.MapEndpoints()
   .WithHypermedia();
```

It will use *"application/vnd.hypermedia+json"* as default mime type, but you can change it by adding the `contentType` parameter:

```csharp
builder.Services.AddHypermedia(contentType: "application/vnd.my.format+json");
```

Then you should create your hypermedia providers for your response models:

```csharp
public record struct Response(HashedId Id, string Name);

public class ResponseHypermediaProvider : HypermediaProvider<Response>
{
    protected override IEnumerable<HypermediaLink> GetLinksFor(Response @object)
    {
        yield return new HypermediaLink("self", "/things/" + @object.Id, "GET");
        yield return new HypermediaLink("update", "/things/" + @object.Id, "PUT");
        yield return new HypermediaLink("delete", "/things/" + @object.Id, "DELETE");
        yield return new HypermediaLink("beers", "/things", "GET");
    }
}

public record struct Request([FromRoute] int Id);

public record GetThing() : GetHandler<Request>("/things/{id}")
{
    protected override IResult Handle(Request request)
        => Results.Ok(new Response(request.Id, "John Doe"));
}
```

You should add "application/vnd.hypermedia+json" as "Accept" header in order to get hypermedia links:

```bash
$ curl -H "Accept: application/vnd.hypermedia+json" "http://localhost:5000/things/1"
{"value":{"id":"Wk","name":"John Doe"},"links":[{"href":"http://localhost:5000/things/Wk","rel":"self","method":"GET"},{"href":"http://localhost:5000/things/Wk","rel":"update","method":"PUT"},{"href":"http://localhost:5000/things/Wk","rel":"delete","method":"DELETE"},{"href":"http://localhost:5000/things","rel":"beers","method":"GET"}]}%
```

## Behavior

- The response uses the hypermedia content type only when the `Accept` header lists it. Any other request gets the plain response.
- Links are added to successful responses with a value, from any handler: `Results.Ok`, `Results.Created`, `Results.Json`, or a typed `TResponse`. Status code and headers such as `Location` are kept.
- Responses without a value, error responses, and values without a matching provider are returned unchanged.
- A provider matches with an `is T` check, so use `HypermediaProvider<IEnumerable<Item>>` for lists.
- Relative links are completed with the request scheme, host and path base. Absolute `http` and `https` links are kept as they are.

In OpenAPI, `ApiExplorer` keeps one declaration per status code and the last one wins. If you call `.ProducesHypermedia<T>(200)` and then `.Produces<T>(200)`, only the plain `T` is documented.
