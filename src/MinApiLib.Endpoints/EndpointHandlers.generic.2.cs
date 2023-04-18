namespace MinApiLib.Endpoints;

public abstract record EndpointHandler<TRequest, TResponse>(string[] Verbs, string Path) : IEndpoint
{
    public EndpointHandler(string verb, string path) : this(new[] { verb }, path) { }

    public RouteHandlerBuilder Configure(IEndpointRouteBuilder builder)
        => Configure(builder.MapMethods(Path, Verbs, Delegate));

    public TResponse Delegate([AsParameters]TRequest request)
        => Handle(request);

    protected virtual RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder;

    protected abstract TResponse Handle(TRequest request);
}

public abstract record DeleteHandler<TRequest, TResponse>(string Path) : EndpointHandler<TRequest, TResponse>(Constants.Delete, Path);
public abstract record GetHandler<TRequest, TResponse>(string Path) : EndpointHandler<TRequest, TResponse>(Constants.Get, Path);
public abstract record PatchHandler<TRequest, TResponse>(string Path) : EndpointHandler<TRequest, TResponse>(Constants.Patch, Path);
public abstract record PostHandler<TRequest, TResponse>(string Path) : EndpointHandler<TRequest, TResponse>(Constants.Post, Path);
public abstract record PutHandler<TRequest, TResponse>(string Path) : EndpointHandler<TRequest, TResponse>(Constants.Put, Path);
