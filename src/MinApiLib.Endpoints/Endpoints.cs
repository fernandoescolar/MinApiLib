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

internal static class NamingConventionUtilities
{
    public static RouteHandlerBuilder MapNamingConventionEndpoint(this IEndpointRouteBuilder builder, Endpoint endpoint)
    {
        var delegateMethod = endpoint.GetType().GetMethods().Where(m => Constants.MethodNames.Contains(m.Name)).FirstOrDefault();
        if (delegateMethod == null)
        {
            throw new Exception($"Endpoint {endpoint.GetType().FullName} does not have a Handle or HandleAsync method.");
        }

        var @delegate = CreateHandlerDelegate(endpoint, delegateMethod);
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

        return Delegate.CreateDelegate(getType(types.ToArray()), target, methodInfo.Name);
    }
}