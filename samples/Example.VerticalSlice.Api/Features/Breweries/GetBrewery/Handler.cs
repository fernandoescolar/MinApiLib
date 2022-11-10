namespace Example.VerticalSlice.Api.Features.GetBrewery;

public record Handler() : GetEndpoint<Request>("/breweries/{id}")
{
    protected override void OnConfigure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<Response>(StatusCodes.Status200OK)
                .Produces<Response>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("GetBrewery")
                .WithTags("Breweries");

    protected override async Task<IResult> OnHandleAsync(Request req, CancellationToken cancellationToken)
    {
        var brewery = await req.Database.Breweries.FindAsync(new object[] { (int)req.Id }, cancellationToken);
        if (brewery is null)
        {
            return Results.NotFound();
        }

        var resource = (Response)brewery;

        return Results.Ok(resource);
    }
}
