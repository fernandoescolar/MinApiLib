public record struct Response(HashedId Id, string Name);

public class ResponseHypermediaProvider : HypermediaProvider<Response>
{
    protected override IEnumerable<HypermediaLink> GetLinksFor(Response @object)
    {
        yield return new HypermediaLink("self", "/things/" + @object.Id, "GET");
        yield return new HypermediaLink("update", "/things/" + @object.Id, "PUT");
        yield return new HypermediaLink("delete", "/things/" + @object.Id, "DELETE");
        yield return new HypermediaLink("beers", "/things", "GET");
    }
}

public record GetThing() : GetEndpoint("/things2/{id}")
{
    public IResult Handle(int id)
        => Results.Ok(new Response(id, "John Doe"));
}