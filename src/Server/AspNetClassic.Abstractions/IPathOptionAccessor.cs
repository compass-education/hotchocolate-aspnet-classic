using Microsoft.Owin;
using HotChocolate.AspNetClassic.Interceptors;

namespace HotChocolate.AspNetClassic
{
    public interface IPathOptionAccessor
    {
        PathString Path { get; }
    }
}
