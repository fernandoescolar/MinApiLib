namespace Example.Simple.Api.Beers;

public abstract record AbstractUpsert(string Verb, string Path) : Endpoint(Verb, Path)
{
    protected async Task<IResult> UpsertAsync(BeerDbContext db, BeerRequest input, int? id = null, bool forceCreation = false, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var brewery = await db.Breweries.FindAsync(new object[] { (int)input.BreweryId }, cancellationToken);
        if (brewery is null)
        {
            return Results.BadRequest("Unkown Brewery");
        }

        var style = await db.Styles.FindAsync(new object[] { (int)input.StyleId }, cancellationToken);
        if (style is null)
        {
            return Results.BadRequest("Invalid beer style");
        }


        var beer = (id.HasValue && !forceCreation) ? await db.Beers.FindAsync(new object[] { id }, cancellationToken) : null;
        if (beer is null)
        {
            beer = new Beer();
            if (id.HasValue)
            {
                beer.Id = id.Value;
            }

            db.Beers.Add(beer);
        }

        beer.Name = input.Name;
        beer.Brewery = brewery;
        beer.Style = style;

        await db.SaveChangesAsync(cancellationToken);

        var resource = (BeerDetail)beer;
        if (id.HasValue)
        {
            return Results.Ok(resource);
        }
        else
        {
            return Results.CreatedAtRoute("GetBeer", new { id = resource.Id.ToString() }, resource);
        }
    }
}
