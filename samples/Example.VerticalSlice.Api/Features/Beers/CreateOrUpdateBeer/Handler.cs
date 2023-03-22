namespace Example.VerticalSlice.Api.Features.CreateOrUpdateBeer;

public record Handler() : PutHandlerAsync<Request>("/beers/{id}")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<Response>(StatusCodes.Status201Created)
                .Produces<Response>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .WithName("CreateOrUpdateBeer")
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


        var beer = await req.Database.Beers.FindAsync(new object[] { (int)req.Id }, cancellationToken);
        if (beer is null)
        {
            beer = new Beer {
                Id = (int)req.Id
            };

            req.Database.Beers.Add(beer);
        }

        beer.Name = req.Body.Name;
        beer.Brewery = brewery;
        beer.Style = style;

        await req.Database.SaveChangesAsync(cancellationToken);

        var resource = (Response)beer;
        return Results.Ok(resource);
    }
}
