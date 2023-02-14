namespace MinApiLib.Endpoints.IntegrationTests.App.Endpoints.Hello2;

public record struct Request(string Name);

public record Hello() : GetEndpoint<Request>("/hello2")
{
    protected override async Task<IResult> OnHandleAsync(Request request, CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
        return Results.Ok($"Hello {request.Name}!");
    }
}