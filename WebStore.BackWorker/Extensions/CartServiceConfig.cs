using Mapster;
using MapsterMapper;
using WebStore.CartApp.Interfaces.Services;
using WebStore.CartApp.Services;

namespace WebStore.BackWorker.Extensions;

internal static class CartServiceConfig
{
    public static void ConfigureCartService(this HostApplicationBuilder builder)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        config.Scan(typeof(CartApp.Profiles.CartProfile).Assembly);
        builder.Services.AddSingleton(config);
        builder.Services.AddScoped<IMapper, ServiceMapper>();
        builder.Services.AddScoped<CartApp.Interfaces.Repositories.IUnitOfWork, CartInfrastructure.Repositories.UnitOfWork>();
        builder.Services.AddScoped<ICartService, CartService>();
        builder.Services.AddScoped<ICartProductService, CartProductService>();
        builder.Services.AddScoped<ICartCustomerService, CartCustomerService>();
    }
}
