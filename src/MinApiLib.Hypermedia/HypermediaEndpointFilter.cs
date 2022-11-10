namespace MinApiLib.Hypermedia;

public class HypermediaEndpointFilter : IEndpointFilter
{
    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var result = await next(context);
        if (!IsHypermediaRequest(context))
        {
            return result;
        }

        if (result is IStatusCodeHttpResult s
            && s.StatusCode >= 200
            && s.StatusCode < 300
            && result is IValueHttpResult v
            && v.Value is not null)
        {
            var hypermedia = context.HttpContext.RequestServices.GetService(typeof(HypermediaConverter)) as HypermediaConverter;
            if (hypermedia is null)
            {
                throw new InvalidOperationException("Unable to resolve HypermediaConverter");
            }

            var hypermediaResponse = hypermedia.Convert(v.Value);
            HypermediaLinkHelper.FullfillLinkUrls(hypermediaResponse, context.HttpContext);

            return new HypermediaResult(hypermediaResponse, s.StatusCode.Value);
        }

        return result;
    }

    private static bool IsHypermediaRequest(EndpointFilterInvocationContext context)
        => context.HttpContext
                  .Request
                  .Headers
                  .Accept
                  .Any(x => x.StartsWith(HypermediaConstants.ContentType, StringComparison.InvariantCultureIgnoreCase));
}
