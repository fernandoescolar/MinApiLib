using Microsoft.AspNetCore.Mvc;

namespace MinApiLib.Endpoints.IntegrationTests.App.Endpoints.HashedIdEndpoints;

public record struct Request(HashedId Id, CancellationToken CancellationToken);

public record struct Response(HashedId Id, string Name);

public record GetHashedIdEndpoints() : GetHandler<Request>("/things/{id}")
{
    protected override IResult Handle(Request request)
    {
        int id = request.Id;
        var response = new Response(id, "John Doe");
        return Results.Ok(response);
    }
}

public record struct LongRequest([FromRoute] HashedLongId Id);

public record GetHashedLongId() : GetHandler<LongRequest>("/long-things/{id}")
{
    protected override IResult Handle(LongRequest request)
        => Results.Ok(new { id = request.Id, raw = request.Id.Value });
}

public record struct BodyRequest([FromBody] BodyRequestBody Body);

public class BodyRequestBody
{
    public required HashedId OwnerId { get; set; }

    public HashedId OptionalId { get; set; }
}

public record PostHashedId() : PostHandler<BodyRequest>("/hashed-body")
{
    protected override IResult Handle(BodyRequest request)
        => Results.Ok(new { owner = request.Body.OwnerId.Value, optional = request.Body.OptionalId.Value });
}
