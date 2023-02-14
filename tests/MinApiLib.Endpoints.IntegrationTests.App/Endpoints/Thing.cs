namespace MinApiLib.Endpoints.IntegrationTests.App.Endpoints.Thing;

public record struct Request(HashedId Id, CancellationToken CancellationToken);

public record struct Response(HashedId Id, string Name);

public record GetThing() : GetEndpoint<Request>("/things/{id}")
{
    protected override Task<IResult> OnHandleAsync(Request request, CancellationToken cancellationToken)
    {
        int id = request.Id;
        var response = new Response(id, "John Doe");
        return Task.FromResult(Results.Ok(response));
    }
}