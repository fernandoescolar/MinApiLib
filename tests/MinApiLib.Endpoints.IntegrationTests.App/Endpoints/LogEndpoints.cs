using Microsoft.AspNetCore.Mvc;
using MinApiLib.Logging;

namespace MinApiLib.Endpoints.IntegrationTests.App.Endpoints;

public record struct Request([FromServices]ILogger<LogEndpoints> Logger);

public record LogEndpoints() : GetHandler<Request>("logged")
{
    protected override IResult Handle(Request request)
    {
        request.Logger.Debug("Hello");
        return Results.Ok();
    }
}
