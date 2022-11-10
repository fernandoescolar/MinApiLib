namespace MinApiLib.OperationCanceled;

[DebuggerStepThrough]
public class OperationCanceledMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<OperationCanceledMiddleware> _logger;

    public OperationCanceledMiddleware(RequestDelegate next, ILogger<OperationCanceledMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(OperationCanceledException)
        {
            _logger.LogInformation("Request was canceled");
            context.Response.StatusCode = 499;
            await context.Response.WriteAsync("Client Closed Request");
        }
    }
}
