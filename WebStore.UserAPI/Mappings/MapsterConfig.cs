using Mapster;
using MapsterMapper;
using WebStore.UserApp.Profiles;

namespace WebStore.UserAPI.Mappings;

public static class MapsterConfig
{
    public static void RegisterMaps(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        config.Scan(typeof(AdminProfile).Assembly);

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
    }
}