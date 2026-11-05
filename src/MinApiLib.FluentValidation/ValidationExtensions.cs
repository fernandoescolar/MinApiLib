namespace MinApiLib.FluentValidation;

public static class ValidationExtensions
{
    public static RouteHandlerBuilder ProducesValidationError(this RouteHandlerBuilder builder)
    {
        builder.Produces<IDictionary<string, string[]>>(StatusCodes.Status400BadRequest);
        return builder;
    }

    /// <summary>
    /// Validates the handler parameter of type <typeparamref name="T"/>.
    /// Without a validator, requests aren't validated and a warning is logged: this is obsolete, and it will throw in a future version.
    /// </summary>
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder)
        => builder.AddEndpointFilterFactory((context, next) =>
            {
                if (!context.MethodInfo.GetParameters().Any(x => x.ParameterType == typeof(T)))
                {
                    throw new InvalidOperationException($"{context.MethodInfo.DeclaringType?.FullName} has no parameter of type {typeof(T).FullName} to validate.");
                }

                EnsureFilterIsRegistered(context);
                if (!HasValidator(context, typeof(T)))
                {
                    LogMissingValidator(context);
                    return next;
                }

                return CreateFilter(context, next, typeof(T));
            })
            .ProducesValidationError();

    /// <summary>
    /// Validates the first handler parameter that has a validator.
    /// Without a validator, requests aren't validated and a warning is logged: this is obsolete, and it will throw in a future version.
    /// </summary>
    public static RouteHandlerBuilder WithValidation(this RouteHandlerBuilder builder)
        => builder.AddEndpointFilterFactory((context, next) =>
            {
                EnsureFilterIsRegistered(context);
                var argumentType = FindValidatedType(context);
                if (argumentType is null)
                {
                    LogMissingValidator(context);
                    return next;
                }

                return CreateFilter(context, next, argumentType);
            })
            .ProducesValidationError();

    /// <summary>Validates the first parameter that has a validator in each endpoint of the group. Endpoints with no validator aren't validated.</summary>
    public static RouteGroupBuilder WithValidation(this RouteGroupBuilder builder)
    {
        builder.AddEndpointFilterFactory((context, next) =>
        {
            var argumentType = FindValidatedType(context);
            return argumentType is null ? next : CreateFilter(context, next, argumentType);
        });

        return builder;
    }

    private static Type? FindValidatedType(EndpointFilterFactoryContext context)
        => context.MethodInfo
                  .GetParameters()
                  .Select(x => x.ParameterType)
                  .FirstOrDefault(x => HasValidator(context, x));

    private static bool HasValidator(EndpointFilterFactoryContext context, Type argumentType)
        => context.ApplicationServices
                  .GetRequiredService<IServiceProviderIsService>()
                  .IsService(typeof(IValidator<>).MakeGenericType(argumentType));

    private static void EnsureFilterIsRegistered(EndpointFilterFactoryContext context)
    {
        var isRegistered = context.ApplicationServices
                                  .GetRequiredService<IServiceProviderIsService>()
                                  .IsService(typeof(ValidationFilter<object>));
        if (!isRegistered)
        {
            throw new InvalidOperationException("The validation filter is not registered: call services.AddValidatorsFromAssembly() or services.AddValidationFilter().");
        }
    }

    private static void LogMissingValidator(EndpointFilterFactoryContext context)
    {
        var logger = context.ApplicationServices
                            .GetService<ILoggerFactory>()?
                            .CreateLogger(typeof(ValidationExtensions).FullName!);

        logger?.LogWarning(
            "{Endpoint} calls WithValidation(), but none of its parameters has a registered IValidator<T>, so its requests aren't validated. The validator must target the type the handler receives. Calling WithValidation() without a validator is obsolete, and it will throw in a future version.",
            context.MethodInfo.DeclaringType?.FullName);
    }

    private static EndpointFilterDelegate CreateFilter(EndpointFilterFactoryContext context, EndpointFilterDelegate next, Type argumentType)
    {
        EnsureFilterIsRegistered(context);

        var filterType = typeof(ValidationFilter<>).MakeGenericType(argumentType);
        var filter = (IEndpointFilter)context.ApplicationServices.GetRequiredService(filterType);

        return invocationContext => filter.InvokeAsync(invocationContext, next);
    }
}
