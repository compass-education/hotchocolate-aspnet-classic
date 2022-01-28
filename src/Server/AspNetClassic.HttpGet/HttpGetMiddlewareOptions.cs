using System;
using HotChocolate.Language;
using Microsoft.Owin;
using HttpContext = Microsoft.Owin.IOwinContext;
using RequestDelegate = Microsoft.Owin.OwinMiddleware;

namespace HotChocolate.AspNetClassic
{
    public class HttpGetMiddlewareOptions
        : IHttpGetMiddlewareOptions
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
