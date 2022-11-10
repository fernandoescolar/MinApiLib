namespace Example.Simple.Api.Beers.Models;

public class BeerDetailChild
{
    public HashedId Id { get; set; }
    public string Name { get; set; }

    public static implicit operator BeerDetailChild(Brewery brewery)
        => new BeerDetailChild
        {
            Id = brewery.Id,
            Name = brewery.Name,
        };

    public static implicit operator BeerDetailChild(BeerStyle style)
        => new BeerDetailChild
        {
            Id = style.Id,
            Name = style.Name,
        };
}