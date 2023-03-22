namespace MinApiLib.Endpoints;

public abstract record EndpointHandler<TRequest>(string[] Verbs, string Path) : EndpointHandler<TRequest, IResult>(Verbs, Path)
{
    public EndpointHandler(string verb, string path) : this(new[] { verb }, path) { }
}

public abstract record DeleteHandler<TRequest>(string Path) : EndpointHandler<TRequest>(Constants.Delete, Path);
public abstract record GetHandler<TRequest>(string Path) : EndpointHandler<TRequest>(Constants.Get, Path);
public abstract record PatchHandler<TRequest>(string Path) : EndpointHandler<TRequest>(Constants.Patch, Path);
public abstract record PostHandler<TRequest>(string Path) : EndpointHandler<TRequest>(Constants.Post, Path);
public abstract record PutHandler<TRequest>(string Path) : EndpointHandler<TRequest>(Constants.Put, Path);
