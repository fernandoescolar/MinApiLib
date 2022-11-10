namespace Example.Data;

public class Seed
{
    public static readonly List<BeerStyle> BeerStyles = new List<BeerStyle>
    {
        new BeerStyle { Id = 1, Name = "Lager" },
        new BeerStyle { Id = 2, Name = "Ale" },
        new BeerStyle { Id = 3, Name = "Stout" },
        new BeerStyle { Id = 4, Name = "Porter" },
        new BeerStyle { Id = 5, Name = "Pilsner" },
        new BeerStyle { Id = 6, Name = "Wheat" },
        new BeerStyle { Id = 7, Name = "Pale Ale" },
        new BeerStyle { Id = 8, Name = "IPA" },
        new BeerStyle { Id = 9, Name = "Sour" },
        new BeerStyle { Id = 10, Name = "Belgian" },
        new BeerStyle { Id = 11, Name = "Barley Wine" },
        new BeerStyle { Id = 12, Name = "Malt Liquor" },
        new BeerStyle { Id = 13, Name = "Mead" },
        new BeerStyle { Id = 14, Name = "Cider" },
        new BeerStyle { Id = 15, Name = "Other" }
    };

    public static readonly List<Brewery> Breweries = new List<Brewery>
    {
        new Brewery { Id = 1, Name = "Damm", City = "Barcelona", Country = "Spain" },
        new Brewery { Id = 2, Name = "Estrella Galicia", City = "La Coruña", Country = "Spain" },
        new Brewery { Id = 3, Name = "Mahou", City = "Madrid", Country = "Spain" },
        new Brewery { Id = 4, Name = "Ámbar", City = "Zaragoza", Country = "Spain" },
        new Brewery { Id = 5, Name = "Alhambra", City = "Granada", Country = "Spain" },
        new Brewery { Id = 6, Name = "San Miguel", City = "Malaga", Country = "Spain" },
        new Brewery { Id = 7, Name = "Amstel", City = "Zoeterwoude", Country = "Netherlands" },
        new Brewery { Id = 8, Name = "Heineken", City = "Amsterdam", Country = "Netherlands" },
        new Brewery { Id = 9, Name = "Brewdog", City = "Ellon", Country = "Scotland" },
        new Brewery { Id = 10, Name = "Budweiser", City = "St. Louis", Country = "USA" },
        new Brewery { Id = 11, Name = "Mikkeler", City = "Copenhagen", Country = "Denmark" },
        new Brewery { Id = 12, Name = "Modelo", City = "Ciudad de México", Country = "México" },
    };

    public static readonly List<Beer> Beers = new List<Beer>
    {
        new Beer { Id = 1, Name = "Estrella Damm", Brewery = Breweries[0], Style = BeerStyles[0] },
        new Beer { Id = 2, Name = "Estrella Galicia", Brewery = Breweries[1], Style = BeerStyles[0] },
        new Beer { Id = 3, Name = "Mahou 5 estrellas", Brewery = Breweries[2], Style = BeerStyles[0] },
        new Beer { Id = 4, Name = "Ámbar Centeno", Brewery = Breweries[3], Style = BeerStyles[0] },
        new Beer { Id = 5, Name = "Alhambra Singular", Brewery = Breweries[4], Style = BeerStyles[0] },
        new Beer { Id = 6, Name = "San Miguel 1516", Brewery = Breweries[5], Style = BeerStyles[0] },
        new Beer { Id = 7, Name = "Amstel Oro", Brewery = Breweries[6], Style = BeerStyles[0] },
        new Beer { Id = 8, Name = "Heineken", Brewery = Breweries[7], Style = BeerStyles[0] },
        new Beer { Id = 9, Name = "Brewdog Punk IPA", Brewery = Breweries[8], Style = BeerStyles[7] },
        new Beer { Id = 10, Name = "Budweiser", Brewery = Breweries[9], Style = BeerStyles[0] },
        new Beer { Id = 11, Name = "Mikkeller Beer Geek Brunch Weasel", Brewery = Breweries[10], Style = BeerStyles[7] },
        new Beer { Id = 12, Name = "Modelo Especial", Brewery = Breweries[11], Style = BeerStyles[0] },
        new Beer { Id = 13, Name = "Estrella Galicia 1906", Brewery = Breweries[1], Style = BeerStyles[0] },
        new Beer { Id = 14, Name = "Ámbar Especial", Brewery = Breweries[3], Style = BeerStyles[0] },
        new Beer { Id = 15, Name = "Voll-Damm", Brewery = Breweries[0], Style = BeerStyles[0] },
        new Beer { Id = 16, Name = "Inedit", Brewery = Breweries[0], Style = BeerStyles[0] },
        new Beer { Id = 17, Name = "Aguila Amstel", Brewery = Breweries[6], Style = BeerStyles[0] },
        new Beer { Id = 18, Name = "Mahou Maestra", Brewery = Breweries[2], Style = BeerStyles[0] },
        new Beer { Id = 19, Name = "Alhambra Reserva 1925", Brewery = Breweries[4], Style = BeerStyles[0] },
        new Beer { Id = 20, Name = "San Miguel 0,0", Brewery = Breweries[5], Style = BeerStyles[0] },
        new Beer { Id = 21, Name = "Amstel Radler", Brewery = Breweries[6], Style = BeerStyles[0] },
        new Beer { Id = 22, Name = "Heineken 0,0", Brewery = Breweries[7], Style = BeerStyles[0] },
        new Beer { Id = 23, Name = "Brewdog Elvis Juice", Brewery = Breweries[8], Style = BeerStyles[7] },
        new Beer { Id = 24, Name = "Budweiser Zero", Brewery = Breweries[9], Style = BeerStyles[0] },
        new Beer { Id = 25, Name = "Mikkeller Beer Geek Vanilla Shake", Brewery = Breweries[10], Style = BeerStyles[7] },
        new Beer { Id = 26, Name = "Modelo Negra", Brewery = Breweries[11], Style = BeerStyles[0] },
    };
}
