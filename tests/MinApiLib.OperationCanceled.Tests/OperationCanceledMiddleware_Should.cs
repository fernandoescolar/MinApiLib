namespace MinApiLib.OperationCanceled.Tests;

public class OperationCanceledMiddleware_Should
{
    [Fact]
    public async Task Return_499_When_Operation_Has_Been_Cancelled()
    {
        var target = new OperationCanceledMiddleware(_ => throw new OperationCanceledException(), new OperationCanceledOptions(), NullLogger<OperationCanceledMiddleware>.Instance);
        var context = new DefaultHttpContext();

        await target.Invoke(context);

        Assert.Equal(499, context.Response.StatusCode);
    }

    [Fact]
    public async Task Not_Return_499_When_Operation_Has_Not_Been_Cancelled()
    {
        var target = new OperationCanceledMiddleware(_ => Task.CompletedTask, new OperationCanceledOptions(), NullLogger<OperationCanceledMiddleware>.Instance);
        var context = new DefaultHttpContext();

        await target.Invoke(context);

        Assert.NotEqual(499, context.Response.StatusCode);
    }
}