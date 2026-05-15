using Mapster;
using MapsterMapper;
using WebStore.CartApp.Profiles;

namespace WebStore.CartAPI.Mappings;

public static class MapsterConfig
{
    public static void RegisterMaps(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        config.Scan(typeof(CartProfile).Assembly);

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
    }
}