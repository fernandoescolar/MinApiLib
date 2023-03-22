namespace MinApiLib.Endpoints;

public abstract record EndpointHandlerAsync<TRequest>(string[] Verbs, string Path) : EndpointHandlerAsync<TRequest, IResult>(Verbs, Path)
{
    public EndpointHandlerAsync(string verb, string path) : this(new[] { verb }, path) { }
}

public abstract record DeleteHandlerAsync<TRequest>(string Path) : EndpointHandlerAsync<TRequest>(Constants.Delete, Path);
public abstract record GetHandlerAsync<TRequest>(string Path) : EndpointHandlerAsync<TRequest>(Constants.Get, Path);
public abstract record PatchHandlerAsync<TRequest>(string Path) : EndpointHandlerAsync<TRequest>(Constants.Patch, Path);
public abstract record PostHandlerAsync<TRequest>(string Path) : EndpointHandlerAsync<TRequest>(Constants.Post, Path);
public abstract record PutHandlerAsync<TRequest>(string Path) : EndpointHandlerAsync<TRequest>(Constants.Put, Path);
