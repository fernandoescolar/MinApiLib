namespace MinApiLib.Endpoints.IntegrationTests.App.Endpoints.Hello;

public record Hello() : GetEndpoint("/hello")
{
    protected override void OnConfigure(RouteHandlerBuilder builder)
        => builder
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .WithName("Hello")
                .WithTags("hello", "world");

    public IResult Handle()
    {
        return Results.Ok("Hello World!");
    }
}
