using Microsoft.AspNetCore.Http.Features;

namespace MinApiLib.Hypermedia;

internal record HypermediaResult(HypermediaResponse Value, IResult OriginalResult, string ContentType) : IResult
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        await ApplyOriginalStatusAndHeadersAsync(httpContext);
        await httpContext.Response.WriteAsJsonAsync(Value, Value.GetType(), options: null, ContentType, httpContext.RequestAborted);
    }

    // Only the original result knows its status code and headers (a Location can need route data to be built),
    // so it runs against a discarded body. This serializes the value one extra time, on hypermedia requests only.
    private async Task ApplyOriginalStatusAndHeadersAsync(HttpContext httpContext)
    {
        if (OriginalResult is null)
        {
            return;
        }

        var responseBody = httpContext.Features.GetRequiredFeature<IHttpResponseBodyFeature>();
        var discardedBody = new StreamResponseBodyFeature(Stream.Null);
        httpContext.Features.Set<IHttpResponseBodyFeature>(discardedBody);
        try
        {
            await OriginalResult.ExecuteAsync(httpContext);
            await discardedBody.CompleteAsync();
        }
        finally
        {
            httpContext.Features.Set(responseBody);
        }

        httpContext.Response.ContentLength = null;
    }
}
