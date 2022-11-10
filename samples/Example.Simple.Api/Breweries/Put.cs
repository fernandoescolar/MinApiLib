namespace Example.Simple.Api.Breweries;

public record Put() : AbstractUpsert("PUT", "/breweries/{id}")
{
    protected override void OnConfigure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<BreweryDetail>(StatusCodes.Status200OK)
                .Produces<BreweryDetail>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .WithName("UpsertBrewery")
                .WithTags("Breweries")
                .WithValidation();

    public Task<IResult> HandleAsync(HashedId id, BreweryRequest input, BeerDbContext db, CancellationToken cancellationToken)
        => UpsertAsync(db, input, id, cancellationToken: cancellationToken);
}
