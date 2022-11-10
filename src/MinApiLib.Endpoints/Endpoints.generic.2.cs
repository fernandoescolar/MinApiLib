namespace MinApiLib.Endpoints;

public abstract record Endpoint<TInput, TOutput>(string[] Verbs, string Path) : Endpoint(Verbs, Path)
{
    public Endpoint(string verb, string path) : this(new[] { verb }, path) { }

    public Task<TOutput> HandleAsync([AsParameters]TInput request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return OnHandleAsync(request, cancellationToken);
    }

    protected abstract Task<TOutput> OnHandleAsync(TInput request, CancellationToken cancellationToken);
}

public abstract record GetEndpoint<TInput, TOutput>(string path) : Endpoint<TInput, TOutput>(Constants.Get, path)
{
}

public abstract record PostEndpoint<TInput, TOutput>(string path) : Endpoint<TInput, TOutput>(Constants.Post, path)
{
}

public abstract record PutEndpoint<TInput, TOutput>(string path) : Endpoint<TInput, TOutput>(Constants.Put, path)
{
}

public abstract record DeleteEndpoint<TInput, TOutput>(string path) : Endpoint<TInput, TOutput>(Constants.Delete, path)
{
}

public abstract record PatchEndpoint<TInput, TOutput>(string path) : Endpoint<TInput, TOutput>(Constants.Patch, path)
{
}
