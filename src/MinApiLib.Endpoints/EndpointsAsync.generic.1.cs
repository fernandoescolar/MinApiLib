namespace MinApiLib.Endpoints;

public abstract record AsyncEndpoint<TRequest>(string[] Verbs, string Path) : AsyncEndpoint<TRequest, IResult>(Verbs, Path)
{
    public AsyncEndpoint(string verb, string path) : this(new[] { verb }, path) { }
}

public abstract record DeleteAsync<TRequest>(string Path) : AsyncEndpoint<TRequest>(Constants.Delete, Path);
public abstract record GetAsync<TRequest>(string Path) : AsyncEndpoint<TRequest>(Constants.Get, Path);
public abstract record PatchAsync<TRequest>(string Path) : AsyncEndpoint<TRequest>(Constants.Patch, Path);
public abstract record PostAsync<TRequest>(string Path) : AsyncEndpoint<TRequest>(Constants.Post, Path);
public abstract record PutAsync<TRequest>(string Path) : AsyncEndpoint<TRequest>(Constants.Put, Path);
