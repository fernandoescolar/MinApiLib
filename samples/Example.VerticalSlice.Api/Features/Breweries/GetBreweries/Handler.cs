namespace Example.VerticalSlice.Api.Features.GetBreweries;

public record Handler() : GetEndpoint<Request>("/breweries")
{
    protected override void OnConfigure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<Response>(StatusCodes.Status200OK)
                .Produces<Response>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .WithName("GetBreweries")
                .WithTags("Breweries");

    protected override async Task<IResult> OnHandleAsync(Request req, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var total = await req.Database.Breweries.CountAsync(cancellationToken);
        if (total == 0)
        {
            return Results.NoContent();
        }

        var breweries = await req.Database
                                    .Breweries
                                    .Select(b => new ResponseItem
                                        {
                                            Id = b.Id,
                                            Name = b.Name,
                                            City = b.City,
                                            Country = b.Country,
                                        })
                                    .OrderBy(b => b.Name)
                                    .Skip((req.Page - 1) * req.PageSize)
                                    .Take(req.PageSize)
                                    .ToListAsync(cancellationToken);

        var resourceList = new Response(breweries, total, req.Page, req.PageSize);

        return Results.Ok(resourceList);
    }
}
