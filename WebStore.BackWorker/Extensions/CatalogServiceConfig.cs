using Mapster;
using MapsterMapper;
using R2StorageApp;
using WebStore.CatalogApp.Interfaces.Services;
using WebStore.CatalogApp.Services;

namespace WebStore.BackWorker.Extensions;

internal static class CatalogServiceConfig
{
    public static void ConfigureCatalogService(this HostApplicationBuilder builder)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        config.Scan(typeof(CatalogApp.Profiles.ProductProfile).Assembly);
        builder.Services.AddSingleton(config);
        builder.Services.AddScoped<IMapper, ServiceMapper>();
        builder.Services.AddScoped<CatalogApp.Interfaces.Repositories.IUnitOfWork, Webstore.CatalogInfrastructure.Repositories.UnitOfWork>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddR2StorageServices(builder.Configuration);
    }
}
