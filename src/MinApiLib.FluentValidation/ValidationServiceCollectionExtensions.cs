using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MinApiLib.FluentValidation;

public static class ValidationServiceCollectionExtensions
{
    public static IServiceCollection AddValidation(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        var serviceDescriptor = ServiceDescriptor.Describe(typeof(ValidationFilter<>), typeof(ValidationFilter<>), lifetime);
        services.TryAdd(serviceDescriptor);

        return services;
    }

    public static IServiceCollection AddValidatorsFromAssembly(this IServiceCollection services, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        foreach(var type in assembly.GetTypes().Where(x => !x.IsAbstract && !x.IsInterface))
        {
            foreach(var interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IValidator<>))
                {
                    services.AddTransient(interfaceType, type);
                }
            }
        }

        services.AddValidation();
        return services;
    }
}