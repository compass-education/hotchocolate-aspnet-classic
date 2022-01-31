using HttpContext = Microsoft.Owin.IOwinContext;

namespace HotChocolate.AspNetClassic;

public interface IHttpRequestInterceptor
{
    ValueTask OnCreateAsync(
        HttpContext context,
        IRequestExecutor requestExecutor,
        IQueryRequestBuilder requestBuilder,
        CancellationToken cancellationToken);
}
