namespace Example.VerticalSlice.Api.Features.CreateBeer;

public record Handler() : PostHandlerAsync<Request>("/beers")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<Response>(StatusCodes.Status201Created)
                .Produces<Response>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .WithName("CreateBeer")
                .WithTags("Beers")
                .WithValidation();

    protected override async Task<IResult> HandleAsync(Request req, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var brewery = await req.Database.Breweries.FindAsync(new object[] { (int)req.Body.BreweryId }, cancellationToken);
        if (brewery is null)
        {
            return Results.BadRequest("Unkown Brewery");
        }

        var style = await req.Database.Styles.FindAsync(new object[] { (int)req.Body.StyleId }, cancellationToken);
        if (style is null)
        {
            return Results.BadRequest("Invalid beer style");
        }


        var beer = new Beer {
            Name = req.Body.Name,
            Brewery = brewery,
            Style = style
        };

        req.Database.Beers.Add(beer);
        await req.Database.SaveChangesAsync(cancellationToken);

        var resource = (Response)beer;
        return Results.CreatedAtRoute("GetBeer", new { id = resource.Id.ToString() }, resource);
    }
}
