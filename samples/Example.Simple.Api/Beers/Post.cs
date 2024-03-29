namespace Example.Simple.Api.Beers;

public record CreateBeer() : AbstractUpsert("POST", "/beers")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<BeerDetail>(StatusCodes.Status201Created)
                .Produces<BeerDetail>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .WithName(nameof(CreateBeer))
                .WithTags("Beers")
                .WithValidation();

    public Task<IResult> HandleAsync(BeerRequest input, BeerDbContext db, CancellationToken cancellationToken)
        => UpsertAsync(db, input, forceCreation: true, cancellationToken: cancellationToken);
}
