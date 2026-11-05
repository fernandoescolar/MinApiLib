using System.Net.Http.Headers;

// MapEndpoints keeps a static list of endpoints that is not safe for hosts starting in parallel.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace MinApiLib.Hypermedia.Tests;

// The result swaps the response body feature, so this exercises it on a real server over TCP.
public class HypermediaKestrelTests : IAsyncLifetime
{
    private WebApplication _app = default!;
    private HttpClient _client = default!;

    public async Task InitializeAsync()
    {
        var assembly = typeof(TestWebApplication).Assembly;
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { ApplicationName = assembly.GetName().Name });
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Services.AddHypermedia(assembly);
        builder.Services.AddScoped<LinkSource>();

        _app = builder.Build();
        _app.MapEndpoints(assembly)
            .WithHypermedia();

        await _app.StartAsync();
        _client = new HttpClient { BaseAddress = new Uri(_app.Urls.First()) };
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _app.DisposeAsync();
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string uri, string accept)
    {
        var request = new HttpRequestMessage(method, uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(accept));
        return await _client.SendAsync(request);
    }

    [Fact]
    public async Task Writes_links_for_an_accept_list()
    {
        var response = await SendAsync(HttpMethod.Get, "/things/1", HypermediaConstants.ContentType);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(HypermediaConstants.ContentType, response.Content.Headers.ContentType?.MediaType);
        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        Assert.Equal("thing", json.GetProperty("value").GetProperty("name").GetString());
        Assert.Equal($"{_client.BaseAddress!.ToString().TrimEnd('/')}/things/1", json.GetProperty("links")[0].GetProperty("href").GetString());
    }

    [Fact]
    public async Task Keeps_status_and_location_generated_from_a_route()
    {
        var response = await SendAsync(HttpMethod.Post, "/things-at-route", HypermediaConstants.ContentType);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.EndsWith("/things/7", response.Headers.Location?.ToString());
        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        Assert.Equal(7, json.GetProperty("value").GetProperty("id").GetInt32());
    }

    [Fact]
    public async Task Keeps_headers_of_custom_results_and_a_single_body()
    {
        var response = await SendAsync(HttpMethod.Get, "/custom-result", HypermediaConstants.ContentType);

        Assert.Equal("\"v1\"", response.Headers.ETag?.Tag);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body).RootElement;
        Assert.True(json.TryGetProperty("links", out _), body);
    }

    [Fact]
    public async Task Returns_the_plain_value_without_a_provider()
    {
        var response = await SendAsync(HttpMethod.Get, "/no-provider", HypermediaConstants.ContentType);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }
}
