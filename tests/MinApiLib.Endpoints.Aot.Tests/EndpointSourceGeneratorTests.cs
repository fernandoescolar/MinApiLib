using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using MinApiLib.Endpoints;

// Compiling this project is itself a test: every type below that the generator
// shouldn't map would produce code that doesn't compile.
public record GlobalNamespaceEndpoint() : GetHandler("/global")
{
    protected override IResult Handle() => Results.Ok();
}

namespace MinApiLib.Endpoints.Aot.Tests
{
    public record PlainEndpoint() : GetHandler("/plain")
    {
        protected override IResult Handle() => Results.Ok("plain");
    }

    internal record InternalEndpoint() : GetHandler("/internal")
    {
        protected override IResult Handle() => Results.Ok();
    }

    public record ConventionEndpoint() : Get("/convention")
    {
        public IResult Handle() => Results.Ok();
    }

    public partial record PartialEndpoint() : GetHandler("/partial");

    public partial record PartialEndpoint
    {
        protected override IResult Handle() => Results.Ok();
    }

    public class Outer
    {
        public record NestedEndpoint() : GetHandler("/nested")
        {
            protected override IResult Handle() => Results.Ok();
        }

        private record PrivateNestedEndpoint() : GetHandler("/never-private")
        {
            protected override IResult Handle() => Results.Ok();
        }
    }

    public abstract record AbstractEndpoint() : GetHandler("/never-abstract");

    public record GenericEndpoint<T>() : GetHandler("/never-generic")
    {
        protected override IResult Handle() => Results.Ok();
    }

    public record EndpointWithDependency(string Dependency) : GetHandler("/never-dependency")
    {
        protected override IResult Handle() => Results.Ok();
    }

    public class NotAnEndpoint
    {
    }

    public class EndpointSourceGeneratorTests
    {
        private static async Task<WebApplication> StartAsync()
        {
            var builder = WebApplication.CreateBuilder();
            builder.WebHost.UseTestServer();
            var app = builder.Build();
            app.MapAotEndpoints();
            await app.StartAsync();
            return app;
        }

        private static string?[] Routes(IEndpointRouteBuilder routes)
            => routes.DataSources
                     .SelectMany(x => x.Endpoints)
                     .OfType<RouteEndpoint>()
                     .Select(x => x.RoutePattern.RawText)
                     .ToArray();

        [Fact]
        public async Task Maps_every_mappable_endpoint_once()
        {
            await using var app = await StartAsync();

            var routes = Routes(app).OrderBy(x => x).ToArray();

            Assert.Equal(new[] { "/convention", "/global", "/internal", "/nested", "/partial", "/plain" }, routes);
        }

        [Fact]
        public async Task The_mapped_endpoints_answer()
        {
            await using var app = await StartAsync();

            var response = await app.GetTestClient().GetAsync("/plain");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("plain", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Returns_a_group_to_apply_conventions_to_all_endpoints()
        {
            var builder = WebApplication.CreateBuilder();
            builder.WebHost.UseTestServer();
            await using var app = builder.Build();
            app.MapAotEndpoints().AddEndpointFilter((context, next) => ValueTask.FromResult<object?>(Results.StatusCode(418)));
            await app.StartAsync();

            var response = await app.GetTestClient().GetAsync("/plain");

            Assert.Equal((HttpStatusCode)418, response.StatusCode);
        }
    }
}
