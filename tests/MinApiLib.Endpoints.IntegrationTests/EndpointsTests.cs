using System.Net;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;

namespace MinApiLib.Endpoints.IntegrationTests;

public class EndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public EndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private static StringContent Json(string json) => new(json, Encoding.UTF8, "application/json");

    [Theory]
    [InlineData("/readme/hello01", "Hello World!")]
    [InlineData("/hello02", "Hello World!")]
    [InlineData("/hello03/Ann", "Hello Ann!")]
    [InlineData("/hello04/Ann", "Hello Ann!")]
    [InlineData("/hello05/Ann", "Hello Ann!")]
    [InlineData("/hello06", "Hello World!")]
    [InlineData("/hello07/Ann", "Hello Ann!")]
    [InlineData("/hello08/Ann", "Hello Ann!")]
    [InlineData("/hello09/Ann", "Hello Ann!")]
    [InlineData("/hello10", "Hello World!")]
    [InlineData("/hello11", "Hello World!")]
    [InlineData("/hello12/Ann", "Hello Ann!")]
    [InlineData("/hello13/Ann", "Hello Ann!")]
    [InlineData("/hello15/Ann", "Hello Ann!")]
    [InlineData("/hello-static", "Hello World!")]
    public async Task Maps_and_runs_every_kind_of_endpoint(string uri, string expected)
    {
        var response = await _client.GetAsync(uri);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(expected, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Binds_query_and_header_parameters_by_convention()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/hello14?name=Ann");
        request.Headers.TryAddWithoutValidation("User-Agent", "tests");

        var response = await _client.SendAsync(request);

        Assert.Contains("Hello Ann! Your user agent is tests.", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Answers_several_verbs()
    {
        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("/hello-verbs")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await _client.PostAsync("/hello-verbs", null)).StatusCode);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, (await _client.PutAsync("/hello-verbs", null)).StatusCode);
    }

    [Fact]
    public async Task Maps_the_query_verb_with_a_body()
    {
        var request = new HttpRequestMessage(new HttpMethod("QUERY"), "/hello-query") { Content = Json("""{"term":"beer"}""") };

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Searching beer", await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("/never-abstract")]
    [InlineData("/never-generic")]
    public async Task Skips_types_it_cannot_create(string uri)
        => Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync(uri)).StatusCode);

    [Fact]
    public async Task Validates_with_the_fluent_validator_of_the_request()
    {
        var invalid = await _client.PostAsync("/things", Json("""{"name":"a"}"""));
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);

        var valid = await _client.PostAsync("/things", Json("""{"name":"abc","city":"x","country":"y"}"""));
        Assert.Equal(HttpStatusCode.Created, valid.StatusCode);
    }

    [Fact]
    public async Task Applies_hypermedia_to_the_whole_group()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/things2/1");
        request.Headers.Accept.ParseAdd("application/vnd.hypermedia+json");

        var response = await _client.SendAsync(request);

        Assert.Contains("\"links\"", await response.Content.ReadAsStringAsync());
    }
}

public record WithoutHandle() : Get("/no-handle");

public record WithOverloads() : Get("/overloads")
{
    public IResult Handle() => Results.Ok();
    public IResult Handle(string name) => Results.Ok(name);
}

public record WithBothMethods() : Get("/both")
{
    public IResult Handle() => Results.Ok();
    public Task<IResult> HandleAsync() => Task.FromResult(Results.Ok());
}

public class NamingConventionTests
{
    private static IEndpointRouteBuilder Routes() => WebApplication.CreateBuilder().Build();

    [Fact]
    public void Fails_clearly_without_a_handle_method()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => Routes().MapEndpoint<WithoutHandle>());

        Assert.Contains(nameof(WithoutHandle), exception.Message);
    }

    [Fact]
    public void Fails_clearly_with_overloads()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => Routes().MapEndpoint<WithOverloads>());

        Assert.Contains(nameof(WithOverloads), exception.Message);
    }

    [Fact]
    public void Fails_clearly_with_both_methods()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => Routes().MapEndpoint<WithBothMethods>());

        Assert.Contains(nameof(WithBothMethods), exception.Message);
    }
}
