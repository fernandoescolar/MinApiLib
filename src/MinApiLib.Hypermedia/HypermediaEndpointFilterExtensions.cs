using Microsoft.AspNetCore.Http.Metadata;

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

    public static RouteHandlerBuilder ProducesHypermedia<TResponse>(this RouteHandlerBuilder builder, int statusCode = 200, string? contentType = null)
    {
        builder.Add(endpoint =>
        {
            var configured = endpoint.ApplicationServices.GetService<HypermediaOptions>()?.ContentType;
            var contentTypes = new[] { contentType ?? configured ?? HypermediaConstants.ContentType };
            endpoint.Metadata.Add(new ProducesHypermediaMetadata(statusCode, typeof(HypermediaObject<TResponse>), contentTypes));
        });

        return builder;
    }

    private sealed record ProducesHypermediaMetadata(int StatusCode, Type? Type, IEnumerable<string> ContentTypes) : IProducesResponseTypeMetadata;
}
