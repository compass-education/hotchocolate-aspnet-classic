using HotChocolate.AspNetClassic.Instrumentation;
using HotChocolate.AspNetClassic.Serialization;

using HttpRequestDelegate = Microsoft.Owin.OwinMiddleware;

namespace HotChocolate.AspNetClassic;

public sealed class HttpPostMiddleware : HttpPostMiddlewareBase
{
    public HttpPostMiddleware(
        HttpRequestDelegate next,
        IRequestExecutorResolver executorResolver,
        IHttpResultSerializer resultSerializer,
        IHttpRequestParser requestParser,
        IServerDiagnosticEvents diagnosticEvents,
        NameString schemaName)
        : base(
            next,
            executorResolver,
            resultSerializer,
            requestParser,
            diagnosticEvents,
            schemaName)
    {
    }
}
