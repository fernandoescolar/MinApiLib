using System.Runtime.CompilerServices;

namespace MinApiLib.Endpoints;

public static class EndpointsExtensions
{
    public static RouteHandlerBuilder MapEndpoint<T>(this IEndpointRouteBuilder endpoints) where T : IEndpoint, new()
        => new T().Configure(endpoints);

    [MethodImpl(MethodImplOptions.NoInlining)]
    [RequiresUnreferencedCode("Use 'MapAotEndpoints' instead. You should install MinApiLib.Endpoints.Aot source generators to use it.")]
    public static RouteGroupBuilder MapEndpoints(this IEndpointRouteBuilder endpoints, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();

        var logger = endpoints.ServiceProvider.GetRequiredService<ILogger<IEndpoint>>();
        var group = endpoints.MapGroup("/");
        foreach (var t in GetEndpoints(assembly))
        {
            logger.LogDebug("Mapping endpoint {Endpoint}", t.FullName);
            var endpoint = (IEndpoint)ActivatorUtilities.CreateInstance(endpoints.ServiceProvider, t);
            endpoint.Configure(group);
        }

        return group;
    }

    [RequiresUnreferencedCode("This method is called from 'MapEndpoints', use 'MapAotEndpoints' instead. You should install MinApiLib.Endpoints.Aot source generators to use it.")]
    private static IEnumerable<Type> GetEndpoints(Assembly assembly)
        => assembly.GetTypes()
                   .Where(t => t.IsClass && !t.IsAbstract && !t.ContainsGenericParameters && typeof(IEndpoint).IsAssignableFrom(t));
}
