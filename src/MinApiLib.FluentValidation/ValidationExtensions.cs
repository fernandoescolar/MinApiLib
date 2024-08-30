using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MinApiLib.FluentValidation;

public static class ValidationExtensions
{
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<ValidationFilter<T>>();
    }

    public static RouteHandlerBuilder WithValidation(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilterFactory((context, next) =>
        {
            var argType = context.MethodInfo.GetParameters().FirstOrDefault()?.ParameterType;
            if (argType is null)
            {
                return invocationContext => next(invocationContext);
            }

            var filterType = typeof(ValidationFilter<>).MakeGenericType(argType);
            var filter = (IEndpointFilter)context.ApplicationServices.GetRequiredService(filterType);
            return invocationContext => filter.InvokeAsync(invocationContext, next);
        });
    }

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