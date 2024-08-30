namespace MinApiLib.FluentValidation;

public class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var argToValidate = context.GetArgument<T>(0);
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        if (validator is null)
        {
            return await next.Invoke(context);
        }

        var validationResult = await validator.ValidateAsync(argToValidate!);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.ToDictionary());
        }

        return await next.Invoke(context);
    }
}