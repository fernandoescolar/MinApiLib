namespace MinApiLib.FluentValidation.Tests;

public class UnitTest1 : TestBase
{
    public UnitTest1(TestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Test1_BadRequest()
    {
        var response = await PostAsync("/test1", "{}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Test1_Ok()
    {
        var response = await PostAsync("/test1", "{\"name\":\"test\"}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

     [Fact]
    public async Task Test2_BadRequest()
    {
        var response = await PostAsync("/test2", "{}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Test2_Ok()
    {
        var response = await PostAsync("/test2", "{\"name\":\"test\"}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private Task<HttpResponseMessage> PostAsync(string uri, string content, string mediaType = "application/json")
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = new StringContent(content, Encoding.UTF8, mediaType)
        };

        return HttpClient.SendAsync(request);
    }
}

