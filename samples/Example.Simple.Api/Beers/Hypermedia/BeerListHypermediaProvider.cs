namespace Example.Simple.Api.Beers.Hypermedia;

public class BeerListHypermediaProvider : HypermediaProvider<BeerList>
{
    protected override IEnumerable<HypermediaLink> GetLinksFor(BeerList @object)
    {
        yield return new HypermediaLink("self", $"/beers?page={@object.Page}&pageSize={@object.PageSize}", "GET");
        if (@object.Page > 1)
        {
            yield return new HypermediaLink("previous", $"/beers?page={@object.Page - 1}&pageSize={@object.PageSize}", "GET");
        }
        if (@object.Page < @object.TotalPages)
        {
            yield return new HypermediaLink("next", $"/beers?page={@object.Page + 1}&pageSize={@object.PageSize}", "GET");
        }

        yield return new HypermediaLink("create", "/beers", "POST");
    }
}
