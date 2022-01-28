using System;

using Microsoft.Owin;

namespace HotChocolate.AspNetClassic.Playground
{
    public class PlaygroundOptions
        : GraphiQLOptionsBase
    {
        public PlaygroundOptions() : base(new PathString("/playground"))
        {
        }
    }
}
