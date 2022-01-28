using System;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate.Execution;
using HotChocolate.Language;
using System.Collections.Generic;
using Microsoft.Owin;
using HttpContext = Microsoft.Owin.IOwinContext;
using RequestDelegate = Microsoft.Owin.OwinMiddleware;

namespace HotChocolate.AspNetClassic
{
    public class HttpGetMiddleware
        : QueryMiddlewareBase
    {
        private const string _namedQueryIdentifier = "namedQuery";
        private const string _operationNameIdentifier = "operationName";
        private const string _queryIdentifier = "query";
        private const string _variablesIdentifier = "variables";

        private readonly IQueryExecutor _queryExecutor;
        private readonly IQueryResultSerializer _resultSerializer;

        public HttpGetMiddleware(
            RequestDelegate next,
            IHttpGetMiddlewareOptions options,
            OwinContextAccessor owinContextAccessor,
            IQueryExecutor queryExecutor,
            IQueryResultSerializer resultSerializer,
            IErrorHandler errorHandler)
            : base(next,
                options,
                owinContextAccessor,
                queryExecutor.Schema.Services,
                resultSerializer,
                errorHandler)
        {
            _queryExecutor = queryExecutor
                ?? throw new ArgumentNullException(nameof(queryExecutor));
            _resultSerializer = resultSerializer
                ?? throw new ArgumentNullException(nameof(resultSerializer));
        }

        /// <inheritdoc />
        protected override bool CanHandleRequest(HttpContext context)
        {
            return string.Equals(
                context.Request.Method,
                HttpMethods.Get,
                StringComparison.Ordinal) &&
                    HasQueryParameter(context);
        }

        protected override async Task ExecuteRequestAsync(
            HttpContext context,
            IServiceProvider services)
        {
            var builder = QueryRequestBuilder.New();

            IReadableStringCollection requestQuery = context.Request.Query;

            builder
                .SetQuery(requestQuery[_queryIdentifier])
                .SetQueryName(requestQuery[_namedQueryIdentifier])
                .SetOperation(requestQuery[_operationNameIdentifier]);

            string variables = requestQuery[_variablesIdentifier];
            if (variables != null
                && variables.Length > 0
                && Utf8GraphQLRequestParser.ParseJson(variables)
                    is IReadOnlyDictionary<string, object> v)
            {
                builder.SetVariableValues(v);
            }

            IReadOnlyQueryRequest request =
                await BuildRequestAsync(
                    context,
                    services,
                    builder)
                    .ConfigureAwait(false);

            IExecutionResult result = await _queryExecutor
                .ExecuteAsync(request, context.GetCancellationToken())
                .ConfigureAwait(false);

            SetResponseHeaders(
                context.Response,
                _resultSerializer.ContentType);

            await _resultSerializer.SerializeAsync(
                result,
                context.Response.Body,
                context.GetCancellationToken())
                .ConfigureAwait(false);
        }

        private static bool HasQueryParameter(HttpContext context)
        {
            return context.Request.Query[_queryIdentifier] != null
                   || context.Request.Query[_namedQueryIdentifier] != null;
        }
    }
}
