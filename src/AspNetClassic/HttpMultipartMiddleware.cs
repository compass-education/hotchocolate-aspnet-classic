using System.Text.Json;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using HotChocolate.AspNetClassic.Instrumentation;
using HotChocolate.AspNetClassic.Serialization;
using HotChocolate.Language;
using HotChocolate.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using static HotChocolate.AspNetClassic.Properties.AspNetClassicResources;

using HttpContext = Microsoft.Owin.IOwinContext;
using HttpRequest = Microsoft.Owin.IOwinRequest;
using HttpResponse = Microsoft.Owin.IOwinResponse;
using HttpRequestDelegate = Microsoft.Owin.OwinMiddleware;
using IFormCollection = Microsoft.Owin.IFormCollection;

namespace HotChocolate.AspNetClassic;

public sealed class HttpMultipartMiddleware : HttpPostMiddlewareBase
{
    private const string _operations = "operations";
    private const string _map = "map";
    private readonly FormOptions _formOptions;

    public HttpMultipartMiddleware(
        HttpRequestDelegate next,
        IRequestExecutorResolver executorResolver,
        IHttpResultSerializer resultSerializer,
        IHttpRequestParser requestParser,
        IServerDiagnosticEvents diagnosticEvents,
        NameString schemaName,
        IOptions<FormOptions> formOptions)
        : base(
            next,
            executorResolver,
            resultSerializer,
            requestParser,
            diagnosticEvents,
            schemaName)
    {
        _formOptions = formOptions.Value;
    }

    public override async Task InvokeAsync(HttpContext context)
    {
        if (HttpMethods.IsPost(context.Request.Method) &&
            (context.GetGraphQLServerOptions()?.EnableMultipartRequests ?? true) &&
            ParseContentType(context) == AllowedContentType.Form)
        {
            if (!IsDefaultSchema)
            {
                context.Environment[WellKnownContextData.SchemaName] = SchemaName.Value;
            }

            using (DiagnosticEvents.ExecuteHttpRequest(context, HttpRequestKind.HttpMultiPart))
            {
                await HandleRequestAsync(context, AllowedContentType.Form);
            }
        }
        else
        {
            // if the request is not a post multipart request or multipart requests are not enabled
            // we will just invoke the next middleware and do nothing:
            await NextAsync(context);
        }
    }

    protected override async ValueTask<IReadOnlyList<GraphQLRequest>> GetRequestsFromBody(
        HttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        IFormCollection? form;

        try
        {
            var formFeature = new FormFeature(httpRequest, _formOptions);
            form = await formFeature.ReadFormAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            throw ThrowHelper.HttpMultipartMiddleware_Invalid_Form(exception);
        }

        // Parse the string values of interest from the IFormCollection
        HttpMultipartRequest multipartRequest = ParseMultipartRequest(form);
        IReadOnlyList<GraphQLRequest> requests = RequestParser.ReadOperationsRequest(
            multipartRequest.Operations);

        foreach (GraphQLRequest? graphQLRequest in requests)
        {
            InsertFilesIntoRequest(graphQLRequest, multipartRequest.FileMap);
        }

        return requests;
    }

    private static HttpMultipartRequest ParseMultipartRequest(IFormCollection form)
    {
        string? operations = null;
        Dictionary<string, string[]>? map = null;

        foreach (var field in form)
        {
            if (field.Key == _operations)
            {
                if (!field.Value.TryPeek(out operations) || string.IsNullOrEmpty(operations))
                {
                    throw ThrowHelper.HttpMultipartMiddleware_No_Operations_Specified();
                }
            }
            else if (field.Key == _map)
            {
                if (string.IsNullOrEmpty(operations))
                {
                    throw ThrowHelper.HttpMultipartMiddleware_Fields_Misordered();
                }

                field.Value.TryPeek(out var mapString);

                try
                {
                    map = JsonSerializer.Deserialize<Dictionary<string, string[]>>(mapString!);
                }
                catch
                {
                    throw ThrowHelper.HttpMultipartMiddleware_InvalidMapJson();
                }
            }
        }

        if (operations is null)
        {
            throw ThrowHelper.HttpMultipartMiddleware_No_Operations_Specified();
        }

        if (map is null)
        {
            throw ThrowHelper.HttpMultipartMiddleware_MapNotSpecified();
        }

        // Validate file mappings and bring them in an easy to use format
        IDictionary<string, IFile> pathToFileMap = MapFilesToObjectPaths(map, form.Files);

        return new HttpMultipartRequest(operations, pathToFileMap);
    }

