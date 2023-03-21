namespace MinApiLib.OperationCanceled;

public static class OperationCanceledMiddlewareExtensions
{
    public static IApplicationBuilder CatchOperationCanceled(this IApplicationBuilder app, Action<OperationCanceledOptions> configureOptions = null)
    {
        var options = new OperationCanceledOptions();
        configureOptions?.Invoke(options);
        app.UseMiddleware<OperationCanceledMiddleware>(options);
        return app;
    }
}
