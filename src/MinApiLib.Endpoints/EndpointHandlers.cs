namespace MinApiLib.Endpoints;

public abstract record EndpointHandler(string[] Verbs, string Path) : IEndpoint
{
    public EndpointHandler(string verb, string path) : this(new[] { verb }, path) { }

    public RouteHandlerBuilder Configure(IEndpointRouteBuilder builder)
        => Configure(builder.MapMethods(Path, Verbs, InternalHandler));

    protected virtual RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder;

    protected abstract IResult Handle();

    private IResult InternalHandler()
        => Handle();
}

public abstract record DeleteHandler(string Path) : EndpointHandler(Constants.Delete, Path);
public abstract record GetHandler(string Path) : EndpointHandler(Constants.Get, Path);
public abstract record PatchHandler(string Path) : EndpointHandler(Constants.Patch, Path);
public abstract record PostHandler(string Path) : EndpointHandler(Constants.Post, Path);
public abstract record PutHandler(string Path) : EndpointHandler(Constants.Put, Path);
