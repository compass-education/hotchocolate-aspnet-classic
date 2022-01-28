using Microsoft.Owin;
using HttpRequest = Microsoft.Owin.IOwinRequest;

namespace HotChocolate.AspNetClassic
{
    public static class HttpRequestExtensions
    {
        public static bool IsHttps(
            this HttpRequest request)
        {
            return request.IsSecure;
        }
    }
}
