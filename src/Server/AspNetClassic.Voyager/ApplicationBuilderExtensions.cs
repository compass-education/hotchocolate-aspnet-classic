using System;

using HotChocolate.AspNetClassic.Voyager;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.StaticFiles.ContentTypes;
using Owin;
using IApplicationBuilder = Owin.IAppBuilder;


namespace HotChocolate.AspNetClassic.Voyager
{
    public static class ApplicationBuilderExtensions
    {
        private const string _resourcesNamespace =
            "HotChocolate.AspNetClassic.Voyager.Resources";

        public static IApplicationBuilder UseVoyager(
            this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseVoyager(new VoyagerOptions());
        }

        public static IApplicationBuilder UseVoyager(
           this IApplicationBuilder applicationBuilder,
           PathString queryPath)
        {
            return applicationBuilder.UseVoyager(new VoyagerOptions
            {
                QueryPath = queryPath,
                Path = queryPath + new PathString("/voyager")
            });
        }

        public static IApplicationBuilder UseVoyager(
            this IApplicationBuilder applicationBuilder,
            PathString queryPath,
            PathString uiPath)
        {
            return applicationBuilder.UseVoyager(new VoyagerOptions
            {
                QueryPath = queryPath,
                Path = uiPath
            });
        }

        public static IApplicationBuilder UseVoyager(
            this IApplicationBuilder applicationBuilder,
            VoyagerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return applicationBuilder
                .UseVoyagerSettingsMiddleware(options)
                .UseVoyagerFileServer(options.Path);
        }

        private static IApplicationBuilder UseVoyagerSettingsMiddleware(
           this IApplicationBuilder applicationBuilder,
           VoyagerOptions options)
        {
            return applicationBuilder.Map(
                options.Path.Add(new PathString("/settings.js")),
                app => app.Use<SettingsMiddleware>(options));
        }

        private static IApplicationBuilder UseVoyagerFileServer(
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
