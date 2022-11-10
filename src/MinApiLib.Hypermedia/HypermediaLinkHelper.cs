namespace MinApiLib.Hypermedia;

public static class HypermediaLinkHelper
{
    public static void FullfillLinkUrls(HypermediaResponse hypermediaResponse, HttpContext httpContext)
    {
        foreach (var link in hypermediaResponse.Links)
        {
            if (!link.Href.StartsWith("http"))
            {
                link.Href = GetUrl(httpContext.Request, link.Href);
            }
        }
    }

    private static string GetUrl(HttpRequest request, string path)
    {
        var url = new StringBuilder();
        url.Append(request.Scheme);
        url.Append("://");
        url.Append(request.Host);
        url.Append(path);
        return url.ToString();
    }
}
