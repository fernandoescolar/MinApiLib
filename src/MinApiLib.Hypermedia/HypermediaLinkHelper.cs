namespace MinApiLib.Hypermedia;

public static class HypermediaLinkHelper
{
    [Obsolete("Use FulfillLinkUrls.")]
    public static void FullfillLinkUrls(HypermediaResponse hypermediaResponse, HttpContext httpContext)
        => FulfillLinkUrls(hypermediaResponse, httpContext);

    public static void FulfillLinkUrls(HypermediaResponse hypermediaResponse, HttpContext httpContext)
    {
        foreach (var link in hypermediaResponse.Links)
        {
            if (!IsAbsolute(link.Href))
            {
                link.Href = GetUrl(httpContext.Request, link.Href);
            }
        }
    }

    private static bool IsAbsolute(string href)
        => href.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
        || href.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

    private static string GetUrl(HttpRequest request, string path)
    {
        var url = new StringBuilder();
        url.Append(request.Scheme);
        url.Append("://");
        url.Append(request.Host);
        url.Append(request.PathBase);
        if (!path.StartsWith('/'))
        {
            url.Append('/');
        }

        url.Append(path);
        return url.ToString();
    }
}
