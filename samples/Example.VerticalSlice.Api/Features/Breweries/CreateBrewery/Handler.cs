namespace Example.VerticalSlice.Api.Features.CreateBrewery;

public record Handler() : PostHandlerAsync<Request>("/breweries")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<Response>(StatusCodes.Status201Created)
                .Produces<Response>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .WithName("CreateBrewery")
                .WithTags("Breweries")
                .WithValidation();

    protected override async Task<IResult> HandleAsync(Request req, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var brewery = new Brewery {
            Name = req.Body.Name,
            City = req.Body.City,
            Country = req.Body.Country,
        };

        req.Database.Breweries.Add(brewery);
        await req.Database.SaveChangesAsync(cancellationToken);

        var resource = (Response)brewery;
        return Results.CreatedAtRoute("GetBrewery", new { id = resource.Id.ToString() }, resource);
    }
}
