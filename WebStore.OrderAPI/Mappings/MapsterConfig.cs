using Mapster;
using MapsterMapper;
using WebStore.OrderApp.Profiles;

namespace WebStore.OrderAPI.Mappings;

public static class MapsterConfig
{
    public static void RegisterMaps(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        config.Scan(typeof(OrderProfile).Assembly);

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
    }
}