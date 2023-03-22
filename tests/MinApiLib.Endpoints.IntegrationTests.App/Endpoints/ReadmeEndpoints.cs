namespace MinApiLib.Endpoints.IntegrationTests.App.Endpoints.Readme;

using Microsoft.AspNetCore.Mvc;

public record Hello01() : GetHandler("/readme/hello01")
{
    protected override IResult Handle()
    {
        return Results.Ok("Hello World!");
    }
}

public record Hello02() : GetHandler("/hello02")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .WithName("Hello")
                .WithTags("hello", "world");

    protected override IResult Handle()
        => Results.Ok("Hello World!");
}

public record struct Request03(string Name);

public record Hello03() : GetHandler<Request03>("/hello03/{name}")
{
    protected override IResult Handle(Request03 request)
        => Results.Ok($"Hello {request.Name}!");
}

public record struct Request04([FromRoute] string Name);

public record Hello04() : GetHandler<Request04>("/hello04/{name}")
{
    protected override IResult Handle(Request04 request)
        => Results.Ok($"Hello {request.Name}!");
}

public record struct Request05(string Name);

public record struct Response05(string Message);

public record Hello05() : GetHandler<Request05, Response05>("/hello05/{name}")
{
    protected override Response05 Handle(Request05 request)
        => new Response05($"Hello {request.Name}!");
}

public record Hello06() : GetHandlerAsync("/hello06")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .WithName("Hello")
                .WithTags("hello", "world");

    protected override async Task<IResult> HandleAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(1000);
        return Results.Ok("Hello World!");
    }
}

public record struct Request07(string Name);

public record Hello07() : GetHandlerAsync<Request07>("/hello07/{name}")
{
    protected override async Task<IResult> HandleAsync(Request07 request, CancellationToken cancellationToken)
    {
        await Task.Delay(1000);
        return Results.Ok($"Hello {request.Name}!");
    }
}

public record struct Request08([FromRoute] string Name);

public record Hello08() : GetHandlerAsync<Request08>("/hello08/{name}")
{
    protected override async Task<IResult> HandleAsync(Request08 request, CancellationToken cancellationToken)
    {
        await Task.Delay(1000);
        return Results.Ok($"Hello {request.Name}!");
    }
}

public record struct Request09(string Name);

public record struct Response09(string Message);

public record Hello09() : GetHandlerAsync<Request09, Response09>("/hello09/{name}")
{
    protected override async Task<Response09> HandleAsync(Request09 request, CancellationToken cancellationToken)
    {
        await Task.Delay(1000);
        return new Response09($"Hello {request.Name}!");
    }
}

public record Hello10(): Get("/hello10")
{
    public IResult Handle()
        => Results.Ok("Hello World!");
}

public record Hello11(): Get("/hello11")
{
    public Task<IResult> HandleAsync()
        => Task.FromResult(Results.Ok("Hello World!"));
}

public record Hello12(): Get("/hello12/{name}")
{
    public IResult Handle(string name)
        => Results.Ok($"Hello {name}!");
}

public record Hello13(): Get("/hello13/{name}")
{
    public IResult Handle([FromRoute] string name)
        => Results.Ok($"Hello {name}!");
}

public record Hello14(): Get("/hello14")
{
    public IResult Handle([FromQuery] string name, [FromHeader] string userAgent, [FromServices] CancellationToken cancellationToken)
        => Results.Ok($"Hello {name}! Your user agent is {userAgent}.");
}

public record struct Response15(string Message);

public record Hello15(): Get("/hello15/{name}")
{
    public Response15 Handle(string name)
        => new Response15($"Hello {name}!");
}