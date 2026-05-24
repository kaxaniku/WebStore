using Microsoft.EntityFrameworkCore;
using Webstore.CatalogInfrastructure.Repositories;
using WebStore.CartInfrastructure.Repositories;
using WebStore.OrderInfrastructure.Repositories;
using WebStore.UserInfrastructure.Repositories;

namespace WebStore.BackWorker.Extensions;

internal static class DbConfig
{
    public static void ConfigureContexts(this HostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<CatalogDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogDb")));
        builder.Services.AddDbContext<UserDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("UserDb")));
        builder.Services.AddDbContext<CartDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("CartDb")));
        builder.Services.AddDbContext<OrderDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("OrderDb")));
    }
}
