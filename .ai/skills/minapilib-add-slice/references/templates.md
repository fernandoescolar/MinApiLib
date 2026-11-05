# Slice templates

Replace `Api`, `Thing`, `Things`, `AppDbContext` and the `/things` route. Each template omits the `namespace Api.Features.<SliceName>;` line that starts every file.

Hypermedia providers appear in `minapilib-hypermedia`. If the app applies `.WithHypermedia()` to the whole group, every response with a body should have one, or it comes back without links.

## Read one: `GetThing`

```csharp
// Request.cs
public record struct Request(
    [FromServices] AppDbContext Database,
    [FromRoute] HashedId Id
);

// Response.cs
public class Response
{
    public HashedId Id { get; set; }
    public string Name { get; set; }

    public static implicit operator Response(Thing thing)
        => new Response { Id = thing.Id, Name = thing.Name };
}

// Handler.cs
public record Handler() : GetHandlerAsync<Request>("/things/{id}")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<Response>(StatusCodes.Status200OK)
                .Produces<Response>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("GetThing")
                .WithTags("Things");

    protected override async Task<IResult> HandleAsync(Request req, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var thing = await req.Database.Things.FirstOrDefaultAsync(t => t.Id == (int)req.Id, cancellationToken);
        if (thing is null)
        {
            return Results.NotFound();
        }

        return Results.Ok((Response)thing);
    }
}
```

## List with paging: `GetThings`

```csharp
// Features/PagedList.cs (shared, once per project; namespace Api.Features)
public class PagedList<T>
{
    public PagedList()
    {
    }

    public PagedList(IEnumerable<T> items, int total, int page, int pageSize)
    {
        Items = items;
        TotalResults = total;
        Page = page;
        PageSize = pageSize;
    }

    public IEnumerable<T> Items { get; set; }
    public int TotalResults { get; set; }
    public int TotalPages { get => (int)Math.Ceiling((double)TotalResults / PageSize); }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

// Request.cs
public record struct Request(
    [FromServices] AppDbContext Database,
    [FromQuery] int Page = 1,
    [FromQuery] int PageSize = 10
);

public class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

// Response.cs
public class Response : PagedList<ResponseItem>
{
    public Response() : base()
    {
    }

    public Response(IEnumerable<ResponseItem> items, int total, int page, int pageSize) : base(items, total, page, pageSize)
    {
    }
}

public class ResponseItem
{
    public HashedId Id { get; set; }
    public string Name { get; set; }
}

// Handler.cs
public record Handler() : GetHandlerAsync<Request>("/things")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<Response>(StatusCodes.Status200OK)
                .Produces<Response>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .WithName("GetThings")
                .WithTags("Things")
                .WithValidation();

    protected override async Task<IResult> HandleAsync(Request req, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var total = await req.Database.Things.CountAsync(cancellationToken);
        if (total == 0)
        {
            return Results.NoContent();
        }

        var items = await req.Database
                             .Things
                             .OrderBy(t => t.Name)
                             .Skip((req.Page - 1) * req.PageSize)
                             .Take(req.PageSize)
                             .Select(t => new ResponseItem { Id = t.Id, Name = t.Name })
                             .ToListAsync(cancellationToken);

        return Results.Ok(new Response(items, total, req.Page, req.PageSize));
    }
}
```

## List nested under a parent: `GetThingsByOwner`

As `GetThings`, with these differences. The parent id is part of `Response`, so links can use it even when a page is empty.

```csharp
// Request.cs: the route parameter is {id}, the property has a clearer name; the validator is the same
public record struct Request(
    [FromRoute(Name = "id")] HashedId OwnerId,
    [FromServices] AppDbContext Database,
    [FromQuery] int Page = 1,
    [FromQuery] int PageSize = 10
);

// Response.cs: add to Response, next to the constructors
public HashedId OwnerId { get; set; }

// Handler.cs: route "/owners/{id}/things", and also declare .Produces(StatusCodes.Status404NotFound)
var owner = await req.Database.Owners.FindAsync(new object[] { (int)req.OwnerId }, cancellationToken);
if (owner is null)
{
    return Results.NotFound();
}

var query = req.Database.Things.Where(t => t.Owner.Id == (int)req.OwnerId);
// CountAsync, OrderBy, Skip, Take and Select on `query`, as in GetThings

return Results.Ok(new Response(items, total, req.Page, req.PageSize) { OwnerId = req.OwnerId });
```

