using System.Threading;
using System.Threading.Tasks;
using System.Web;
using HotChocolate.Execution;

namespace HotChocolate.AspNetClassic.Server
{

    public delegate ValueTask HttpRequestInterceptorDelegate(
        HttpContext context,
        IRequestExecutor requestExecutor,
        IQueryRequestBuilder requestBuilder,
        CancellationToken cancellationToken);
}
