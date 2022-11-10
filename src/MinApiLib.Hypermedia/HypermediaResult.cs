namespace MinApiLib.Hypermedia;

internal record HypermediaResult(HypermediaResponse Value, int StatusCode) : IResult
{
    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = StatusCode;
        httpContext.Response.ContentType = HypermediaConstants.ContentType;
        return httpContext.Response.WriteAsJsonAsync(Value, Value.GetType(), httpContext.RequestAborted);
    }
}