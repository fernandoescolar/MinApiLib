namespace Example.VerticalSlice.Api.Features.CreateBrewery;

public class Response
{
    public HashedId Id { get; set; }

    public string Name { get; set; }

    public string City { get; set; }

    public string Country { get; set; }

    public static implicit operator Response(Brewery brewery)
        => new Response
        {
            Id = brewery.Id,
            Name = brewery.Name,
            City = brewery.City,
            Country = brewery.Country
        };
}

public class ResponseHypermediaProvider : HypermediaProvider<Response>
{
    protected override IEnumerable<HypermediaLink> GetLinksFor(Response @object)
    {
        yield return new HypermediaLink("self", "/breweries/" + @object.Id, "GET");
        yield return new HypermediaLink("update", "/breweries/" + @object.Id, "PUT");
        yield return new HypermediaLink("delete", "/breweries/" + @object.Id, "DELETE");
        yield return new HypermediaLink("beers", "/breweries", "GET");
    }
}