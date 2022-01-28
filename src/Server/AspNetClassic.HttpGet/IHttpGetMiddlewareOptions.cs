using Microsoft.Owin;
using HttpContext = Microsoft.Owin.IOwinContext;
using RequestDelegate = Microsoft.Owin.OwinMiddleware;

namespace HotChocolate.AspNetClassic
{
    public interface IHttpGetMiddlewareOptions
        : IPathOptionAccessor
    {
    }
}
