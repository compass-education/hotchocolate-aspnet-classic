using System.Threading;
using Microsoft.Owin;
using HttpContext = Microsoft.Owin.IOwinContext;

namespace HotChocolate.AspNetClassic.Voyager
{
    internal static class HttpContextExtensions
    {
        public static CancellationToken GetCancellationToken(
            this HttpContext context)
        {
            return context.Request.CallCancelled;
        }
    }
}
