namespace MinApiLib.Hypermedia.Tests;

public record Thing(int Id, string Name);
public record struct ThingStruct(int Id);
public record ThingItem(int Id);
public record ThingWithService(int Id);
public record NoProvider(string Name);
public record struct IdRequest([FromRoute] int Id);

public class ThingProvider : HypermediaProvider<Thing>
{
    protected override IEnumerable<HypermediaLink> GetLinksFor(Thing @object)
    {
        yield return new HypermediaLink("self", "/things/" + @object.Id, "GET");
        yield return new HypermediaLink("docs", "https://docs.example.com/things", "GET");
    }
}

public class ThingStructProvider : HypermediaProvider<ThingStruct>
{
    protected override IEnumerable<HypermediaLink> GetLinksFor(ThingStruct @object)
    {
        yield return new HypermediaLink("self", "/structs/" + @object.Id, "GET");
    }
}

public class ThingListProvider : HypermediaProvider<IEnumerable<ThingItem>>
{
    protected override IEnumerable<HypermediaLink> GetLinksFor(IEnumerable<ThingItem> @object)
    {
        yield return new HypermediaLink("self", "/list", "GET");
    }
}

public class LinkSource
{
    public string Path => "/from-scoped-service";
}

public class ThingWithServiceProvider : HypermediaProvider<ThingWithService>
{
    private readonly LinkSource _source;

    public ThingWithServiceProvider(LinkSource source)
    {
        _source = source;
    }

    protected override IEnumerable<HypermediaLink> GetLinksFor(ThingWithService @object)
    {
        yield return new HypermediaLink("self", _source.Path, "GET");
    }
}

public record GetThing() : GetHandler<IdRequest>("/things/{id}")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder.WithName("GetThing");

    protected override IResult Handle(IdRequest request)
        => request.Id == 0 ? Results.NotFound() : Results.Ok(new Thing(request.Id, "thing"));
}

public record CreateThing() : PostHandler("/things")
{
    protected override IResult Handle()
        => Results.Created("/things/1", new Thing(1, "created"));
}

public record CreateThingAtRoute() : PostHandler("/things-at-route")
{
    protected override IResult Handle()
        => Results.CreatedAtRoute("GetThing", new { id = 7 }, new Thing(7, "created"));
}

public record AcceptThing() : PostHandler("/things-accepted")
{
    protected override IResult Handle()
        => Results.Accepted("/things/1/status", new Thing(1, "accepted"));
}

public record GetTypedThing() : GetHandler<IdRequest, Thing>("/typed/{id}")
{
    protected override Thing Handle(IdRequest request)
        => new Thing(request.Id, "typed");
}

public record GetTypedThingAsync() : GetHandlerAsync<IdRequest, Thing>("/typed-async/{id}")
{
    protected override Task<Thing> HandleAsync(IdRequest request, CancellationToken cancellationToken)
        => Task.FromResult(new Thing(request.Id, "typed-async"));
}

public record GetConventionThing() : Get("/convention")
{
    public Thing Handle()
        => new Thing(1, "convention");
}

public record GetJsonThing() : GetHandler("/json")
{
    protected override IResult Handle()
        => Results.Json(new Thing(1, "json"));
}

public record GetStruct() : GetHandler("/struct")
{
    protected override IResult Handle()
        => Results.Ok(new ThingStruct(1));
}

public record GetList() : GetHandler("/list")
{
    protected override IResult Handle()
        => Results.Ok(new List<ThingItem> { new ThingItem(1) });
}

public record GetThingWithService() : GetHandler("/with-service")
{
    protected override IResult Handle()
        => Results.Ok(new ThingWithService(1));
}

public record GetNoProvider() : GetHandler("/no-provider")
{
    protected override IResult Handle()
        => Results.Ok(new NoProvider("no links"));
}

public record GetNoProviderTyped() : GetHandler<IdRequest, NoProvider>("/no-provider-typed/{id}")
{
    protected override NoProvider Handle(IdRequest request)
        => new NoProvider("no links");
}

public record GetNoContent() : GetHandler("/no-content")
{
    protected override IResult Handle()
        => Results.NoContent();
}

public record GetBadRequest() : GetHandler("/bad-request")
{
    protected override IResult Handle()
        => Results.BadRequest(new Thing(1, "invalid"));
}

public record GetOkWithoutValue() : GetHandler("/ok-without-value")
{
    protected override IResult Handle()
        => Results.Ok();
}

public record GetCustomResult() : GetHandler("/custom-result")
{
    protected override IResult Handle()
        => new ResultWithHeader(new Thing(1, "custom"));
}

public record GetDocumented() : GetHandler("/documented")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<Thing>(StatusCodes.Status200OK)
                .Produces<Thing>(StatusCodes.Status200OK)
                .WithName("GetDocumented");

    protected override IResult Handle()
        => Results.Ok(new Thing(1, "documented"));
}

public class ResultWithHeader : IResult, IValueHttpResult, IStatusCodeHttpResult
{
    public ResultWithHeader(object value)
    {
        Value = value;
    }

    public object? Value { get; }
    public int? StatusCode => StatusCodes.Status200OK;

    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.Headers.ETag = "\"v1\"";
        return httpContext.Response.WriteAsJsonAsync(Value);
    }
}
