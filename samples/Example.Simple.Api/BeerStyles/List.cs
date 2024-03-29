namespace Example.Simple.Api.BeerStyles;

public record GetBeerStyles() : Get("/styles")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<IEnumerable<BeerStyleDetail>>(StatusCodes.Status200OK)
                .Produces<IEnumerable<BeerStyleDetail>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .WithName(nameof(GetBeerStyles))
                .WithTags("Styles");

    public async Task<IResult> HandleAsync(BeerDbContext db, CancellationToken cancellationToken)
    {
        var total = await db.Styles.CountAsync(cancellationToken);
        if (total == 0)
        {
            return Results.NoContent();
        }

        var styles = await db.Styles
                        .Select(s => new BeerStyleDetail
                        {
                            Id = s.Id,
                            Name = s.Name,
                        })
                        .OrderBy(s => s.Name)
                        .ToListAsync(cancellationToken);

        return Results.Ok(styles);
    }
}
