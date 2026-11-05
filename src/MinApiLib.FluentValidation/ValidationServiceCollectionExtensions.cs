using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MinApiLib.FluentValidation;

public static class ValidationServiceCollectionExtensions
{
    public static IServiceCollection AddValidationFilter(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        var serviceDescriptor = ServiceDescriptor.Describe(typeof(ValidationFilter<>), typeof(ValidationFilter<>), lifetime);
        services.TryAdd(serviceDescriptor);

        return services;
    }

    [Obsolete("Use AddValidationFilter() instead.")]
#if NET10_0_OR_GREATER
    // ASP.NET Core 10 has its own AddValidation(): an optional parameter here would make calls to it ambiguous.
    public static IServiceCollection AddValidation(this IServiceCollection services, ServiceLifetime lifetime)
#else
    public static IServiceCollection AddValidation(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
#endif
        => services.AddValidationFilter(lifetime);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static IServiceCollection AddValidatorsFromAssembly(this IServiceCollection services, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        foreach (var type in assembly.GetTypes().Where(x => !x.IsAbstract && !x.IsInterface && !x.ContainsGenericParameters))
        {
            foreach (var interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IValidator<>))
                {
                    services.AddTransient(interfaceType, type);
                }
            }
        }

        services.AddValidationFilter();
        return services;
    }
}
