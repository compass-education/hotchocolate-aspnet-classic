using System;
using Microsoft.Owin;

namespace HotChocolate.AspNetClassic.GraphiQL
{
    public class GraphiQLOptions
        : GraphiQLOptionsBase
    {
        public GraphiQLOptions() : base(new PathString("/graphiql"))
        {
        }
    }
}
