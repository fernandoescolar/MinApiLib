namespace MinApiLib.Hypermedia;

public interface IHypermediaProvider
{
    bool CanProvideLinkFor(object @object);

    IEnumerable<HypermediaLink> GetLinks(object @object);

    HypermediaResponse Convert(object @object);
}

