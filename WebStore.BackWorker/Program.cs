using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebStore.BackWorker.Extensions;
using WebStore.CartApp.Interfaces.Services;
using WebStore.CartApp.Profiles;
using WebStore.CartApp.Services;

namespace WebStore.BackWorker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("Default");

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog();
        });

        builder.Services.AddHostedService<Worker>();
        builder.ConfigureHangfire(connectionString!);

        builder.ConfigureContexts();

        builder.ConfigureEmail();

        var config = TypeAdapterConfig.GlobalSettings;

        config.Scan(typeof(CartProfile).Assembly);
        builder.Services.AddSingleton(config);
        builder.Services.AddScoped<IMapper, ServiceMapper>();
        builder.Services.AddScoped<CartApp.Interfaces.Repositories.IUnitOfWork, CartInfrastructure.Repositories.UnitOfWork>();
        builder.Services.AddScoped<ICartService, CartService>();
        builder.Services.AddScoped<ICartProductService, CartProductService>();
        builder.Services.AddScoped<ICartCustomerService, CartCustomerService>();

        builder.ConfigureMassTransit();

        var host = builder.Build();
        host.Run();
    }
}