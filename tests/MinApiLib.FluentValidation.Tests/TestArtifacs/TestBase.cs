namespace MinApiLib.FluentValidation.Tests;

public abstract class TestBase : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TestBase(TestWebApplicationFactory factory)
    {
        var services = new ServiceCollection();
        _client = factory.CreateDefaultClient();
    }

    protected HttpClient HttpClient => _client;
}