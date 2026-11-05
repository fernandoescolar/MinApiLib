namespace Example.Simple.Api.Beers.Hypermedia;

public class BreweryBeerListHypermediaProvider : HypermediaProvider<BreweryBeerList>
{
    protected override IEnumerable<HypermediaLink> GetLinksFor(BreweryBeerList @object)
    {
        yield return new HypermediaLink("brewery", $"/breweries/{@object.BreweryId}", "GET");
        yield return new HypermediaLink("self", $"/breweries/{@object.BreweryId}/beers?page={@object.Page}&pageSize={@object.PageSize}", "GET");
        if (@object.Page > 1)
        {
            yield return new HypermediaLink("previous", $"/breweries/{@object.BreweryId}/beers?page={@object.Page - 1}&pageSize={@object.PageSize}", "GET");
        }
        if (@object.Page < @object.TotalPages)
        {
            yield return new HypermediaLink("next", $"/breweries/{@object.BreweryId}/beers?page={@object.Page + 1}&pageSize={@object.PageSize}", "GET");
        }

        yield return new HypermediaLink("create", "/beers", "POST");
    }
}
