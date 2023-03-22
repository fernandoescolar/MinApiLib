namespace MinApiLib.Endpoints;

public abstract record EndpointHandlerAsync(string[] Verbs, string Path) : IEndpoint
{
    public EndpointHandlerAsync(string verb, string path) : this(new[] { verb }, path) { }

    public RouteHandlerBuilder Configure(IEndpointRouteBuilder builder)
        => Configure(builder.MapMethods(Path, Verbs, InternalHandlerAsync));

    protected virtual RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder;

    protected abstract Task<IResult> HandleAsync(CancellationToken cancellationToken);

    private Task<IResult> InternalHandlerAsync(CancellationToken cancellationToken)
        => HandleAsync(cancellationToken);
}

public abstract record DeleteHandlerAsync(string Path) : EndpointHandlerAsync(Constants.Delete, Path);
public abstract record GetHandlerAsync(string Path) : EndpointHandlerAsync(Constants.Get, Path);
public abstract record PatchHandlerAsync(string Path) : EndpointHandlerAsync(Constants.Patch, Path);
public abstract record PostHandlerAsync(string Path) : EndpointHandlerAsync(Constants.Post, Path);
public abstract record PutHandlerAsync(string Path) : EndpointHandlerAsync(Constants.Put, Path);
