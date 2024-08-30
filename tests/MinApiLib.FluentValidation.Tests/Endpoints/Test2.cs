namespace MinApiLib.FluentValidation.Tests;

public readonly record struct RequestDummy2(
    [FromBody] RequestBodyDummy2 Body
);
public readonly record struct RequestBodyDummy2(
    string Name
);

public class RequestDummyValidator2 : AbstractValidator<RequestDummy2>
{
    public RequestDummyValidator2()
    {
        RuleFor(x => x.Body).NotNull();
        RuleFor(x => x.Body.Name).NotEmpty();
    }
}

public record HandlerDummy2() : PostHandler<RequestDummy2>("test2")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder.WithValidation<RequestDummy2>();

    protected override IResult Handle(RequestDummy2 request)
    {
        return Results.Ok();
    }
}