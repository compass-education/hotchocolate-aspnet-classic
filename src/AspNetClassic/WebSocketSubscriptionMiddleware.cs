
using HotChocolate.AspNetClassic.Instrumentation;
using HotChocolate.AspNetClassic.Serialization;
using HotChocolate.AspNetClassic.Subscriptions;

using HttpContext = Microsoft.Owin.IOwinContext;
using HttpRequestDelegate = Microsoft.Owin.OwinMiddleware;

namespace HotChocolate.AspNetClassic;

public class WebSocketSubscriptionMiddleware : MiddlewareBase
{
    private readonly IServerDiagnosticEvents _diagnosticEvents;

    public WebSocketSubscriptionMiddleware(
        HttpRequestDelegate next,
        IRequestExecutorResolver executorResolver,
        IHttpResultSerializer resultSerializer,
        IServerDiagnosticEvents diagnosticEvents,
        NameString schemaName)
        : base(next, executorResolver, resultSerializer, schemaName)
    {
        _diagnosticEvents = diagnosticEvents ??
            throw new ArgumentNullException(nameof(diagnosticEvents));
    }

    public Task InvokeAsync(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            return HandleWebSocketSessionAsync(context);
        }
        else
        {
            return NextAsync(context);
        }
    }

    private async Task HandleWebSocketSessionAsync(HttpContext context)
    {
        if (!IsDefaultSchema)
        {
            context.Environment[WellKnownContextData.SchemaName] = SchemaName.Value;
        }

        using (_diagnosticEvents.WebSocketSession(context))
        {
            try
            {
                IRequestExecutor requestExecutor = 
                    await GetExecutorAsync(context.Request.CallCancelled);
                IMessagePipeline? messagePipeline = 
                    requestExecutor.GetRequiredService<IMessagePipeline>();
                ISocketSessionInterceptor? socketSessionInterceptor = 
                    requestExecutor.GetRequiredService<ISocketSessionInterceptor>();
                context.Environment[WellKnownContextData.RequestExecutor] = requestExecutor;

                await WebSocketSession
                    .New(context, messagePipeline, socketSessionInterceptor)
                    .HandleAsync(context.Request.CallCancelled);
            }
            catch (Exception ex)
            {
                _diagnosticEvents.WebSocketSessionError(context, ex);
            }
        }
    }
}
