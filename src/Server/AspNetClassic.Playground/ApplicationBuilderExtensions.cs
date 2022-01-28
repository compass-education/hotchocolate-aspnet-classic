using System;

using HotChocolate.AspNetClassic.Playground;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.StaticFiles.ContentTypes;
using Owin;
using IApplicationBuilder = Owin.IAppBuilder;

namespace HotChocolate.AspNetClassic
{
    public static class ApplicationBuilderExtensions
    {
        private const string _resourcesNamespace =
            "HotChocolate.AspNetClassic.Playground.Resources";

        public static IApplicationBuilder UsePlayground(
            this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UsePlayground(new PlaygroundOptions());
        }

        public static IApplicationBuilder UsePlayground(
            this IApplicationBuilder applicationBuilder,
            PathString queryPath)
        {
            return applicationBuilder.UsePlayground(new PlaygroundOptions
            {
                QueryPath = queryPath,
                Path = queryPath + new PathString("/playground")
            });
        }

        public static IApplicationBuilder UsePlayground(
            this IApplicationBuilder applicationBuilder,
            PathString queryPath,
            PathString uiPath)
        {
            return applicationBuilder.UsePlayground(new PlaygroundOptions
            {
                QueryPath = queryPath,
                Path = uiPath
            });
        }

        public static IApplicationBuilder UsePlayground(
            this IApplicationBuilder applicationBuilder,
            PlaygroundOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return applicationBuilder
                .UsePlaygroundSettingsMiddleware(options)
                .UsePlaygroundFileServer(options.Path);
        }

        private static IApplicationBuilder UsePlaygroundSettingsMiddleware(
           this IApplicationBuilder applicationBuilder,
           PlaygroundOptions options)
        {
            return applicationBuilder.Map(
                options.Path.Add(new PathString("/settings.js")),
                app => app.Use<SettingsMiddleware>(options));
        }

        private static IApplicationBuilder UsePlaygroundFileServer(
            this IApplicationBuilder applicationBuilder,
            PathString route)
        {
            var fileServerOptions = new FileServerOptions
            {
                RequestPath = route,
                FileSystem = CreateFileSystem(),
                EnableDefaultFiles = true,
                StaticFileOptions =
                {
                    ContentTypeProvider =
                        new FileExtensionContentTypeProvider()
                }
            };

            return applicationBuilder.UseFileServer(fileServerOptions);
        }

        private static IFileSystem CreateFileSystem()
        {
            Type type = typeof(ApplicationBuilderExtensions);

            return new EmbeddedResourceFileSystem(
                type.Assembly,
                _resourcesNamespace);
        }
    }
}