    private static IDictionary<string, IFile> MapFilesToObjectPaths(
        IDictionary<string, string[]> map,
        IFormFileCollection files)
    {
        var pathToFileMap = new Dictionary<string, IFile>();

        foreach (var filePair in map)
        {
            if (filePair.Value is null || filePair.Value.Length < 1)
            {
                throw ThrowHelper.HttpMultipartMiddleware_NoObjectPath(filePair.Key);
            }

            IFormFile? file = filePair.Key is { Length: > 0 }
                ? files.GetFile(filePair.Key)
                : null;

            if (file is null)
            {
                throw ThrowHelper.HttpMultipartMiddleware_FileMissing(filePair.Key);
            }

            foreach (var objectPath in filePair.Value)
            {
                pathToFileMap.Add(objectPath, new UploadedFile(file));
            }
        }

        return pathToFileMap;
    }

    private static void InsertFilesIntoRequest(
        GraphQLRequest request,
        IDictionary<string, IFile> fileMap)
    {
        if (!(request.Variables is Dictionary<string, object?> mutableVariables))
        {
            throw new InvalidOperationException(
                HttpMultipartMiddleware_InsertFilesIntoRequest_VariablesImmutable);
        }

        foreach (var file in fileMap)
        {
            var path = VariablePath.Parse(file.Key);

            if (!mutableVariables.TryGetValue(path.Key.Value, out var value))
            {
                throw ThrowHelper.HttpMultipartMiddleware_VariableNotFound(file.Key);
            }

            if (path.Key.Next is null)
            {
                mutableVariables[path.Key.Value] = new FileValueNode(file.Value);
                continue;
            }

            if (value is null)
            {
                throw ThrowHelper.HttpMultipartMiddleware_VariableStructureInvalid();
            }

            mutableVariables[path.Key.Value] = RewriteVariable(
                file.Key,
                path.Key.Next,
                value,
                new FileValueNode(file.Value));
        }
    }

    private static IValueNode RewriteVariable(
        string objectPath,
        IVariablePathSegment segment,
        object value,
        FileValueNode file)
    {
        if (segment is KeyPathSegment key && value is ObjectValueNode ov)
        {
            var pos = -1;

            for (var i = 0; i < ov.Fields.Count; i++)
            {
                if (ov.Fields[i].Name.Value.EqualsOrdinal(key.Value))
                {
                    pos = i;
                    break;
                }
            }

            if (pos == -1)
            {
                throw ThrowHelper.HttpMultipartMiddleware_VariableNotFound(objectPath);
            }

            ObjectFieldNode[] fields = ov.Fields.ToArray();
            ObjectFieldNode field = fields[pos];
            fields[pos] = field.WithValue(
                key.Next is not null
                    ? RewriteVariable(objectPath, key.Next, field.Value, file)
                    : file);
            return ov.WithFields(fields);
        }

        if (segment is IndexPathSegment index && value is ListValueNode lv)
        {
            IValueNode[] items = lv.Items.ToArray();
            IValueNode item = items[index.Value];
            items[index.Value] = index.Next is not null
                ? RewriteVariable(objectPath, index.Next, item, file)
                : file;
            return lv.WithItems(items);
        }

        throw ThrowHelper.HttpMultipartMiddleware_VariableNotFound(objectPath);
    }
}
