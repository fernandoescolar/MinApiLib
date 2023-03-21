namespace MinApiLib.Endpoints.IntegrationTests.App.Endpoints.Hello;

public record Hello() : Get("/hello")
{
     protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .WithName("Hello")
                .WithTags("hello", "world");

    protected override IResult Handle()
    {
        return Results.Ok("Hello World!");
    }
}
