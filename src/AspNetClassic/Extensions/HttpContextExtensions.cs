
using Microsoft.Extensions.Primitives;

using HttpContext = Microsoft.Owin.IOwinContext;

namespace HotChocolate.AspNetClassic;

internal static class HttpContextExtensions
{
    public static GraphQLServerOptions? GetGraphQLServerOptions(this HttpContext context)
        => context.GetEndpoint()?.Metadata.GetMetadata<GraphQLServerOptions>() ??
           (context.Environment.TryGetValue(nameof(GraphQLServerOptions), out var o) &&
            o is GraphQLServerOptions options
                ? options
                : null);

    public static GraphQLToolOptions? GetGraphQLToolOptions(this HttpContext context)
        => GetGraphQLServerOptions(context)?.Tool;

    public static GraphQLEndpointOptions? GetGraphQLEndpointOptions(this HttpContext context)
        => context.GetEndpoint()?.Metadata.GetMetadata<GraphQLEndpointOptions>() ??
           (context.Environment.TryGetValue(nameof(GraphQLEndpointOptions), out var o) &&
            o is GraphQLEndpointOptions options
               ? options
               : null);

    public static bool IsTracingEnabled(this HttpContext context)
    {
        var headers = context.Request.Headers;

        return (headers.TryGetValue(HttpHeaderKeys.Tracing, out var values)
                || headers.TryGetValue(HttpHeaderKeys.ApolloTracing, out values)) &&
               values.Any(v => v == HttpHeaderValues.TracingEnabled);
    }

    public static bool IncludeQueryPlan(this HttpContext context)
    {
        var headers = context.Request.Headers;

        if (headers.TryGetValue(HttpHeaderKeys.QueryPlan, out var values) &&
            values.Any(v => v == HttpHeaderValues.IncludeQueryPlan))
        {
            return true;
        }

        return false;
    }
}
