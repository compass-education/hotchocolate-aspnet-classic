using System.Threading;
using System.Threading.Tasks;
using System.Web;
using HotChocolate.Execution;

namespace HotChocolate.AspNetClassic.Server
{
    public interface IHttpRequestInterceptor
    {
        ValueTask OnCreateAsync(
            HttpContext context,
            IRequestExecutor requestExecutor,
            IQueryRequestBuilder requestBuilder,
            CancellationToken cancellationToken);
    }
}
