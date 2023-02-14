namespace MinApiLib.Hypermedia;

internal record HypermediaResult(HypermediaResponse Value, int StatusCode, string ContentType) : IResult
{
    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = StatusCode;
        httpContext.Response.ContentType = ContentType;
        return httpContext.Response.WriteAsJsonAsync(Value, Value.GetType(), httpContext.RequestAborted);
    }
}