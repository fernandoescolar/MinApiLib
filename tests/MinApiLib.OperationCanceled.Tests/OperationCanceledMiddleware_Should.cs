using Microsoft.AspNetCore.Http.Features;

namespace MinApiLib.OperationCanceled.Tests;

public class OperationCanceledMiddleware_Should
{
    private static DefaultHttpContext AbortedContext()
    {
        var aborted = new CancellationTokenSource();
        aborted.Cancel();
        return new DefaultHttpContext { RequestAborted = aborted.Token };
    }

    private static OperationCanceledMiddleware Target(RequestDelegate next, OperationCanceledOptions? options = null)
        => new(next, options ?? new OperationCanceledOptions(), NullLogger<OperationCanceledMiddleware>.Instance);

    [Fact]
    public async Task Return_499_When_The_Client_Aborted_The_Request()
    {
        var context = AbortedContext();

        await Target(_ => throw new OperationCanceledException()).Invoke(context);

        Assert.Equal(499, context.Response.StatusCode);
    }

    [Fact]
    public async Task Return_Specified_Status_Code_When_The_Client_Aborted_The_Request()
    {
        var context = AbortedContext();

        await Target(_ => throw new OperationCanceledException(), new OperationCanceledOptions { StatusCode = 444 }).Invoke(context);

        Assert.Equal(444, context.Response.StatusCode);
    }

    [Fact]
    public async Task Catch_TaskCanceledException_When_The_Client_Aborted_The_Request()
    {
        var context = AbortedContext();

        await Target(_ => throw new TaskCanceledException()).Invoke(context);

        Assert.Equal(499, context.Response.StatusCode);
    }

    [Fact]
    public async Task Not_Catch_A_Cancellation_When_The_Client_Is_Still_Connected()
    {
        var context = new DefaultHttpContext();

        await Assert.ThrowsAsync<TaskCanceledException>(() => Target(_ => throw new TaskCanceledException()).Invoke(context));
    }

    [Fact]
    public async Task Not_Touch_The_Response_When_It_Has_Already_Started()
    {
        var context = AbortedContext();
        context.Features.Set<IHttpResponseFeature>(new StartedResponseFeature());

        await Target(_ => throw new OperationCanceledException()).Invoke(context);

        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async Task Not_Change_The_Status_Code_When_Nothing_Is_Canceled()
    {
        var context = AbortedContext();

        await Target(_ => Task.CompletedTask).Invoke(context);

        Assert.Equal(200, context.Response.StatusCode);
    }

    private sealed class StartedResponseFeature : HttpResponseFeature
    {
        public override bool HasStarted => true;
    }
}
