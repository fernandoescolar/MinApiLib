namespace MinApiLib.Validation;

public class ValidationEndpointFilter : IEndpointFilter
{
    public ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        foreach (var argument in context.Arguments)
        {
            if (argument is null) continue;
            if (!argument.GetType().IsClass) continue;

            var bodyProperty = argument.GetType().GetProperties().FirstOrDefault(x => x.Name == "Body");
            if (bodyProperty is not null)
            {
                if (!MiniValidator.TryValidate(bodyProperty.GetValue(argument), out var errors))
                {
                    return ValueTask.FromResult<object>(Results.BadRequest(errors));
                }
            }
            else if (!MiniValidator.TryValidate(argument, out var errors))
            {
                return ValueTask.FromResult<object>(Results.BadRequest(errors));
            }

            break;
        }

        return next(context);
    }
}
