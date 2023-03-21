namespace MinApiLib.Endpoints;

public abstract record AsyncEndpoint(string[] Verbs, string Path) : IEndpoint
{
    public AsyncEndpoint(string verb, string path) : this(new[] { verb }, path) { }

    public RouteHandlerBuilder Configure(IEndpointRouteBuilder builder)
        => Configure(builder.MapMethods(Path, Verbs, InternalHandlerAsync));

    protected virtual RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder;

    protected abstract IResult HandleAsync(CancellationToken cancellationToken);

    private IResult InternalHandlerAsync(CancellationToken cancellationToken)
        => HandleAsync(cancellationToken);
}

public abstract record DeleteAsync(string Path) : AsyncEndpoint(Constants.Delete, Path);
public abstract record GetAsync(string Path) : AsyncEndpoint(Constants.Get, Path);
public abstract record PatchAsync(string Path) : AsyncEndpoint(Constants.Patch, Path);
public abstract record PostAsync(string Path) : AsyncEndpoint(Constants.Post, Path);
public abstract record PutAsync(string Path) : AsyncEndpoint(Constants.Put, Path);
