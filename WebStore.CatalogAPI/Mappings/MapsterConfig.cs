using Mapster;
using MapsterMapper;
using WebStore.CatalogApp.Profiles;

namespace WebStore.CatalogApp.Mappings;

public static class MapsterConfig
{
    public static void RegisterMaps(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        config.Scan(typeof(CategoryProfile).Assembly);

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
    }
}