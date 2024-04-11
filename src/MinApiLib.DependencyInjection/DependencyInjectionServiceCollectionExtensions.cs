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
                .ForEach(x => services.AddRegisterInIServiceCollectionAttribute(x.Type, x.Attribute));

        return services;
    }

    private static IServiceCollection AddRegisterInIServiceCollectionAttribute(this IServiceCollection services, Type t, RegisterInIServiceCollectionAttribute attribute)
    {
        if (attribute == null)
        {
            throw new ArgumentNullException(nameof(attribute));
        }

        if (attribute.Types is null || attribute.Types.Count() == 0)
        {
            services.Add(new ServiceDescriptor(t, t, attribute.ServiceLifetime));
            return services;
        }

        if (attribute.Types.Count() == 1)
        {
            services.Add(new ServiceDescriptor(attribute.Types.First(), t, attribute.ServiceLifetime));
            return services;
        }

        services.Add(new ServiceDescriptor(t, t, attribute.ServiceLifetime));
        attribute.Types.ToList().ForEach(x => services.Add(new ServiceDescriptor(x, sp => sp.GetRequiredService(t), ServiceLifetime.Transient)));

        return services;
    }
}