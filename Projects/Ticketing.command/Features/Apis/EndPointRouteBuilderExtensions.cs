namespace Ticketing.Command.Features.Apis;

public static class EndPointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapMinimalApisEndpoints
    (
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        IEnumerable<IMinimalApi> minimalApis = endpointRouteBuilder.ServiceProvider.GetServices<IMinimalApi>();

        foreach (IMinimalApi minimalApi in minimalApis)
        {
            minimalApi.AddEndpoint(endpointRouteBuilder);
        }

        return endpointRouteBuilder;
    }
}