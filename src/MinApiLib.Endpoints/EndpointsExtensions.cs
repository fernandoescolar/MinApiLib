namespace MinApiLib.Endpoints;

public static class EndpointsExtensions
{
    public static RouteGroupBuilder MapEndpoints(this IEndpointRouteBuilder endpoints, Assembly assembly = null)
    {
        var group = endpoints.MapGroup("/");
        foreach (var t in GetEndpoints(assembly ?? Assembly.GetCallingAssembly()))
        {
            group.MapEndpoint(t);
        }

        return group;
    }

    private static RouteHandlerBuilder MapEndpoint(this RouteGroupBuilder builder, Type type)
    {
        if (!type.GetConstructors().Any(c => c.GetParameters().Length == 0))
        {
            throw new InvalidOperationException($"Endpoint type {type.FullName} must have a parameterless constructor.");
        }

        var delegateMethod = type.GetMethods().Where(m => Constants.MethodNames.Contains(m.Name)).FirstOrDefault();
        if (delegateMethod == null)
        {
            throw new Exception($"Endpoint {type.Name} does not have a Handle or HandleAsync method.");
        }

        var endpoint = Activator.CreateInstance(type, new object[] { }) as Endpoint;
        var @delegate = CreateDelegate(endpoint, delegateMethod);
        var routeHanderBuilder = builder.MapMethods(endpoint.Path, endpoint.Verbs, @delegate);
        endpoint.Configure(routeHanderBuilder);

        return routeHanderBuilder;
    }

    private static IEnumerable<Type> GetEndpoints(Assembly assembly)
    {
        var types = assembly.GetTypes();
        var endpoints = types.Where(t => t.IsClass && !t.IsAbstract && typeof(Endpoint).IsAssignableFrom(t));
        foreach(var endpoint in endpoints)
        {
            Console.WriteLine($"Found endpoint: {endpoint.FullName}");
        }
        if (!endpoints.Any())
        {
            Console.WriteLine($"No endpoints found in assembly {assembly.FullName}.");
        }

        return endpoints;
    }

    public static Delegate CreateDelegate(object target, MethodInfo methodInfo)
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
