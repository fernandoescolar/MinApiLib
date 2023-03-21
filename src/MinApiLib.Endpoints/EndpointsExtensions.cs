namespace MinApiLib.Endpoints;

public static class EndpointsExtensions
{
    public static RouteGroupBuilder MapEndpoints(this IEndpointRouteBuilder endpoints, Assembly assembly = null)
    {
        var logger = endpoints.ServiceProvider.GetRequiredService<ILogger<IEndpoint>>();
        var group = endpoints.MapGroup("/");
        foreach (var t in GetEndpoints(assembly ?? Assembly.GetCallingAssembly()))
        {
            logger.LogDebug("Mapping endpoint {Endpoint}", t.FullName);
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

        var endpoint = Activator.CreateInstance(type, new object[] { }) as IEndpoint;
        return endpoint.Configure(builder);
    }

    private static IEnumerable<Type> GetEndpoints(Assembly assembly)
        => assembly.GetTypes()
                   .Where(t => t.IsClass && !t.IsAbstract && typeof(IEndpoint).IsAssignableFrom(t));

}
