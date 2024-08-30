namespace MinApiLib.FluentValidation.Tests;

public class TestWebApplication
{
    public static void Main(string[] _)
    {
        var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions
        {
            ApplicationName = "TestWebApplication"
        });
        builder.Services.AddValidatorsFromAssembly();
        builder.Services.AddRouting();

        var app = builder.Build();
        app.UseRouting();
        app.MapEndpoints();
        app.Run();
    }
}