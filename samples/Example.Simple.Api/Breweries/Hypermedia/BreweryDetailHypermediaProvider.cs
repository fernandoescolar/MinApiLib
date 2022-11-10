namespace Example.Simple.Api.Breweries.Hypermedia;

public class BreweryDetailHypermediaProvider : HypermediaProvider<BreweryDetail>
{
    protected override IEnumerable<HypermediaLink> GetLinksFor(BreweryDetail @object)
    {
        yield return new HypermediaLink("self", "/breweries/" + @object.Id, "GET");
        yield return new HypermediaLink("update", "/breweries/" + @object.Id, "PUT");
        yield return new HypermediaLink("delete", "/breweries/" + @object.Id, "DELETE");
        yield return new HypermediaLink("beers", "/breweries", "GET");
    }
}
