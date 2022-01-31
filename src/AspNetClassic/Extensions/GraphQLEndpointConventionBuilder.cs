using Microsoft.AspNetClassic.Builder;

namespace HotChocolate.AspNetClassic.Extensions;

/// <summary>
/// Represents the endpoint convention builder for GraphQL.
/// </summary>
public sealed class GraphQLEndpointConventionBuilder : IEndpointConventionBuilder
{
    private readonly IEndpointConventionBuilder _builder;

    internal GraphQLEndpointConventionBuilder(IEndpointConventionBuilder builder)
    {
        _builder = builder;
    }

    /// <inheritdoc />
    public void Add(Action<EndpointBuilder> convention) =>
        _builder.Add(convention);
}
