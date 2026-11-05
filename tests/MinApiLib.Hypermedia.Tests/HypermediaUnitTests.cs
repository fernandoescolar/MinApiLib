namespace MinApiLib.Hypermedia.Tests;

public class HypermediaProviderTests
{
    private readonly ThingProvider _provider = new();

    [Fact]
    public void Provides_links_only_for_its_type()
    {
        Assert.True(_provider.CanProvideLinkFor(new Thing(1, "x")));
        Assert.False(_provider.CanProvideLinkFor(new NoProvider("x")));
    }

    [Fact]
    public void Converts_its_type_into_a_hypermedia_object()
    {
        var thing = new Thing(1, "x");

        var result = Assert.IsType<HypermediaObject<Thing>>(_provider.Convert(thing));

        Assert.Equal(thing, result.Value);
        Assert.Equal(2, result.Links.Count());
    }

    [Fact]
    public void Rejects_other_types()
    {
        Assert.Throws<ArgumentException>(() => _provider.Convert(new NoProvider("x")));
        Assert.Throws<ArgumentException>(() => _provider.GetLinks(new NoProvider("x")));
    }
}

public class HypermediaConverterTests
{
    private readonly HypermediaConverter _converter = new(new IHypermediaProvider[] { new ThingProvider(), new ThingListProvider() });

    [Fact]
    public void Uses_the_matching_provider()
        => Assert.IsType<HypermediaObject<Thing>>(_converter.Convert(new Thing(1, "x")));

    [Fact]
    public void Matches_collections_through_their_interface()
        => Assert.IsType<HypermediaObject<IEnumerable<ThingItem>>>(_converter.Convert(new List<ThingItem>()));

    [Fact]
    public void Returns_null_without_a_matching_provider()
        => Assert.Null(_converter.Convert(new NoProvider("x")));

    [Fact]
    public void Returns_null_for_null()
        => Assert.Null(_converter.Convert(null!));
}

public class HypermediaLinkHelperTests
{
    private static string Fulfill(string href, string pathBase = "")
    {
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("api.example.com", 8443);
        context.Request.PathBase = pathBase;

        var response = new HypermediaResponse(new[] { new HypermediaLink("rel", href, "GET") });
        HypermediaLinkHelper.FullfillLinkUrls(response, context);

        return response.Links.Single().Href;
    }

    [Theory]
    [InlineData("/things", "https://api.example.com:8443/things")]
    [InlineData("/things?page=2&pageSize=10", "https://api.example.com:8443/things?page=2&pageSize=10")]
    [InlineData("things", "https://api.example.com:8443/things")]
    [InlineData("http-status/1", "https://api.example.com:8443/http-status/1")]
    [InlineData("http://other.example.com/things", "http://other.example.com/things")]
    [InlineData("https://other.example.com/things", "https://other.example.com/things")]
    [InlineData("HTTPS://other.example.com/things", "HTTPS://other.example.com/things")]
    public void Builds_absolute_urls(string href, string expected)
        => Assert.Equal(expected, Fulfill(href));

    [Theory]
    [InlineData("/things", "https://api.example.com:8443/base/things")]
    [InlineData("things", "https://api.example.com:8443/base/things")]
    [InlineData("https://other.example.com/things", "https://other.example.com/things")]
    public void Includes_the_path_base(string href, string expected)
        => Assert.Equal(expected, Fulfill(href, "/base"));
}
