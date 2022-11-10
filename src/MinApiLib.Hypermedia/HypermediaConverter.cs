namespace MinApiLib.Hypermedia;

public class HypermediaConverter
{
    private readonly IEnumerable<IHypermediaProvider> _providers;

    public HypermediaConverter(IEnumerable<IHypermediaProvider> providers)
    {
        _providers = providers;
    }


    public HypermediaResponse Convert(object @object)
    {
        if (@object is null)
        {
            return null;
        }

        var provider = _providers.FirstOrDefault(p => p.CanProvideLinkFor(@object));
        if (provider is null)
        {
            return null;
        }

        return provider.Convert(@object);
    }
}
