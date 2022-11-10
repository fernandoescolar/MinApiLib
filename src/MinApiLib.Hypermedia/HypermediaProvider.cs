namespace MinApiLib.Hypermedia;

public abstract class HypermediaProvider<T> : IHypermediaProvider
{
    public bool CanProvideLinkFor(object @object) => @object is T;

    public HypermediaResponse Convert(object @object)
    {
        if (@object is T t)
        {
            return ConvertObject(t);
        }

        throw new ArgumentException("Invalid type", nameof(@object));
    }

    public IEnumerable<HypermediaLink> GetLinks(object @object)
    {
        if (@object is T t)
        {
            return GetLinksFor(t);
        }

        throw new ArgumentException("Invalid type", nameof(@object));
    }

    protected abstract IEnumerable<HypermediaLink> GetLinksFor(T @object);

    private HypermediaObject<T> ConvertObject(T t)
        => new HypermediaObject<T>(t, GetLinksFor(t).ToList());
}
