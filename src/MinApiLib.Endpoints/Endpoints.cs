namespace MinApiLib.Endpoints;

public abstract record Endpoint(string[] Verbs, string Path)
{
    public Endpoint(string verb, string path) : this(new[] { verb }, path) { }

    public void Configure(RouteHandlerBuilder builder) => OnConfigure(builder);

    protected virtual void OnConfigure(RouteHandlerBuilder builder) { }
}

public abstract record GetEndpoint(string path) : Endpoint(Constants.Get, path)
{
}

public abstract record PostEndpoint(string path) : Endpoint(Constants.Post, path)
{
}

public abstract record PutEndpoint(string path) : Endpoint(Constants.Put, path)
{
}

public abstract record DeleteEndpoint(string path) : Endpoint(Constants.Delete, path)
{
}

public abstract record PatchEndpoint(string path) : Endpoint(Constants.Patch, path)
{
}
