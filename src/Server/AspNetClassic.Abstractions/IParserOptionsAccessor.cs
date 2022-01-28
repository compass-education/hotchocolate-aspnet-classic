using HotChocolate.Language;
using Microsoft.Owin;
using HotChocolate.AspNetClassic.Interceptors;

namespace HotChocolate.AspNetClassic
{
    public interface IParserOptionsAccessor
    {
        ParserOptions ParserOptions { get; }
    }
}
