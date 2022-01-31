using HotChocolate.AspNetClassic.Subscriptions;
using HotChocolate.AspNetClassic.Subscriptions.Messages;

namespace HotChocolate.AspNetClassic;

public interface ISocketSessionInterceptor
{
    ValueTask<ConnectionStatus> OnConnectAsync(
        ISocketConnection connection,
        InitializeConnectionMessage message,
        CancellationToken cancellationToken);

    ValueTask OnRequestAsync(
        ISocketConnection connection,
        IQueryRequestBuilder requestBuilder,
        CancellationToken cancellationToken);

    ValueTask OnCloseAsync(
        ISocketConnection connection,
        CancellationToken cancellationToken);
}
