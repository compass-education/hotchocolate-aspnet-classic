using HttpContext = Microsoft.Owin.IOwinContext;

namespace HotChocolate.AspNetClassic;

public delegate ValueTask HttpRequestInterceptorDelegate(
    HttpContext context,
    IRequestExecutor requestExecutor,
    IQueryRequestBuilder requestBuilder,
    CancellationToken cancellationToken);
