namespace MinApiLib.Hypermedia.Tests;

public class HypermediaIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private const string Hypermedia = HypermediaConstants.ContentType;

    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public HypermediaIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private Task<HttpResponseMessage> SendAsync(HttpMethod method, string uri, params string[] accept)
        => SendAsync(_client, method, uri, accept);

    private static Task<HttpResponseMessage> SendAsync(HttpClient client, HttpMethod method, string uri, params string[] accept)
    {
        var request = new HttpRequestMessage(method, uri);
        if (accept.Length > 0)
        {
            request.Headers.TryAddWithoutValidation("Accept", accept);
        }

        return client.SendAsync(request);
    }

    private static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response)
        => JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

    private static void AssertIsHypermedia(JsonElement json)
    {
        Assert.True(json.TryGetProperty("value", out _), $"Expected a 'value' property in: {json}");
        Assert.True(json.TryGetProperty("links", out _), $"Expected a 'links' property in: {json}");
    }

    private static void AssertIsPlain(JsonElement json)
    {
        Assert.False(json.TryGetProperty("links", out _), $"Unexpected 'links' property in: {json}");
        Assert.False(json.TryGetProperty("value", out _), $"Unexpected 'value' property in: {json}");
    }

    private static string[] Hrefs(JsonElement json)
        => json.GetProperty("links").EnumerateArray().Select(x => x.GetProperty("href").GetString()!).ToArray();

    // Response shape

    [Fact]
    public async Task Wraps_value_and_links_in_camel_case()
    {
        var response = await SendAsync(HttpMethod.Get, "/things/1", Hypermedia);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await ReadJsonAsync(response);
        Assert.Equal(1, json.GetProperty("value").GetProperty("id").GetInt32());
        Assert.Equal("thing", json.GetProperty("value").GetProperty("name").GetString());

        var self = json.GetProperty("links").EnumerateArray().First();
        Assert.Equal("self", self.GetProperty("rel").GetString());
        Assert.Equal("GET", self.GetProperty("method").GetString());
        Assert.Equal("http://localhost/things/1", self.GetProperty("href").GetString());
    }

    [Fact]
    public async Task Responds_with_the_hypermedia_content_type()
    {
        var response = await SendAsync(HttpMethod.Get, "/things/1", Hypermedia);

        Assert.Equal(Hypermedia, response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Responds_with_a_configured_content_type()
    {
        const string custom = "application/vnd.custom+json";
        using var factory = _factory.WithWebHostBuilder(b =>
            b.ConfigureServices(s => s.AddSingleton(new HypermediaOptions(custom))));
        var client = factory.CreateClient();

        var response = await SendAsync(client, HttpMethod.Get, "/things/1", custom);

        Assert.Equal(custom, response.Content.Headers.ContentType?.MediaType);
        AssertIsHypermedia(await ReadJsonAsync(response));

        var ignored = await SendAsync(client, HttpMethod.Get, "/things/1", Hypermedia);
        AssertIsPlain(await ReadJsonAsync(ignored));
    }

    [Fact]
    public async Task Keeps_absolute_links_untouched()
    {
        var response = await SendAsync(HttpMethod.Get, "/things/1", Hypermedia);

        Assert.Contains("https://docs.example.com/things", Hrefs(await ReadJsonAsync(response)));
    }

    [Fact]
    public async Task Includes_the_path_base_in_relative_links()
    {
        var response = await SendAsync(HttpMethod.Get, "/base/things/1", Hypermedia);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("http://localhost/base/things/1", Hrefs(await ReadJsonAsync(response)));
    }

    [Theory]
    [InlineData(Hypermedia)]
    [InlineData("application/json")]
    public async Task Tells_caches_that_the_response_varies_by_accept(string accept)
    {
        var response = await SendAsync(HttpMethod.Get, "/things/1", accept);

        Assert.Contains("Accept", response.Headers.Vary);
    }

    [Fact]
    public async Task Without_filter_does_not_vary_by_accept()
    {
        var response = await SendAsync(HttpMethod.Get, "/without-filter", Hypermedia);

        Assert.DoesNotContain("Accept", response.Headers.Vary);
    }

    // Content negotiation

    [Theory]
    [InlineData(Hypermedia, true)]
    [InlineData("APPLICATION/VND.HYPERMEDIA+JSON", true)]
    [InlineData(Hypermedia + "; charset=utf-8", true)]
    [InlineData("application/json, " + Hypermedia, true)]
    [InlineData(Hypermedia + ", application/json", true)]
    [InlineData("text/html," + Hypermedia + ";q=0.9,*/*;q=0.8", true)]
    [InlineData(Hypermedia + ";q=0", false)]
    [InlineData(Hypermedia + "-other", false)]
    [InlineData("application/json", false)]
    [InlineData("*/*", false)]
    [InlineData("not a media type", false)]
    public async Task Negotiates_on_the_accept_header(string accept, bool expectsHypermedia)
    {
        // HttpClient would split a list into several values; a real server receives the raw line.
        var context = await _factory.Server.SendAsync(c =>
        {
            c.Request.Method = HttpMethods.Get;
            c.Request.Path = "/things/1";
            c.Request.Headers.Accept = accept;
        });

        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        var json = JsonDocument.Parse(context.Response.Body).RootElement;
        if (expectsHypermedia)
        {
            AssertIsHypermedia(json);
        }
        else
        {
            AssertIsPlain(json);
        }
    }

    [Fact]
    public async Task Negotiates_with_several_accept_headers()
    {
        var response = await SendAsync(HttpMethod.Get, "/things/1", "application/json", Hypermedia);

        AssertIsHypermedia(await ReadJsonAsync(response));
    }

    [Fact]
    public async Task Returns_the_plain_value_without_accept_header()
    {
        var response = await SendAsync(HttpMethod.Get, "/things/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        AssertIsPlain(await ReadJsonAsync(response));
    }

    // Success results

    [Theory]
    [InlineData("/typed/1")]
    [InlineData("/typed-async/1")]
    [InlineData("/convention")]
    [InlineData("/json")]
    [InlineData("/struct")]
    [InlineData("/list")]
    [InlineData("/single-endpoint")]
    public async Task Adds_links_to_every_kind_of_result(string uri)
    {
        var response = await SendAsync(HttpMethod.Get, uri, Hypermedia);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        AssertIsHypermedia(await ReadJsonAsync(response));
    }

    [Fact]
    public async Task Resolves_providers_with_scoped_dependencies()
    {
        var response = await SendAsync(HttpMethod.Get, "/with-service", Hypermedia);

        Assert.Contains("http://localhost/from-scoped-service", Hrefs(await ReadJsonAsync(response)));
    }

    [Fact]
    public async Task Created_keeps_status_and_location()
    {
        var response = await SendAsync(HttpMethod.Post, "/things", Hypermedia);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/things/1", response.Headers.Location?.ToString());
        AssertIsHypermedia(await ReadJsonAsync(response));
    }

    [Fact]
    public async Task Created_at_route_keeps_status_and_location()
    {
        var response = await SendAsync(HttpMethod.Post, "/things-at-route", Hypermedia);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("http://localhost/things/7", response.Headers.Location?.ToString());
        AssertIsHypermedia(await ReadJsonAsync(response));
    }

    [Fact]
    public async Task Accepted_keeps_status_and_location()
    {
        var response = await SendAsync(HttpMethod.Post, "/things-accepted", Hypermedia);

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        Assert.Equal("/things/1/status", response.Headers.Location?.ToString());
        AssertIsHypermedia(await ReadJsonAsync(response));
    }

    [Fact]
    public async Task Custom_results_keep_their_headers()
    {
        var response = await SendAsync(HttpMethod.Get, "/custom-result", Hypermedia);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("\"v1\"", response.Headers.ETag?.Tag);
        AssertIsHypermedia(await ReadJsonAsync(response));
    }

    // Results that must pass through untouched

    [Theory]
    [InlineData("/no-provider")]
    [InlineData("/no-provider-typed/1")]
    public async Task Without_provider_returns_the_plain_value(string uri)
    {
        var response = await SendAsync(HttpMethod.Get, uri, Hypermedia);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("no links", (await ReadJsonAsync(response)).GetProperty("name").GetString());
    }

    [Fact]
    public async Task Without_filter_returns_the_plain_value()
    {
        var response = await SendAsync(HttpMethod.Get, "/without-filter", Hypermedia);

        AssertIsPlain(await ReadJsonAsync(response));
    }

    [Theory]
    [InlineData("/no-content", HttpStatusCode.NoContent)]
    [InlineData("/ok-without-value", HttpStatusCode.OK)]
    [InlineData("/things/0", HttpStatusCode.NotFound)]
    public async Task Results_without_value_pass_through(string uri, HttpStatusCode expected)
    {
        var response = await SendAsync(HttpMethod.Get, uri, Hypermedia);

        Assert.Equal(expected, response.StatusCode);
        Assert.Equal(string.Empty, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Error_results_with_value_pass_through()
    {
        var response = await SendAsync(HttpMethod.Get, "/bad-request", Hypermedia);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        AssertIsPlain(await ReadJsonAsync(response));
    }

    // Endpoint metadata

    [Fact]
    public void ProducesHypermedia_declares_the_wrapped_type_and_content_type()
    {
        var endpoint = _factory.Services
                               .GetRequiredService<EndpointDataSource>()
                               .Endpoints
                               .Single(x => x.Metadata.GetMetadata<IEndpointNameMetadata>()?.EndpointName == "GetDocumented");

        var produces = endpoint.Metadata
                               .GetOrderedMetadata<IProducesResponseTypeMetadata>()
                               .Single(x => x.Type == typeof(HypermediaObject<Thing>));

        Assert.Equal(StatusCodes.Status200OK, produces.StatusCode);
        Assert.Contains(Hypermedia, produces.ContentTypes);
    }

    [Fact]
    public void ProducesHypermedia_uses_the_content_type_of_the_application()
    {
        const string custom = "application/vnd.custom+json";
        using var factory = _factory.WithWebHostBuilder(b =>
            b.ConfigureServices(s => s.AddSingleton(new HypermediaOptions(custom))));

        var endpoint = factory.Services
                              .GetRequiredService<EndpointDataSource>()
                              .Endpoints
                              .Single(x => x.Metadata.GetMetadata<IEndpointNameMetadata>()?.EndpointName == "GetDocumented");

        var produces = endpoint.Metadata
                               .GetOrderedMetadata<IProducesResponseTypeMetadata>()
                               .Single(x => x.Type == typeof(HypermediaObject<Thing>));

        Assert.Equal(new[] { custom }, produces.ContentTypes);
    }
}
