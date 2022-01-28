using Microsoft.Owin;
using HttpContext = Microsoft.Owin.IOwinContext;
using RequestDelegate = Microsoft.Owin.OwinMiddleware;

namespace HotChocolate.AspNetClassic
{
    public interface IHttpPostMiddlewareOptions
        : IPathOptionAccessor
        , IParserOptionsAccessor
    {
        int MaxRequestSize { get; }
    }
}
