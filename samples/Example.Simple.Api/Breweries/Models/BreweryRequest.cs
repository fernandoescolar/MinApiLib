namespace Example.Simple.Api.Breweries.Models;

public class BreweryRequest
{
    public string Name { get; set; }

    public string City { get; set; }

    public string Country { get; set; }
}

public class BreweryRequestValidator : AbstractValidator<BreweryRequest>
{
    public BreweryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().Length(3, 150);
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();
    }
}
