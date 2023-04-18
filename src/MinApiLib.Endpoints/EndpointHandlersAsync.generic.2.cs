namespace MinApiLib.Endpoints;

public abstract record EndpointHandlerAsync<TRequest, TResponse>(string[] Verbs, string Path) : IEndpoint
{
    public EndpointHandlerAsync(string verb, string path) : this(new[] { verb }, path) { }

    public RouteHandlerBuilder Configure(IEndpointRouteBuilder builder)
        => Configure(builder.MapMethods(Path, Verbs, DelegateAsync));

    public Task<TResponse> DelegateAsync([AsParameters]TRequest request, CancellationToken cancellationToken)
        => HandleAsync(request, cancellationToken);

    protected virtual RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder;

    protected abstract Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}

public abstract record DeleteHandlerAsync<TRequest, TResponse>(string Path) : EndpointHandlerAsync<TRequest, TResponse>(Constants.Delete, Path);
public abstract record GetHandlerAsync<TRequest, TResponse>(string Path) : EndpointHandlerAsync<TRequest, TResponse>(Constants.Get, Path);
public abstract record PatchHandlerAsync<TRequest, TResponse>(string Path) : EndpointHandlerAsync<TRequest, TResponse>(Constants.Patch, Path);
public abstract record PostHandlerAsync<TRequest, TResponse>(string Path) : EndpointHandlerAsync<TRequest, TResponse>(Constants.Post, Path);
public abstract record PutHandlerAsync<TRequest, TResponse>(string Path) : EndpointHandlerAsync<TRequest, TResponse>(Constants.Put, Path);
