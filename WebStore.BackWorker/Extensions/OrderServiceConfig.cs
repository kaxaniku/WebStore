using Mapster;
using MapsterMapper;
using WebStore.OrderApp.Interfaces.Services;
using WebStore.OrderApp.Services;

namespace WebStore.BackWorker.Extensions;

internal static class OrderServiceConfig
{
    public static void ConfigureOrderService(this HostApplicationBuilder builder)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        config.Scan(typeof(OrderApp.Profiles.ProductProfile).Assembly);
        builder.Services.AddSingleton(config);
        builder.Services.AddScoped<IMapper, ServiceMapper>();
        builder.Services.AddScoped<OrderApp.Interfaces.Repositories.IUnitOfWork, OrderInfrastructure.Repositories.UnitOfWork>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IOrderProductService, OrderProductService>();
    }
}