## Create: `CreateThing`

```csharp
// Request.cs
public record struct Request(
    [FromServices] AppDbContext Database,
    [FromBody] RequestBody Body
);

public class RequestBody
{
    public string Name { get; set; }

    public required HashedId OwnerId { get; set; }
}

public class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Body.Name).NotEmpty().Length(3, 100);
    }
}

// Response.cs: as GetThing, in its own namespace

// Handler.cs
public record Handler() : PostHandlerAsync<Request>("/things")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<Response>(StatusCodes.Status201Created)
                .Produces<Response>(StatusCodes.Status201Created)
                .WithName("CreateThing")
                .WithTags("Things")
                .WithValidation();

    protected override async Task<IResult> HandleAsync(Request req, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // A valid hash can still point to an entity that doesn't exist.
        var owner = await req.Database.Owners.FindAsync(new object[] { (int)req.Body.OwnerId }, cancellationToken);
        if (owner is null)
        {
            return Results.BadRequest("Unknown owner");
        }

        var thing = new Thing { Name = req.Body.Name, Owner = owner };
        req.Database.Things.Add(thing);
        await req.Database.SaveChangesAsync(cancellationToken);

        var resource = (Response)thing;
        return Results.CreatedAtRoute("GetThing", new { id = resource.Id.ToString() }, resource);
    }
}
```

## Create or update: `CreateOrUpdateThing`

`Request` adds `[FromRoute] HashedId Id` to the create template, with its own validator, and `Response` is its own copy. If the body references other entities, look them up first and return 400, as `CreateThing` does.

```csharp
public record Handler() : PutHandlerAsync<Request>("/things/{id}")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<Response>(StatusCodes.Status200OK)
                .Produces<Response>(StatusCodes.Status200OK)
                .WithName("CreateOrUpdateThing")
                .WithTags("Things")
                .WithValidation();

    protected override async Task<IResult> HandleAsync(Request req, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var thing = await req.Database.Things.FindAsync(new object[] { (int)req.Id }, cancellationToken);
        if (thing is null)
        {
            thing = new Thing { Id = (int)req.Id };
            req.Database.Things.Add(thing);
        }

        thing.Name = req.Body.Name;

        await req.Database.SaveChangesAsync(cancellationToken);

        return Results.Ok((Response)thing);
    }
}
```

## Delete: `DeleteThing`

There is no `Response.cs`.

```csharp
// Request.cs
public record struct Request(
    [FromServices] AppDbContext Database,
    [FromRoute] HashedId Id
);

// Handler.cs
public record Handler() : DeleteHandlerAsync<Request>("/things/{id}")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("DeleteThing")
                .WithTags("Things");

    protected override async Task<IResult> HandleAsync(Request req, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var thing = await req.Database.Things.FindAsync(new object[] { (int)req.Id }, cancellationToken);
        if (thing is null)
        {
            return Results.NotFound();
        }

        req.Database.Things.Remove(thing);
        await req.Database.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
```

## Search with a body: `SearchThings` (HTTP `QUERY`)

Use `QUERY` for safe reads whose criteria don't fit in a URL. `.ExcludeFromDescription()` is mandatory while Swagger is enabled, because Swashbuckle fails the whole document on this verb.

```csharp
// Request.cs
public record struct Request(
    [FromServices] AppDbContext Database,
    [FromBody] RequestBody Body
);

public class RequestBody
{
    public string Term { get; set; }
}

public class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Body.Term).NotEmpty();
    }
}

// Response.cs: only ResponseItem, plus a HypermediaProvider<IEnumerable<ResponseItem>> if hypermedia is group-wide

// Handler.cs
public record Handler() : QueryHandlerAsync<Request>("/things/search")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .Produces<List<ResponseItem>>(StatusCodes.Status200OK)
                .WithName("SearchThings")
                .WithTags("Things")
                .ExcludeFromDescription()
                .WithValidation();

    protected override async Task<IResult> HandleAsync(Request req, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var items = await req.Database.Things
                             .Where(t => t.Name.Contains(req.Body.Term))
                             .Select(t => new ResponseItem { Id = t.Id, Name = t.Name })
                             .ToListAsync(cancellationToken);

        return Results.Ok(items);
    }
}
```
