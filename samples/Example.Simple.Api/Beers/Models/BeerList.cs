namespace Example.Simple.Api.Beers.Models;

public class BeerList : PagedList<BeerListItem>
{
    public BeerList(): base()
    {
    }

    public BeerList(IEnumerable<BeerListItem> items, int total, int page, int pageSize) : base(items, total, page, pageSize)
    {
    }
}
