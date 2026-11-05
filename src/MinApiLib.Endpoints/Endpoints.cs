namespace MinApiLib.Endpoints;

public abstract record Endpoint(string[] Verbs, string Path) : IEndpoint
{
    public Endpoint(string verb, string path) : this(new[] { verb }, path) { }

    public RouteHandlerBuilder Configure(IEndpointRouteBuilder builder)
        => Configure(builder.MapNamingConventionEndpoint(this));

    protected virtual RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder;
}

public abstract record Delete(string Path) : Endpoint(Constants.Delete, Path);
public abstract record Get(string Path) : Endpoint(Constants.Get, Path);
public abstract record Patch(string Path) : Endpoint(Constants.Patch, Path);
public abstract record Post(string Path) : Endpoint(Constants.Post, Path);
public abstract record Put(string Path) : Endpoint(Constants.Put, Path);
public abstract record Query(string Path) : Endpoint(Constants.Query, Path);

internal static class NamingConventionUtilities
{
    public static RouteHandlerBuilder MapNamingConventionEndpoint(this IEndpointRouteBuilder builder, Endpoint endpoint)
    {
        var methods = endpoint.GetType().GetMethods().Where(m => Constants.MethodNames.Contains(m.Name)).ToArray();
        if (methods.Length == 0)
        {
            throw new InvalidOperationException($"Endpoint {endpoint.GetType().FullName} does not have a public Handle or HandleAsync method.");
        }

        if (methods.Length > 1)
        {
            throw new InvalidOperationException($"Endpoint {endpoint.GetType().FullName} has {methods.Length} Handle or HandleAsync methods, and it must have only one.");
        }

        var @delegate = CreateHandlerDelegate(endpoint, methods[0]);
        return builder.MapMethods(endpoint.Path, endpoint.Verbs, @delegate);
    }

    private static Delegate CreateHandlerDelegate(object target, MethodInfo methodInfo)
    {
        Func<Type[], Type> getType;
        var isAction = methodInfo.ReturnType.Equals((typeof(void)));
        var types = methodInfo.GetParameters().Select(p => p.ParameterType);
        if (isAction)
        {
            getType = Expression.GetActionType;
        }
        else
        {
            getType = Expression.GetFuncType;
            types = types.Concat(new[] { methodInfo.ReturnType });
        }

        if (methodInfo.IsStatic)
        {
            return Delegate.CreateDelegate(getType(types.ToArray()), methodInfo);
        }

        return Delegate.CreateDelegate(getType(types.ToArray()), target, methodInfo);
    }
}