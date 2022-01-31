using System;
using System.Threading;
using System.Threading.Tasks;

namespace HotChocolate.AspNetClassic.Subscriptions;

public interface ISocketSession : IDisposable
{
    Task HandleAsync(CancellationToken cancellationToken);
}
