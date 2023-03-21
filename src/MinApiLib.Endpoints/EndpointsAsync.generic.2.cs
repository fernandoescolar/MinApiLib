namespace MinApiLib.Endpoints;

public abstract record AsyncEndpoint<TRequest, TResponse>(string[] Verbs, string Path) : IEndpoint
{
    public AsyncEndpoint(string verb, string path) : this(new[] { verb }, path) { }

    public RouteHandlerBuilder Configure(IEndpointRouteBuilder builder)
        => Configure(builder.MapMethods(Path, Verbs, InternalHandlerAsync));

    protected virtual RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder;

    protected abstract Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);

    private Task<TResponse> InternalHandlerAsync([AsParameters]TRequest request, CancellationToken cancellationToken)
        => HandleAsync(request, cancellationToken);
}

public abstract record DeleteAsync<TRequest, TResponse>(string Path) : AsyncEndpoint<TRequest, TResponse>(Constants.Delete, Path);
public abstract record GetAsync<TRequest, TResponse>(string Path) : AsyncEndpoint<TRequest, TResponse>(Constants.Get, Path);
public abstract record PatchAsync<TRequest, TResponse>(string Path) : AsyncEndpoint<TRequest, TResponse>(Constants.Patch, Path);
public abstract record PostAsync<TRequest, TResponse>(string Path) : AsyncEndpoint<TRequest, TResponse>(Constants.Post, Path);
public abstract record PutAsync<TRequest, TResponse>(string Path) : AsyncEndpoint<TRequest, TResponse>(Constants.Put, Path);
