namespace MinApiLib.DependencyInjection;

public static class DependencyInjectionServiceCollectionExtensions
{
    public static IServiceCollection AddAssembly(this IServiceCollection services, Assembly assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        assembly.GetTypes()
                .Where(x => !x.IsAbstract && !x.IsInterface)
                .Select(x => new { Type = x, Attribute = x.GetCustomAttribute<RegisterInIServiceCollectionAttribute>() })
                .Where(x => x.Attribute != null)
                .ToList()
                .ForEach(x => services.Add(new ServiceDescriptor(x.Attribute.Type ?? x.Type, x.Type, x.Attribute.ServiceLifetime)));

        return services;
    }
}