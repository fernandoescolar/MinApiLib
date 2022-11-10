namespace Example.Data;

public class BeerDbContext : DbContext
{
    public DbSet<Beer> Beers { get; set; }
    public DbSet<BeerStyle> Styles { get; set; }
    public DbSet<Brewery> Breweries { get; set; }

    public BeerDbContext(DbContextOptions options)
        : base(options)
    {
    }
}
