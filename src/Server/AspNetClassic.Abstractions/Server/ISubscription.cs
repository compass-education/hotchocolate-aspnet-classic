using System;

namespace HotChocolate.AspNetClassic.Server
{
    public interface ISubscription
        : IDisposable
    {
        event EventHandler Completed;

        string Id { get; }
    }
}
