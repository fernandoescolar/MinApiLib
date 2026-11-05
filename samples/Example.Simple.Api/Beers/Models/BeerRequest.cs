namespace Example.Simple.Api.Beers.Models;

public class BeerRequest
{
    public string Name { get; set; }

    public required HashedId BreweryId { get; set; }

    public required HashedId StyleId { get; set; }
}

public class BeerRequestValidator : AbstractValidator<BeerRequest>
{
    public BeerRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().Length(3, 100);
    }
}
