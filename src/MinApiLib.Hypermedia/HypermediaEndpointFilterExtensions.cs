namespace MinApiLib.Hypermedia;

public static class HypermediaEndpointFilterExtensions
{
    public static RouteHandlerBuilder WithHypermedia(this RouteHandlerBuilder builder)
    {
        builder.AddEndpointFilter<HypermediaEndpointFilter>();
        return builder;
    }

    public static RouteGroupBuilder WithHypermedia(this RouteGroupBuilder builder)
    {
        builder.AddEndpointFilter<HypermediaEndpointFilter>();
        return builder;
    }

    public static RouteHandlerBuilder ProducesHypermedia<TResponse>(this RouteHandlerBuilder builder, int statusCode = 200, string contentType = null)
    {
        contentType ??= HypermediaConstants.ConfiguredContentType;
        builder.Produces<HypermediaObject<TResponse>>(statusCode, default, additionalContentTypes: new[] { contentType });
        return builder;
    }
}
