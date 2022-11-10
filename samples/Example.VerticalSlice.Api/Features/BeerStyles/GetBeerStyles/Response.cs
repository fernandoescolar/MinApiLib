namespace Example.VerticalSlice.Api.Features.GetBeerStyles;

public class Response
{
    public HashedId Id { get; set; }
    public string Name { get; set; }
}

public class ResponseHypermediaProvider : HypermediaProvider<IEnumerable<Response>>
{
    protected override IEnumerable<HypermediaLink> GetLinksFor(IEnumerable<Response> @object)
    {
        yield return new HypermediaLink("self", "/styles", "GET");
    }
}
