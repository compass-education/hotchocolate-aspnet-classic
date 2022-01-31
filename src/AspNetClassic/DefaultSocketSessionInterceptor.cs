using System.Security.Claims;

using HotChocolate.AspNetClassic.Subscriptions;
using HotChocolate.AspNetClassic.Subscriptions.Messages;

using HttpContext = Microsoft.Owin.IOwinContext;

namespace HotChocolate.AspNetClassic;

public class DefaultSocketSessionInterceptor : ISocketSessionInterceptor
{
    public virtual ValueTask<ConnectionStatus> OnConnectAsync(
        ISocketConnection connection,
        InitializeConnectionMessage message,
        CancellationToken cancellationToken) =>
        new ValueTask<ConnectionStatus>(ConnectionStatus.Accept());

    public virtual ValueTask OnRequestAsync(
        ISocketConnection connection,
        IQueryRequestBuilder requestBuilder,
        CancellationToken cancellationToken)
    {
        HttpContext context = connection.HttpContext;
        requestBuilder.TrySetServices(connection.RequestServices);
        requestBuilder.TryAddProperty(nameof(CancellationToken), connection.RequestAborted);
        requestBuilder.TryAddProperty(nameof(HttpContext), context);
        requestBuilder.TryAddProperty(nameof(ClaimsPrincipal), context.User);

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

    public virtual ValueTask OnCloseAsync(
        ISocketConnection connection,
        CancellationToken cancellationToken) =>
        default;
}
