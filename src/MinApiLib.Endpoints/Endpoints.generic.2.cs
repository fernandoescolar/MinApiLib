namespace MinApiLib.Endpoints;

public abstract record Endpoint<TRequest, TResponse>(string[] Verbs, string Path) : IEndpoint
{
    public Endpoint(string verb, string path) : this(new[] { verb }, path) { }

    public RouteHandlerBuilder Configure(IEndpointRouteBuilder builder)
        => Configure(builder.MapMethods(Path, Verbs, InternalHandler));

    protected virtual RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder;

    protected abstract TResponse Handle(TRequest request);

    private TResponse InternalHandler([AsParameters]TRequest request)
        => Handle(request);
}

public abstract record Delete<TRequest, TResponse>(string Path) : Endpoint<TRequest, TResponse>(Constants.Delete, Path);
public abstract record Get<TRequest, TResponse>(string Path) : Endpoint<TRequest, TResponse>(Constants.Get, Path);
public abstract record Patch<TRequest, TResponse>(string Path) : Endpoint<TRequest, TResponse>(Constants.Patch, Path);
public abstract record Post<TRequest, TResponse>(string Path) : Endpoint<TRequest, TResponse>(Constants.Post, Path);
public abstract record Put<TRequest, TResponse>(string Path) : Endpoint<TRequest, TResponse>(Constants.Put, Path);
