using System.Runtime.CompilerServices;

namespace MinApiLib.DependencyInjection;

public static class DependencyInjectionServiceCollectionExtensions
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static IServiceCollection AddAssembly(this IServiceCollection services, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        foreach (var type in assembly.GetTypes().Where(x => !x.IsAbstract && !x.IsInterface))
        {
            var attribute = type.GetCustomAttribute<RegisterInIServiceCollectionAttribute>();
            if (attribute is not null)
            {
                services.AddRegisterInIServiceCollectionAttribute(type, attribute);
            }
        }

        return services;
    }

    private static IServiceCollection AddRegisterInIServiceCollectionAttribute(this IServiceCollection services, Type t, RegisterInIServiceCollectionAttribute attribute)
    {
        var types = attribute.Types.ToArray();
        if (types.Length == 0)
        {
            services.Add(new ServiceDescriptor(t, t, attribute.ServiceLifetime));
            return services;
        }

        if (types.Length == 1)
        {
            services.Add(new ServiceDescriptor(types[0], t, attribute.ServiceLifetime));
            return services;
        }

        services.Add(new ServiceDescriptor(t, t, attribute.ServiceLifetime));
        foreach (var type in types)
        {
            services.Add(new ServiceDescriptor(type, sp => sp.GetRequiredService(t), ServiceLifetime.Transient));
        }

        return services;
    }
}
