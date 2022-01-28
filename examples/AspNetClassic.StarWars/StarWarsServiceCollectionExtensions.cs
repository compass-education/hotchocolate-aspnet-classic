using Microsoft.Extensions.DependencyInjection;
using StarWars.Data;

namespace StarWars
{
    public static class StarWarsServiceCollectionExtensions
    {
        public static IServiceCollection AddStarWarsRepositories(
            this IServiceCollection services)
        {
            services.AddSingleton<CharacterRepository>();
            services.AddSingleton<ReviewRepository>();
            return services;
        }
    }
}
