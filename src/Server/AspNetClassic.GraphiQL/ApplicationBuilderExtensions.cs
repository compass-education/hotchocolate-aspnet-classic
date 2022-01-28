using System;
using HotChocolate.AspNetClassic.GraphiQL;
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
            "HotChocolate.AspNetClassic.GraphiQL.Resources";

        public static IApplicationBuilder UseGraphiQL(
            this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseGraphiQL(new GraphiQLOptions());
        }

        public static IApplicationBuilder UseGraphiQL(
            this IApplicationBuilder applicationBuilder,
            PathString queryPath)
        {
            return applicationBuilder.UseGraphiQL(new GraphiQLOptions
            {
                QueryPath = queryPath,
                Path = queryPath + new PathString("/graphiql")
            });
        }

        public static IApplicationBuilder UseGraphiQL(
            this IApplicationBuilder applicationBuilder,
            PathString queryPath,
            PathString uiPath)
        {
            return applicationBuilder.UseGraphiQL(new GraphiQLOptions
            {
                QueryPath = queryPath,
                Path = uiPath
            });
        }

        public static IApplicationBuilder UseGraphiQL(
            this IApplicationBuilder applicationBuilder,
            GraphiQLOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return applicationBuilder
                .UseGraphiQLSettingsMiddleware(options)
                .UseGraphiQLFileServer(options.Path);
        }

        private static IApplicationBuilder UseGraphiQLSettingsMiddleware(
           this IApplicationBuilder applicationBuilder,
           GraphiQLOptions options)
        {
            return applicationBuilder.Map(
                options.Path.Add(new PathString("/settings.js")),
                app => app.Use<SettingsMiddleware>(options));
        }

        private static IApplicationBuilder UseGraphiQLFileServer(
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
