namespace MinApiLib.Validation;

public static class ValidationEndpointFilterExtensions
{
    public static RouteHandlerBuilder WithValidation(this RouteHandlerBuilder builder)
    {
        builder.AddEndpointFilter<ValidationEndpointFilter>();
        return builder;
    }

    public static RouteGroupBuilder WithValidation(this RouteGroupBuilder builder)
    {
        builder.AddEndpointFilter<ValidationEndpointFilter>();
        return builder;
    }
}
