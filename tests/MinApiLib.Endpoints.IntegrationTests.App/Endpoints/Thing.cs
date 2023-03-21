namespace MinApiLib.Endpoints.IntegrationTests.App.Endpoints.Thing;

public record struct Request(HashedId Id, CancellationToken CancellationToken);

public record struct Response(HashedId Id, string Name);

public record GetThing() : Get<Request>("/things/{id}")
{
    protected override IResult Handle(Request request)
    {
        int id = request.Id;
        var response = new Response(id, "John Doe");
        return Results.Ok(response);
    }
}