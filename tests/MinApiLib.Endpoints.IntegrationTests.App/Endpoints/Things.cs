namespace MinApiLib.Endpoints.IntegrationTests.App.Endpoints.Things;

public record struct Response(HashedId Id, string Name);

public record GetThings() : Get("/things")
{
    protected override IResult Handle()
        => Results.Ok(new Response(1, "John Doe"));
}