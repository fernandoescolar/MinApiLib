namespace Example.Simple.Api.Breweries;

public abstract record AbstractUpsert(string Verb, string Path): Endpoint(Verb, Path)
{
    protected async Task<IResult> UpsertAsync(BeerDbContext db, BreweryRequest input, int? id = null, bool forceCreation = false, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var brewery = (id.HasValue && !forceCreation) ? await db.Breweries.FindAsync(new object[] { id }, cancellationToken) : null;
        if (brewery is null)
        {
            brewery = new Brewery();
            if (id.HasValue)
            {
                brewery.Id = id.Value;
            }

            db.Breweries.Add(brewery);
        }

        brewery.Name = input.Name;
        brewery.City = input.City;
        brewery.Country = input.Country;

        await db.SaveChangesAsync(cancellationToken);

        var resource = (BreweryDetail)brewery;
        if (id.HasValue)
        {
            return Results.Ok(resource);
        }
        else
        {
            return Results.CreatedAtRoute("GetBrewery", new { id = resource.Id.ToString() }, resource);
        }
    }
}
