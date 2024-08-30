namespace MinApiLib.FluentValidation;

public static class ValidationExtensions
{
    public static RouteHandlerBuilder ProducesValidationError(this RouteHandlerBuilder builder)
    {
        builder.Produces<IDictionary<string, string[]>>(StatusCodes.Status400BadRequest);
        return builder;
    }

    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder)
        => builder.AddEndpointFilter<ValidationFilter<T>>()
                  .ProducesValidationError();

    public static RouteHandlerBuilder WithValidation(this RouteHandlerBuilder builder)
        => builder.AddEndpointFilterFactory((context, next) =>
            {
                var argType = context.MethodInfo.GetParameters().FirstOrDefault()?.ParameterType;
                if (argType is null)
                {
                    return invocationContext => next(invocationContext);
                }

                var filterType = typeof(ValidationFilter<>).MakeGenericType(argType);
                var filter = (IEndpointFilter)context.ApplicationServices.GetRequiredService(filterType);
                return invocationContext => filter.InvokeAsync(invocationContext, next);
            })
            .ProducesValidationError();
}