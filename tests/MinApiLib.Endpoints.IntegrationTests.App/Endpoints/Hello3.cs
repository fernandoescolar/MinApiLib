namespace MinApiLib.Endpoints.IntegrationTests.App.Endpoints.Hello3;

public record struct Request(string Name);

public record struct Response(string Message);

public record Hello() : GetAsync<Request, Response>("/hello3")
{
    protected override async Task<Response> HandleAsync(Request request, CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
        var response = new Response($"Hello {request.Name}!");
        return response;
    }
}