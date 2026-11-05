namespace Example.Simple.Api.Beers.Models;

public class BreweryBeerList : PagedList<BeerListItem>
{
    public BreweryBeerList() : base()
    {
    }

    public BreweryBeerList(HashedId breweryId, IEnumerable<BeerListItem> items, int total, int page, int pageSize) : base(items, total, page, pageSize)
    {
        BreweryId = breweryId;
    }

    public HashedId BreweryId { get; set; }
}
