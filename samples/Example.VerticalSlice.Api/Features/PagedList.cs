namespace Example.VerticalSlice.Api.Features;

public class PagedList<T>
{
    public PagedList()
    {
    }

    public PagedList(IEnumerable<T> items, int total, int page, int pageSize)
    {
        Items = items;
        TotalResults = total;
        Page = page;
        PageSize = pageSize;
    }

    public IEnumerable<T> Items { get; set; }
    public int TotalResults { get; set; }
    public int TotalPages { get => (int)Math.Ceiling((double)TotalResults / PageSize); }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
