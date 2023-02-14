namespace MinApiLib.Endpoints.IntegrationTests.App.Endpoints.Things;

public record struct Response(HashedId Id, string Name);

public record Get() : GetEndpoint("/things")
{
    public IResult Handle()
        => Results.Ok(new Response(1, "John Doe"));
}