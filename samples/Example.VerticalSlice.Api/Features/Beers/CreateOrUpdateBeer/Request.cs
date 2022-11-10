namespace Example.VerticalSlice.Api.Features.CreateOrUpdateBeer;

public record struct Request(
    [FromServices] BeerDbContext Database,
    [FromBody] RequestBody Body,
    [FromRoute] HashedId Id
);

public class RequestBody
{
    [Required, StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }

    [Required]
    public HashedId BreweryId { get; set; }

    [Required]
    public HashedId StyleId { get; set; }
}