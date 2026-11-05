namespace MinApiLib.Hypermedia.Tests;

public class TestWebApplication
{
    public static void Main(string[] _)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(TestWebApplication).Assembly.GetName().Name
        });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHypermedia();
        builder.Services.AddScoped<LinkSource>();

        var app = builder.Build();
        app.UsePathBase("/base");
        app.UseRouting();

        app.MapEndpoints()
           .WithHypermedia();

        app.MapGet("/single-endpoint", () => Results.Ok(new Thing(1, "single")))
           .WithHypermedia();

        app.MapGet("/without-filter", () => Results.Ok(new Thing(1, "plain")));

        app.Run();
    }
}

public class TestWebApplicationFactory : WebApplicationFactory<TestWebApplication>
{
}
