using Microsoft.Owin;
using HttpContext = Microsoft.Owin.IOwinContext;
using HttpResponse = Microsoft.Owin.IOwinResponse;
using RequestDelegate = Microsoft.Owin.OwinMiddleware;

namespace HotChocolate.AspNetClassic
{
    public interface IHttpGetSchemaMiddlewareOptions
        : IPathOptionAccessor
    {
    }
}
