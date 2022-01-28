using System;
using System.Threading.Tasks;

using Microsoft.Owin;
using HttpContext = Microsoft.Owin.IOwinContext;
using HttpRequest = Microsoft.Owin.IOwinRequest;
using RequestDelegate = Microsoft.Owin.OwinMiddleware;


namespace HotChocolate.AspNetClassic.Voyager
{
    internal sealed class SettingsMiddleware
        : RequestDelegate
    {
        private readonly VoyagerOptions _options;
        private readonly string _queryPath;

        public SettingsMiddleware(
            RequestDelegate next,
            VoyagerOptions options)
            : base(next)
        {
            _options = options
                       ?? throw new ArgumentNullException(nameof(options));

            Uri uiPath = UriFromPath(options.Path);
            Uri queryPath = UriFromPath(options.QueryPath);

            _queryPath = uiPath.MakeRelativeUri(queryPath).ToString();
            }

        /// <inheritdoc />
        public override async Task Invoke(HttpContext context)
        {
            string queryUrl = BuildUrl(context.Request, _queryPath);

            context.Response.ContentType = "application/javascript";

            await context.Response.WriteAsync($@"
                window.Settings = {{
                    url: ""{queryUrl}"",
                }}
            ",
            context.GetCancellationToken())
            .ConfigureAwait(false);
        }

        private static string BuildUrl(
            HttpRequest request,

            string path)
        {
            string uiPath = request.PathBase.Value
                .Substring(0, request.PathBase.Value.Length - 11);
            string scheme = request.Scheme;

            Uri uri = request.Uri;
            var uriBuilder = new UriBuilder(scheme, uri.Host, uri.Port,
                uiPath + path);

            return uriBuilder.ToString().TrimEnd('/');
        }

        private static Uri UriFromPath(PathString path)
        {
            return new Uri(
                "http://p" +
                (path.HasValue ? path.Value : "/").TrimEnd('/') +
                "/");
        }
    }
}
