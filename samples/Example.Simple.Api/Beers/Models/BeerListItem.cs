namespace Example.Simple.Api.Beers.Models;

public class BeerListItem
{
    public HashedId Id { get; set; }
    public string Name { get; set; }

    public int BreweryId { get; set; }
    public string BreweryName { get; set; }

    public int StyleId { get; set; }
    public string StyleName { get; set; }
}
