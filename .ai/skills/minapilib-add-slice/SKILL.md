---
name: minapilib-add-slice
description: Add or change an API endpoint as a vertical slice using MinApiLib handlers (Handler, Request, Response), or scaffold all the endpoints of a resource. Use whenever a task adds a route, feature or CRUD resource to a MinApiLib API, even if the user doesn't mention slices.
---

# Add a vertical slice

Follow the rules and traps in `AGENT.md`. Copy the closest template from [references/templates.md](references/templates.md).

## Steps

1. **Name the use case** as verb + noun (`CreateBeer`), and check that the name is unique across `Features/`.
2. **Create** `Features/<Resource>/<SliceName>/` with `Handler.cs`, `Request.cs` and, if it returns a body, `Response.cs`. The namespace is `<RootNamespace>.Features.<SliceName>`, without the resource.
3. **Write `Request`:** a `record struct` with an explicit source on every member, and its validator if the slice receives input.
4. **Write `Response`:** only what this use case returns, with ids as `HashedId`. Do not reuse another slice's type.
5. **Write `HandleAsync`:** the logic lives here. Call the data access directly.
6. **Write `Configure`:** `.WithName("<SliceName>")`, `.WithTags("<Resource>")`, and one `.Produces` per status returned. Re-read the handler's `return` statements to check them.
7. **Add what the slice needs:** validation with `minapilib-validation`, links with `minapilib-hypermedia`.
8. **Test it** with `minapilib-tests`.

## Choosing the status code

| Situation | Result |
|---|---|
| Read one, found | `Results.Ok(resource)` |
| Read one or delete, missing | `Results.NotFound()` |
| List with no rows | `Results.NoContent()` |
| Created | `Results.CreatedAtRoute("Get<Noun>", new { id = resource.Id.ToString() }, resource)` |
| Deleted | `Results.NoContent()` |
| Invalid input | The validator answers 400 through `.WithValidation()` |
| Reference to a missing entity in the body | `Results.BadRequest("...")` |

## Whole resource

Create one independent slice per operation, usually `Get<Nouns>`, `Get<Noun>`, `Create<Noun>`, `CreateOrUpdate<Noun>` and `Delete<Noun>`, all under `Features/<Nouns>/`. Do not extract a shared `Response`, a base handler or a generic repository, even if the classes start out identical.

Start with `Get<Noun>`: the `Create<Noun>` slice links to its route name.

## Changing an existing slice

Change only that slice, even if a sibling has the same code. If a change really must apply to several slices, make it in each one, or follow rule 10 in `AGENT.md` if the same reason now applies to three or more.

## Where other code goes

| Code | Location |
|---|---|
| Used by one slice | Its folder, as a private method or a local type |
| Same type needed by 3+ slices | `Features/`, in the `<RootNamespace>.Features` namespace (visible to slices without a `using`) |
| Service used by 3+ slices or wrapping infrastructure | A class with `[RegisterInIServiceCollection(typeof(IThing), ServiceLifetime.Scoped)]`, received through `Request` with `[FromServices]` |
| Entities and `DbContext` | The data project or folder, outside `Features/` |

`[RegisterInIServiceCollection]` defaults to `Transient`, and needs `builder.Services.AddAssembly()`.

## Data access notes

- Compare `HashedId` in LINQ with a cast: `t.Id == (int)req.Id`.
- `FindAsync` needs the raw key: `FindAsync(new object[] { (int)req.Id }, cancellationToken)`.
- `FindAsync` does not load navigation properties. Use `Include` when the response maps them.
- Projecting `Id = t.Id` into a `HashedId` property works inside `Select`.
