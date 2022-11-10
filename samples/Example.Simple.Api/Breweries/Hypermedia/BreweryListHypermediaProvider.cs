namespace Example.Simple.Api.Breweries.Hypermedia;

public class BreweryListHypermediaProvider : HypermediaProvider<BreweryList>
{
    protected override IEnumerable<HypermediaLink> GetLinksFor(BreweryList @object)
    {
        yield return new HypermediaLink("self", $"/breweries?page={@object.Page}&pageSize={@object.PageSize}", "GET");
        if (@object.Page > 1)
        {
            yield return new HypermediaLink("previous", $"/breweries?page={@object.Page - 1}&pageSize={@object.PageSize}", "GET");
        }
        if (@object.Page < @object.TotalPages)
        {
            yield return new HypermediaLink("next", $"/breweries?page={@object.Page + 1}&pageSize={@object.PageSize}", "GET");
        }

        yield return new HypermediaLink("create", "/breweries", "POST");
    }
}
