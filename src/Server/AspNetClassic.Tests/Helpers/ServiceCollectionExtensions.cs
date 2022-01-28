using HotChocolate.Execution.Batching;
using HotChocolate.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using StarWars;

namespace HotChocolate.AspNetClassic
{
    public static class TestingServiceCollectionExtensions
    {
        public static IServiceCollection AddStarWars(
            this IServiceCollection services)
        {
            // Add the custom services like repositories etc ...
            services.AddStarWarsRepositories();

            // Add in-memory event provider
            services.AddInMemorySubscriptionProvider();

            // Add GraphQL Services
            services.AddGraphQL(sp => SchemaBuilder.New()
                .AddServices(sp)
                .AddStarWarsTypes()
                .AddAuthorizeDirectiveType()
                .AddDirectiveType<ExportDirectiveType>()
                .Create());

            return services;
        }
    }
}
