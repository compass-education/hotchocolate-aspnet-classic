using System.Threading;
using System.Threading.Tasks;
using HotChocolate.AspNetClassic.Subscriptions.Messages;

namespace HotChocolate.AspNetClassic.Subscriptions;

internal static class SocketConnectionExtensions
{
    public static Task SendAsync(
        this ISocketConnection connection,
        OperationMessage message,
        CancellationToken cancellationToken) =>
        connection.SendAsync(message.Serialize(), cancellationToken);
}
