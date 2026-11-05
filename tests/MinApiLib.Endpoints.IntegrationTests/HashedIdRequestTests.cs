using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using MinApiLib.HashedIds;

namespace MinApiLib.Endpoints.IntegrationTests;

public class HashedIdRequestTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HashedIdRequestTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private Task<HttpResponseMessage> PostBodyAsync(string json)
        => _client.PostAsync("/hashed-body", new StringContent(json, Encoding.UTF8, "application/json"));

    [Fact]
    public async Task Binds_a_valid_route_value_and_writes_the_hash()
    {
        var hash = new HashedId(5).ToString();

        var response = await _client.GetAsync($"/things/{hash}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains($"\"id\":\"{hash}\"", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Rejects_an_invalid_route_value()
        => Assert.Equal(HttpStatusCode.BadRequest, (await _client.GetAsync("/things/!!nope!!")).StatusCode);

    [Fact]
    public async Task Binds_long_ids_beyond_the_int_range()
    {
        var id = new HashedLongId(int.MaxValue + 10L);

        var response = await _client.GetAsync($"/long-things/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains($"\"raw\":{id.Value}", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Accepts_a_valid_value_in_the_body()
    {
        var response = await PostBodyAsync($$"""{"ownerId":"{{new HashedId(5)}}"}""");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"owner\":5", await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("""{"ownerId":"!!nope!!"}""")]
    [InlineData("""{"ownerId":5}""")]
    [InlineData("""{"ownerId":null}""")]
    public async Task Rejects_an_invalid_value_in_the_body_before_the_handler(string json)
        => Assert.Equal(HttpStatusCode.BadRequest, (await PostBodyAsync(json)).StatusCode);

    [Fact]
    public async Task A_required_property_rejects_a_missing_value()
        => Assert.Equal(HttpStatusCode.BadRequest, (await PostBodyAsync("{}")).StatusCode);

    [Fact]
    public async Task A_property_that_is_not_required_defaults_to_zero()
    {
        var response = await PostBodyAsync($$"""{"ownerId":"{{new HashedId(5)}}"}""");

        Assert.Contains("\"optional\":0", await response.Content.ReadAsStringAsync());
    }
}
