---
name: minapilib-hypermedia
description: Add hypermedia links (HATEOAS) to a MinApiLib vertical slice with HypermediaProvider. Use when a response needs links, when adding a slice to an API that uses WithHypermedia, or when a request with the hypermedia Accept header returns no links.
---

# Add hypermedia to a slice

Links are only added when the client sends `Accept: application/vnd.hypermedia+json`. Other clients get the plain response.

## Wiring

Once, in `Program.cs`:

```csharp
builder.Services.AddHypermedia();
// ...
app.MapEndpoints()
   .WithHypermedia();
```

`AddHypermedia` registers every `HypermediaProvider` in the calling assembly, so providers need no manual registration.

## In the slice

```csharp
// Response.cs
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

// Handler.cs, in Configure: declare both representations, once per success status
.ProducesHypermedia<Response>(StatusCodes.Status200OK)
.Produces<Response>(StatusCodes.Status200OK)
```

For a paged list, add links conditionally:

```csharp
protected override IEnumerable<HypermediaLink> GetLinksFor(Response @object)
{
    yield return new HypermediaLink("self", $"/things?page={@object.Page}&pageSize={@object.PageSize}", "GET");
    if (@object.Page > 1)
    {
        yield return new HypermediaLink("previous", $"/things?page={@object.Page - 1}&pageSize={@object.PageSize}", "GET");
    }
    if (@object.Page < @object.TotalPages)
    {
        yield return new HypermediaLink("next", $"/things?page={@object.Page + 1}&pageSize={@object.PageSize}", "GET");
    }
    yield return new HypermediaLink("create", "/things", "POST");
}
```

## Rules

- **Every 2xx response with a value needs a provider** whose type matches it, when `.WithHypermedia()` is on the group. Without one, the client silently gets the plain value instead of `{ value, links }`.
- **The match is an `is T` check on the returned value.** A slice returning a list needs a `HypermediaProvider<IEnumerable<ResponseItem>>`, or a wrapper `Response` as `PagedList` does; a provider for `ResponseItem` doesn't apply. A provider of a base class also matches its derived classes, so don't derive one `Response` from another that has its own provider.
- **Slices without a body need nothing.** `Results.NoContent()`, `Results.NotFound()` and error results pass through the filter untouched.
- **Status and headers are kept.** `Results.Created` and `Results.CreatedAtRoute` keep their `Location`.
- **OpenAPI shows one declaration per status, and the last one wins.** The usual `.ProducesHypermedia<T>(200).Produces<T>(200)` documents only the plain `T`, so don't rely on Swagger to show the links.
- **Build `href`s from `HashedId` values**, never raw ints. A link with a raw id doesn't resolve: `/things/1` returns 400 or the wrong resource.
- **Put parent ids on the response** (`OwnerId`) rather than reading them from `Items.First()`, which fails on empty pages.
- **`rel` describes the target:** `self`, `update`, `delete`, `next`, `previous`, `create`, or the plural resource name of the link's actual target.
- **Relative `href`s start with `/`.** The library prepends scheme, host and `PathBase`.

If only some slices should have links, drop the group-level call and add `.WithHypermedia()` to those slices in `Configure`.

## Verify

Request the endpoint with the `Accept` header, and check the links resolve:

```bash
curl -H "Accept: application/vnd.hypermedia+json" http://localhost:5000/things/<hashed-id>
# {"value":{"id":"...","name":"..."},"links":[{"href":"http://localhost:5000/things/...","rel":"self","method":"GET"}]}
```
