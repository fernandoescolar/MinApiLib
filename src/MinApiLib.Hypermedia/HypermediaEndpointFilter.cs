namespace MinApiLib.Hypermedia;

public class HypermediaEndpointFilter : IEndpointFilter
{
    private readonly HypermediaOptions _options;

    public HypermediaEndpointFilter(HypermediaOptions options)
    {
        _options = options;
    }

    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var result = await next(context);
        if (!IsHypermediaRequest(context))
        {
            return result;
        }

        var value = GetSuccessValue(result);
        if (value is null)
        {
            return result;
        }

        var hypermedia = context.HttpContext.RequestServices.GetService(typeof(HypermediaConverter)) as HypermediaConverter;
        if (hypermedia is null)
        {
            throw new InvalidOperationException("Unable to resolve HypermediaConverter");
        }

        var hypermediaResponse = hypermedia.Convert(value);
        if (hypermediaResponse is null)
        {
            return result;
        }

        HypermediaLinkHelper.FullfillLinkUrls(hypermediaResponse, context.HttpContext);

        return new HypermediaResult(hypermediaResponse, result as IResult, _options.ContentType);
    }

    private static object GetSuccessValue(object result)
        => result switch
        {
            IStatusCodeHttpResult { StatusCode: < 200 or >= 300 } => null,
            IValueHttpResult valueResult => valueResult.Value,
            IResult => null,
            _ => result
        };

    private bool IsHypermediaRequest(EndpointFilterInvocationContext context)
        => context.HttpContext
                  .Request
                  .GetTypedHeaders()
                  .Accept
                  .Any(x => x.Quality != 0 && x.MediaType.Equals(_options.ContentType, StringComparison.OrdinalIgnoreCase));
}
