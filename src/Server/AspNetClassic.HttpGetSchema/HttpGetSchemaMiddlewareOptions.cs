using System;
using Microsoft.Owin;
using HttpContext = Microsoft.Owin.IOwinContext;
using HttpResponse = Microsoft.Owin.IOwinResponse;
using RequestDelegate = Microsoft.Owin.OwinMiddleware;

namespace HotChocolate.AspNetClassic
{
    public class HttpGetSchemaMiddlewareOptions
        : IHttpGetSchemaMiddlewareOptions
    {
        private PathString _path = new PathString("/");

        public PathString Path
        {
            get => _path;
            set
            {
                if (!value.HasValue)
                {
                    // TODO : resources
                    throw new ArgumentException(
                        "The path cannot be empty.");
                }

                _path = value;
            }
        }
    }
}
