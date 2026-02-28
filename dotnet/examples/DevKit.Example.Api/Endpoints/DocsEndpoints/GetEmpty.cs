namespace DevKit.Example.Api.Endpoints.DocsEndpoints;

public static class GetEmpty
{
    internal static RouteGroupBuilder MapGetEmpty(this RouteGroupBuilder routeBuilder)
    {
        routeBuilder
            .MapGet("/", () => $"{nameof(MapGetEmpty)}");

        return routeBuilder;
    }
}
