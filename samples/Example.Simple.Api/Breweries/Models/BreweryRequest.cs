namespace Example.Simple.Api.Breweries.Models;

public class BreweryRequest
{
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string Name { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    public string Country { get; set; }
}
