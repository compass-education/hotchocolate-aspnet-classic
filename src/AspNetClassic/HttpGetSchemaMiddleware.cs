
using HotChocolate.AspNetClassic.Instrumentation;
using HotChocolate.AspNetClassic.Serialization;
using Microsoft.AspNetCore.Http;
using static System.Net.HttpStatusCode;
using static HotChocolate.AspNetClassic.ErrorHelper;
using static HotChocolate.SchemaSerializer;

using HttpContext = Microsoft.Owin.IOwinContext;
using HttpRequestDelegate = Microsoft.Owin.OwinMiddleware;

namespace HotChocolate.AspNetClassic;

public sealed class HttpGetSchemaMiddleware : MiddlewareBase
{
    private readonly MiddlewareRoutingType _routing;
    private readonly IServerDiagnosticEvents _diagnosticEvents;

    public HttpGetSchemaMiddleware(
        HttpRequestDelegate next,
        IRequestExecutorResolver executorResolver,
        IHttpResultSerializer resultSerializer,
        IServerDiagnosticEvents diagnosticEvents,
        NameString schemaName,
        MiddlewareRoutingType routing)
        : base(next, executorResolver, resultSerializer, schemaName)
    {
        _diagnosticEvents = diagnosticEvents ??
            throw new ArgumentNullException(nameof(diagnosticEvents));
        _routing = routing;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var handle = _routing == MiddlewareRoutingType.Integrated
            ? HttpMethods.IsGet(context.Request.Method) &&
              context.Request.Query.Get("SDL") != null &&
              (context.GetGraphQLServerOptions()?.EnableSchemaRequests ?? true)
            : HttpMethods.IsGet(context.Request.Method) &&
              (context.GetGraphQLServerOptions()?.EnableSchemaRequests ?? true);

        if (handle)
        {
            if (!IsDefaultSchema)
            {
                context.Environment[WellKnownContextData.SchemaName] = SchemaName.Value;
            }

            using (_diagnosticEvents.ExecuteHttpRequest(context, HttpRequestKind.HttpGetSchema))
            {
                await HandleRequestAsync(context);
            }
        }
        else
        {
            // if the request is not a get request or if the content type is not correct
            // we will just invoke the next middleware and do nothing.
            await NextAsync(context);
        }
    }

    private async Task HandleRequestAsync(HttpContext context)
    {
        ISchema schema = await GetSchemaAsync(context.Request.CallCancelled);
        context.Environment[WellKnownContextData.Schema] = schema;

        var queryParamsDict = context.Request.Query.ToDictionary(x => x.Key, x => x.Value);
        
        bool indent =
            !(queryParamsDict.ContainsKey("indentation") &&
              string.Equals(
                  queryParamsDict["indentation"].FirstOrDefault(),
                  "none",
                  StringComparison.OrdinalIgnoreCase));

        if (queryParamsDict.TryGetValue("types", out var typesValue))
        {
            if (string.IsNullOrEmpty(typesValue))
            {
                await WriteResultAsync(context, TypeNameIsEmpty(), BadRequest);
                return;
            }

            await WriteTypesAsync(context, schema, typesValue, indent);
        }
        else
        {
            await WriteSchemaAsync(context, schema, indent);
        }
    }

    private async Task WriteTypesAsync(
        HttpContext context,
        ISchema schema,
        string typeNames,
        bool indent)
    {
        var types = new List<INamedType>();

        foreach (string typeName in typeNames.Split(','))
        {
            if (!SchemaCoordinate.TryParse(typeName, out SchemaCoordinate? coordinate) ||
                coordinate.Value.MemberName is not null ||
                coordinate.Value.ArgumentName is not null)
            {
                await WriteResultAsync(context, InvalidTypeName(typeName), BadRequest);
                return;
            }

            if (!schema.TryGetType<INamedType>(coordinate.Value.Name, out INamedType? type))
            {
                await WriteResultAsync(context, TypeNotFound(typeName), NotFound);
                return;
            }

            types.Add(type);
        }

        context.Response.ContentType = ContentType.GraphQL;
        context.Response.Headers.SetContentDisposition(GetTypesFileName(types));
        await SerializeAsync(types, context.Response.Body, indent, context.Request.CallCancelled);
        return;
    }

    private async Task WriteSchemaAsync(HttpContext context, ISchema schema, bool indent)
    {
        context.Response.ContentType = ContentType.GraphQL;
        context.Response.Headers.SetContentDisposition(GetSchemaFileName(schema));
        await SerializeAsync(schema, context.Response.Body, indent, context.Request.CallCancelled);
    }

    private string GetTypesFileName(List<INamedType> types)
        => types.Count == 1
            ? $"{types[0].Name.Value}.graphql"
            : "types.graphql";

    private string GetSchemaFileName(ISchema schema)
        => schema.Name.IsEmpty || schema.Name.Equals(Schema.DefaultName)
            ? "schema.graphql"
            : schema.Name + ".schema.graphql";
}
