namespace MinApiLib.FluentValidation;

public class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var arguments = context.Arguments.OfType<T>().ToArray();
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        if (arguments.Length == 0 || validator is null)
        {
            return await next.Invoke(context);
        }

        var validationResult = await validator.ValidateAsync(arguments[0], context.HttpContext.RequestAborted);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.ToDictionary());
        }

        return await next.Invoke(context);
    }
}
