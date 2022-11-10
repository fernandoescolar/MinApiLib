namespace Example.Simple.Api.Breweries;

public record Post() : AbstractUpsert("POST", "/breweries")
{
    protected override void OnConfigure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<BreweryDetail>(StatusCodes.Status201Created)
                .Produces<BreweryDetail>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .WithName("CreateBrewery")
                .WithTags("Breweries")
                .WithValidation();

    public Task<IResult> HandleAsync(BreweryRequest input, BeerDbContext db, CancellationToken cancellationToken)
        => UpsertAsync(db, input, forceCreation: true, cancellationToken: cancellationToken);
}
