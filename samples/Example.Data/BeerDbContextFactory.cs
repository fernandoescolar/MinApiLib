namespace Example.Data;

public class BeerDbContextFactory : IDesignTimeDbContextFactory<BeerDbContext>
{
    public BeerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BeerDbContext>();
        optionsBuilder.UseSqlite("Data Source=beers.db");

        return new BeerDbContext(optionsBuilder.Options);
    }
}