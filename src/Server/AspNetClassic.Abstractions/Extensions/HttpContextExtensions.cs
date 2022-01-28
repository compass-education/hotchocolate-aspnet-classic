using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using HotChocolate.Server;
using Microsoft.Owin;
using HttpContext = Microsoft.Owin.IOwinContext;

namespace HotChocolate.AspNetClassic
{
    public static class HttpContextExtensions
    {
        public static IServiceProvider CreateRequestServices(
            this HttpContext context,
            IServiceProvider services)
        {
            context.Environment.Add(EnvironmentKeys.ServiceProvider,
                services);

            return services;
        }

        public static CancellationToken GetCancellationToken(
            this HttpContext context)
        {
            return context.Request.CallCancelled;
        }

        public static IPrincipal GetUser(this HttpContext context)
        {
            return context.Request.User;
        }

        public static bool IsTracingEnabled(this HttpContext context)
        {
            return context.Request.Headers
                       .TryGetValue(HttpHeaderKeys.Tracing,
                           out string[] values) &&
                   values.Any(v => v == HttpHeaderValues.TracingEnabled);
        }

        public static bool IsValidPath(
            this HttpContext context,
            PathString path)
        {
            return context.Request.Path.Equals(path,
                StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsValidPath(
            this HttpContext context,
            PathString path1, PathString path2)
        {
            return context.Request.Path.Equals(path1,
                StringComparison.OrdinalIgnoreCase)
                || context.Request.Path.Equals(path2,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
