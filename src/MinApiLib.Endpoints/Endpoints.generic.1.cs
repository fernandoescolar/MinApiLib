namespace MinApiLib.Endpoints;

public abstract record Endpoint<TInput>(string[] Verbs, string Path) : Endpoint<TInput, IResult>(Verbs, Path)
{
    public Endpoint(string verb, string path) : this(new[] { verb }, path) { }
}

public abstract record GetEndpoint<TInput>(string path) : Endpoint<TInput>(Constants.Get, path)
{
}

public abstract record PostEndpoint<TInput>(string path) : Endpoint<TInput>(Constants.Post, path)
{
}

public abstract record PutEndpoint<TInput>(string path) : Endpoint<TInput>(Constants.Put, path)
{
}

public abstract record DeleteEndpoint<TInput>(string path) : Endpoint<TInput>(Constants.Delete, path)
{
}

public abstract record PatchEndpoint<TInput>(string path) : Endpoint<TInput>(Constants.Patch, path)
{
}
