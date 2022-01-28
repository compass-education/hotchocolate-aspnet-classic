﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HotChocolate.AspNetClassic.Server
{
    public interface ISocketConnectionInterceptor<TContext>
    {
        Task<ConnectionStatus> OnOpenAsync(
            TContext context,
            IReadOnlyDictionary<string, object> properties,
            CancellationToken cancellationToken);
    }
}
