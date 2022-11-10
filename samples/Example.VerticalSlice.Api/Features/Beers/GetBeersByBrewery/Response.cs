namespace Example.VerticalSlice.Api.Features.GetBeersByBrewery;

public class Response : PagedList<ResponseItem>
{
    public Response(): base()
    {
    }

    public Response(IEnumerable<ResponseItem> items, int total, int page, int pageSize) : base(items, total, page, pageSize)
    {
    }
}

public class ResponseItem
{
    public HashedId Id { get; set; }
    public string Name { get; set; }

    public int BreweryId { get; set; }
    public string BreweryName { get; set; }

    public int StyleId { get; set; }
    public string StyleName { get; set; }
}

public class ResponseHypermediaProvider : HypermediaProvider<Response>
{
    protected override IEnumerable<HypermediaLink> GetLinksFor(Response @object)
    {
        var breweryId = @object.Items.FirstOrDefault()?.BreweryId;
        if (breweryId is not null)
        {
            yield return new HypermediaLink("brewery", $"/breweries/{breweryId}", "GET");
            yield return new HypermediaLink("self", $"/breweries/{breweryId}/beers?page={@object.Page}&pageSize={@object.PageSize}", "GET");

            if (@object.Page > 1)
            {
                yield return new HypermediaLink("previous", $"/breweries/{breweryId}/beers?page={@object.Page - 1}&pageSize={@object.PageSize}", "GET");
            }

            if (@object.Page < @object.TotalPages)
            {
                yield return new HypermediaLink("next", $"/breweries/{breweryId}/beers?page={@object.Page + 1}&pageSize={@object.PageSize}", "GET");
            }
        }

        yield return new HypermediaLink("create", "/beers", "POST");
    }
}