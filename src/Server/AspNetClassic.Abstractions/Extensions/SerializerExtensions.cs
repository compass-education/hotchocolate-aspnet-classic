using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Execution;
using Microsoft.Owin;
using HttpContext = Microsoft.Owin.IOwinContext;

namespace HotChocolate.AspNetClassic
{
    public static class SerializerExtensions
    {
        public static ValueTask SerializeAsync(
            this IQueryResultSerializer serializer,
            IExecutionResult result,
            Stream outputStream,
            CancellationToken cancellationToken)
        {
            if (result is IReadOnlyQueryResult queryResult)
            {
                return serializer.SerializeAsync(
                    queryResult,
                    outputStream,
                    cancellationToken);
            }
            else
            {
                // TODO : resources
                return serializer.SerializeAsync(
                    QueryResult.CreateError(
                        ErrorBuilder.New()
                            .SetMessage("Result type not supported.")
                            .SetCode(ErrorCodes.Serialization.ResultTypeNotSupported)
                            .Build()),
                    outputStream,
                    cancellationToken);
            }
        }
    }
}
