namespace MinApiLib.Hypermedia;

public class HypermediaLink
{
    public HypermediaLink()
    {
    }

    public HypermediaLink(string rel, string href, string method)
    {
        Rel = rel;
        Href = href;
        Method = method;
    }
    public string Href { get; set; }
    public string Rel { get; set; }
    public string Method { get; set; }
}