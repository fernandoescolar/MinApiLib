# Vertical Slice APIs with MinApiLib

Context for agents writing ASP.NET Core Minimal APIs with **Vertical Slice Architecture** on top of **MinApiLib**.
The reference implementation is `samples/Example.VerticalSlice.Api`.

Scope: API application code. The library itself (`src/`) is not organized in slices; do not apply these layout rules there.

## The paradigm

Code is organized by **use case**, not by technical layer. Each endpoint is a self-contained slice holding its route, input, output, validation, logic and links. Changing one use case should touch one folder.

Do not add `Controllers/`, `Services/`, `Repositories/`, `Dtos/` or `Mappers/` layers, MediatR-style dispatchers, or generic CRUD base classes. The handler talks to the data access directly.

## Layout

```
Features/
  PagedList.cs                  # the few types truly shared by slices
  Beers/                        # resource: grouping folder only
    CreateBeer/                 # slice: <Verb><Noun>
      Handler.cs                # route, OpenAPI metadata, logic
      Request.cs                # Request + RequestBody (+ validator)
      Response.cs               # Response + mapping + hypermedia provider
    GetBeers/ ...
Program.cs
_Imports.cs                     # global usings
```

- **Namespace:** `<RootNamespace>.Features.<SliceName>`. It skips the resource folder, so `Features/Beers/CreateBeer` is `...Features.CreateBeer`.
- **Slice names** are unique across the project, and verb + noun: `GetBeer`, `GetBeers`, `GetBeersByBrewery`, `CreateBeer`, `CreateOrUpdateBeer`, `DeleteBeer`. Never name a slice like an entity: a `Beer` namespace would shadow the `Beer` type.
- **Type names are always `Handler`, `Request`, `Response`** (`ResponseItem`, `ResponseChild`, `RequestBody` for nested ones). The namespace disambiguates them.
- Omit `Response.cs` when the slice returns no body.

## Rules

1. **Handlers** are `public record Handler() : <Verb>HandlerAsync<Request>("/route")`, overriding `Configure` and `HandleAsync`. Default to the `<TRequest>` form returning `IResult`.
2. **Requests** are `public record struct Request(...)`, bound with `[AsParameters]` by the library. Give every member an explicit source: `[FromServices]`, `[FromRoute]`, `[FromQuery]`, `[FromBody]`, `[FromHeader]`.
3. **Input is validated with a FluentValidation validator for `Request`,** next to it in `Request.cs`, and `.WithValidation()` in `Configure`. Name the JSON body member `Body`.
4. **Dependencies go in `Request` with `[FromServices]`, never in the handler constructor.** Handlers are created once at startup, so they behave as singletons.
5. **`Configure` declares the whole contract:** `.WithName("<SliceName>")`, `.WithTags("<Resource>")`, and a `.Produces` for every status the handler returns, no more and no fewer.
6. **`HandleAsync`** starts with `cancellationToken.ThrowIfCancellationRequested();` and passes the token to every async call.
7. **Public ids are `HashedId`**, in requests, responses and links. A raw `int` id never crosses the API boundary, including foreign keys in list items.
8. **Mapping:** single entities use an `implicit operator Response(Entity)`; lists project with `Select` so only the needed columns are queried.
9. **Slices do not reference each other's types.** They only meet through route names (`Results.CreatedAtRoute("GetBeer", ...)`) and URLs. Identical `Response` classes in two slices are intentional.
10. **Sharing is the exception.** Keep a helper private to its slice folder until a third slice needs the same thing for the same reason, then move it to `Features/` (types) or a service registered with `[RegisterInIServiceCollection]` and received through `Request`.
11. **Every slice gets tests** for its success path and each declared error status.

## MinApiLib map

| Package | Use | Wire-up |
|---|---|---|
| `MinApiLib.Endpoints` | Handler records, assembly scan | `app.MapEndpoints()` |
| `MinApiLib.FluentValidation` | Validators for `Request` | `services.AddValidatorsFromAssembly()`, `.WithValidation()` |
| `MinApiLib.HashedIds` | `HashedId`, `HashedLongId` | `services.AddHashedIds(...)` |
| `MinApiLib.Hypermedia` | Links on `Accept: application/vnd.hypermedia+json` | `services.AddHypermedia()`, `.WithHypermedia()` |
| `MinApiLib.OperationCanceled` | Requests aborted by the client become 499 | `app.CatchOperationCanceled()` |
| `MinApiLib.DependencyInjection` | Attribute-based registration | `services.AddAssembly()` |

Handler base records exist for `Get`, `Post`, `Put`, `Delete`, `Patch` and `Query`, as `<Verb>Handler` (sync) and `<Verb>HandlerAsync`, each with no generics, `<TRequest>` and `<TRequest, TResponse>`.

Each package has its own README with details: `src/<Package>/README.md`. `MinApiLib.Validation` is deprecated: never add it.

## Traps

Each of these fails silently or far from its cause.

| Trap | Consequence | Do this |
|---|---|---|
| A validator for `RequestBody` instead of `Request`, or none | `.WithValidation()` finds no validator: requests pass unchecked, with only a warning in the log | Target the type the handler receives, and test that invalid input gets a 400 |
| A missing `HashedId` in the JSON body | It arrives as `0`; an invalid one is already a 400 | Mark the property `required`, and look up the entity: a valid hash can point to nothing |
| A `Response` deriving from another with a provider | The base provider can answer for it, with the wrong links | Don't derive; give each response its own type and provider |
| 2xx with a value and no matching provider | The client asked for hypermedia and silently gets the plain value, without links | Give every such response a provider, including lists |
| `<TRequest, TResponse>` handlers | Only 200 can be returned | Use `<TRequest>` and return `IResult` when the slice has other outcomes |
| A `QUERY` endpoint in Swagger | Swashbuckle throws, so the **whole** `swagger.json` fails | Add `.ExcludeFromDescription()` to `QUERY` endpoints |
| Unchecked paging values | `pageSize=-1` returned every row on SQLite; `pageSize=0` returned `totalPages: 2147483647` | Check ranges in the validator of `Request` |
| Scoped service in a handler constructor | Fails at startup, or becomes a captive dependency shared by all requests | Use `[FromServices]` in `Request` |
| `HashedId` passed to `FindAsync` | Key type mismatch | `FindAsync(new object[] { (int)req.Id }, cancellationToken)` |
| Duplicate `Response` schema ids in Swagger | Swashbuckle cannot tell slices apart | `c.CustomSchemaIds(x => x.FullName)` |

`HashedId` obfuscates ids; it is not authorization. Check access to the resource as usual.

## Skills

| Skill | When |
|---|---|
| `minapilib-setup` | Creating an API project or adding MinApiLib to one |
| `minapilib-add-slice` | Adding or changing endpoints, or a whole resource |
| `minapilib-validation` | Adding input validation to a slice |
| `minapilib-hypermedia` | Adding hypermedia links to a slice |
| `minapilib-tests` | Writing integration tests for slices |
| `minapilib-review` | Reviewing slices against these rules |

## This repository

- `MinApiLib.sln` holds the library and its tests; the samples are in `samples/MinApiLib.Samples.sln`.
- The libraries and tests target `net8.0;net9.0;net10.0`, and the samples `net10.0`. Shared settings are in `src/Directory.Build.props` and `tests/Directory.Build.props`.
- Libraries using the Web SDK must set `<OutputType>Library</OutputType>` in their own `.csproj`: the SDK overrides it from `Directory.Build.props`.
- The context and skills for AI tools come from `.ai`. Edit them there, then run `.ai/convert.sh`: the copies each tool reads are generated.
