using System;
using System.Collections.Generic;

namespace HotChocolate.AspNetClassic.Subscriptions;

public interface ISubscriptionManager
    : IEnumerable<ISubscriptionSession>
    , IDisposable
{
    void Register(ISubscriptionSession subscriptionSession);

    void Unregister(string subscriptionId);
}
