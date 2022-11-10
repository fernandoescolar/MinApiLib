namespace MinApiLib.Hypermedia;

public static class HypermediaServiceCollectionExtentions
{
    public static IServiceCollection AddHypermedia(this IServiceCollection services, Assembly assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        assembly.GetHypermediaProviders().ToList().ForEach(t => services.AddScoped(typeof(IHypermediaProvider), t));
        services.AddScoped<HypermediaConverter>();

        return services;
    }

    private static IEnumerable<Type> GetHypermediaProviders(this Assembly assembly)
    {
        var types = assembly.GetTypes();
        var providers = types.Where(t => t.IsClass && !t.IsAbstract && typeof(IHypermediaProvider).IsAssignableFrom(t));
        return providers;
    }
}
