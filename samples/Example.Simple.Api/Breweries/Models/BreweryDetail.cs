namespace Example.Simple.Api.Breweries.Models;

public class BreweryDetail
{
    public HashedId Id { get; set; }

    public string Name { get; set; }

    public string City { get; set; }

    public string Country { get; set; }

    public static implicit operator BreweryDetail(Brewery brewery)
        => new BreweryDetail
        {
            Id = brewery.Id,
            Name = brewery.Name,
            City = brewery.City,
            Country = brewery.Country
        };
}
