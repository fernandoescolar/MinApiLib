namespace MinApiLib.Validation;

public class ValidationEndpointFilter : IEndpointFilter
{
    public ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        foreach (var argument in context.Arguments)
        {
            if (argument is null) continue;

            var argumentIsBody = argument.GetType().GetCustomAttributes(typeof(FromBodyAttribute), true).FirstOrDefault() is not null;
            if (argumentIsBody)
            {
                if (!MiniValidator.TryValidate(argument, out var errors))
                {
                    return ValueTask.FromResult<object>(Results.BadRequest(errors));
                }

                break;
            }

            var bodyProperty = argument.GetType().GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(FromBodyAttribute), true).Any());
            bodyProperty ??= argument.GetType().GetProperties().FirstOrDefault(x => x.Name == "Body");
            if (bodyProperty is not null)
            {
                if (!MiniValidator.TryValidate(bodyProperty.GetValue(argument), out var errors))
                {
                    return ValueTask.FromResult<object>(Results.BadRequest(errors));
                }

                break;
            }
        }

        return next(context);
    }
}
