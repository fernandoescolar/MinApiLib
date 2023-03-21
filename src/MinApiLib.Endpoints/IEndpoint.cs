namespace MinApiLib.Endpoints;

public interface IEndpoint
{
    RouteHandlerBuilder Configure(IEndpointRouteBuilder builder);
}
