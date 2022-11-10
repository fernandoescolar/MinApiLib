namespace Example.Simple.Api.Beers;

public record Put() : AbstractUpsert("PUT", "/beers/{id}")
{
    protected override void OnConfigure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<BeerDetail>(StatusCodes.Status200OK)
                .Produces<BeerDetail>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .WithName("UpsertBeer")
                .WithTags("Beers")
                .WithValidation();

    public Task<IResult> HandleAsync(HashedId id, BeerRequest input, BeerDbContext db, CancellationToken cancellationToken)
        => UpsertAsync(db, input, id, cancellationToken: cancellationToken);
}
