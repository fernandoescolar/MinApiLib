namespace Example.VerticalSlice.Api.Features.CreateBeer;

public class Response
{
    public HashedId Id { get; set; }
    public string Name { get; set; }

    public ResponseChild Style { get; set; }
    public ResponseChild Brewery { get; set; }

    public static implicit operator Response(Beer beer)
        => new Response
        {
            Id = beer.Id,
            Name = beer.Name,
            Style = beer.Style,
            Brewery = beer.Brewery
        };
}

public class ResponseChild
{
    public HashedId Id { get; set; }
    public string Name { get; set; }

    public static implicit operator ResponseChild(Brewery brewery)
        => new ResponseChild
        {
            Id = brewery.Id,
            Name = brewery.Name,
        };

    public static implicit operator ResponseChild(BeerStyle style)
        => new ResponseChild
        {
            Id = style.Id,
            Name = style.Name,
        };
}

public class ResponseHypermediaProvider : HypermediaProvider<Response>
{
    protected override IEnumerable<HypermediaLink> GetLinksFor(Response @object)
    {
        yield return new HypermediaLink("self", "/beers/" + @object.Id, "GET");
        yield return new HypermediaLink("update", "/beers/" + @object.Id, "PUT");
        yield return new HypermediaLink("delete", "/beers/" + @object.Id, "DELETE");
        yield return new HypermediaLink("beers", "/beers", "GET");
    }
}

