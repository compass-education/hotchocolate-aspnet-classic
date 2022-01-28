using HotChocolate;
using StarWars.Types;

namespace StarWars
{
    public static class SchemaBuilderExtensions
    {
        public static ISchemaBuilder AddStarWarsTypes(
            this ISchemaBuilder schemaBuilder)
        {
            return schemaBuilder
                .AddQueryType<QueryType>()
                .AddMutationType<MutationType>()
                .AddSubscriptionType<SubscriptionType>()
                .AddType<HumanType>()
                .AddType<DroidType>()
                .AddType<EpisodeType>();
        }
    }
}
