namespace Example.Simple.Api.Breweries;

public record UpsertBrewery() : AbstractUpsert("PUT", "/breweries/{id}")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<BreweryDetail>(StatusCodes.Status200OK)
                .Produces<BreweryDetail>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .WithName(nameof(UpsertBrewery))
                .WithTags("Breweries")
                .WithValidation();

    public Task<IResult> HandleAsync(HashedId id, BreweryRequest input, BeerDbContext db, CancellationToken cancellationToken)
        => UpsertAsync(db, input, id, cancellationToken: cancellationToken);
}
