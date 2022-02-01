using System.Security.Claims;

using HttpContext = Microsoft.Owin.IOwinContext;

namespace HotChocolate.AspNetClassic;

public class DefaultHttpRequestInterceptor : IHttpRequestInterceptor
{
    public virtual ValueTask OnCreateAsync(
        HttpContext context,
        IRequestExecutor requestExecutor,
        IQueryRequestBuilder requestBuilder,
        CancellationToken cancellationToken)
    {
        requestBuilder.TrySetServices(context.RequestServices);
        requestBuilder.TryAddProperty(nameof(HttpContext), context);
        requestBuilder.TryAddProperty(nameof(ClaimsPrincipal), context.Authentication.User);
        requestBuilder.TryAddProperty(nameof(CancellationToken), context.Request.CallCancelled);

        if (context.IsTracingEnabled())
        {
            requestBuilder.TryAddProperty(WellKnownContextData.EnableTracing, true);
        }

        if (context.IncludeQueryPlan())
        {
            requestBuilder.TryAddProperty(WellKnownContextData.IncludeQueryPlan, true);
        }

        return default;
    }
}
