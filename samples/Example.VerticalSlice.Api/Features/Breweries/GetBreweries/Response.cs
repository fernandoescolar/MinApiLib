namespace Example.VerticalSlice.Api.Features.GetBreweries;

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

    public string City { get; set; }

    public string Country { get; set; }
}

public class ResponseHypermediaProvider : HypermediaProvider<Response>
{
    protected override IEnumerable<HypermediaLink> GetLinksFor(Response @object)
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
