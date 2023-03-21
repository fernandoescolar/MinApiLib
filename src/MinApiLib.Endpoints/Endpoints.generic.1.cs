namespace MinApiLib.Endpoints;

public abstract record Endpoint<TRequest>(string[] Verbs, string Path) : Endpoint<TRequest, IResult>(Verbs, Path)
{
    public Endpoint(string verb, string path) : this(new[] { verb }, path) { }
}

public abstract record Delete<TRequest>(string Path) : Endpoint<TRequest>(Constants.Delete, Path);
public abstract record Get<TRequest>(string Path) : Endpoint<TRequest>(Constants.Get, Path);
public abstract record Patch<TRequest>(string Path) : Endpoint<TRequest>(Constants.Patch, Path);
public abstract record Post<TRequest>(string Path) : Endpoint<TRequest>(Constants.Post, Path);
public abstract record Put<TRequest>(string Path) : Endpoint<TRequest>(Constants.Put, Path);
