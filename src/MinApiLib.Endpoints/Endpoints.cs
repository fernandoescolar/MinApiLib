namespace MinApiLib.Endpoints;

public abstract record Endpoint(string[] Verbs, string Path) : IEndpoint
{
    public Endpoint(string verb, string path) : this(new[] { verb }, path) { }

    public RouteHandlerBuilder Configure(IEndpointRouteBuilder builder)
        => Configure(builder.MapMethods(Path, Verbs, InternalHandler));

    protected virtual RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder;

    protected abstract IResult Handle();

    private IResult InternalHandler()
        => Handle();
}

public abstract record Delete(string Path) : Endpoint(Constants.Delete, Path);
public abstract record Get(string Path) : Endpoint(Constants.Get, Path);
public abstract record Patch(string Path) : Endpoint(Constants.Patch, Path);
public abstract record Post(string Path) : Endpoint(Constants.Post, Path);
public abstract record Put(string Path) : Endpoint(Constants.Put, Path);
