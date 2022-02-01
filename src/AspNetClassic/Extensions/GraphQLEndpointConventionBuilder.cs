using Microsoft.AspNetClassic.Builder;

using IApplicationBuilder = Owin.IAppBuilder;

namespace HotChocolate.AspNetClassic.Extensions;

/// <summary>
/// Represents the endpoint convention builder for GraphQL.
/// </summary>
public sealed class GraphQLEndpointConventionBuilder : IApplicationBuilder
{
    private readonly IApplicationBuilder _builder;

    internal GraphQLEndpointConventionBuilder(IApplicationBuilder builder)
    {
        _builder = builder;
    }

    public IApplicationBuilder Use(object middleware, params object[] args)
    {
        return _builder.Use(middleware, args);
    }

    public object Build(Type returnType)
    {
        return _builder.Build(returnType);
    }

    public IApplicationBuilder New()
    {
        return _builder.New();
    }

    public IDictionary<string, object> Properties => _builder.Properties;
}
