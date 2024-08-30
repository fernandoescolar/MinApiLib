using Microsoft.AspNetCore.Mvc;

namespace MinApiLib.FluentValidation.Tests;

public readonly record struct RequestDummy1(
    [FromBody] RequestBodyDummy1 Body
);
public readonly record struct RequestBodyDummy1(
    string Name
);

public class RequestDummyValidator1 : AbstractValidator<RequestDummy1>
{
    public RequestDummyValidator1()
    {
        RuleFor(x => x.Body).NotNull();
        RuleFor(x => x.Body.Name).NotEmpty();
    }
}

public record HandlerDummy1() : PostHandler<RequestDummy1>("test1")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder.WithValidation();

    protected override IResult Handle(RequestDummy1 request)
    {
        return Results.Ok();
    }
}