namespace Example.Simple.Api.Beers.Hypermedia;

public class BeerDetailHypermediaProvider : HypermediaProvider<BeerDetail>
{
    protected override IEnumerable<HypermediaLink> GetLinksFor(BeerDetail @object)
    {
        yield return new HypermediaLink("self", "/beers/" + @object.Id, "GET");
        yield return new HypermediaLink("update", "/beers/" + @object.Id, "PUT");
        yield return new HypermediaLink("delete", "/beers/" + @object.Id, "DELETE");
        yield return new HypermediaLink("beers", "/beers", "GET");
    }
}
