namespace Example.Data;

public static class BeerDbExtensions
{
    public static IServiceCollection AddBeerDbContext(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<BeerDbContext>(options =>
            options.UseSqlite("Data Source=beers.db"));
        return serviceCollection;
    }

    public static void SeedBeerDbData(this IHost app)
    {
        var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

        using (var scope = scopedFactory.CreateScope())
        {
            var db = scope.ServiceProvider.GetService<BeerDbContext>();
            db.Database.EnsureCreated();
            if (db.Beers.Any())
            {
                return;
            }
            db.Styles.AddRange(Seed.BeerStyles);
            db.Breweries.AddRange(Seed.Breweries);
            db.SaveChanges();

            db.Beers.AddRange(Seed.Beers);
            db.SaveChanges();
        }
    }
}
