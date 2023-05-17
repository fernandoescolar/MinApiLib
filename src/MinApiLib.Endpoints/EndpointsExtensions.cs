namespace MinApiLib.Endpoints;

public static class EndpointsExtensions
{
    private static List<IEndpoint> _endpoints = new();

    public static RouteGroupBuilder MapEndpoints(this IEndpointRouteBuilder endpoints, Assembly assembly = null)
    {
        _endpoints.Clear();

        var logger = endpoints.ServiceProvider.GetRequiredService<ILogger<IEndpoint>>();
        var group = endpoints.MapGroup("/");
        foreach (var t in GetEndpoints(assembly ?? Assembly.GetCallingAssembly()))
        {
            logger.LogDebug("Mapping endpoint {Endpoint}", t.FullName);
            var endpoint = ActivatorUtilities.CreateInstance(endpoints.ServiceProvider, t) as IEndpoint;
            endpoint.Configure(group);
            _endpoints.Add(endpoint);
        }

        return group;
    }

    private static IEnumerable<Type> GetEndpoints(Assembly assembly)
        => assembly.GetTypes()
                   .Where(t => t.IsClass && !t.IsAbstract && typeof(IEndpoint).IsAssignableFrom(t));

}
