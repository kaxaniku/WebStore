using Hangfire;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Webstore.CatalogInfrastructure.Repositories;
using WebStore.BackWorker.Consumers;

namespace WebStore.BackWorker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("Default");

        builder.Services.AddHostedService<Worker>();
        builder.Services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString));
        builder.Services.AddHangfireServer(options =>
        {
            options.WorkerCount = 1;
        });

        builder.Services.AddDbContext<CatalogDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogDb")));

        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumer<CategoryCreatedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        var host = builder.Build();
        host.Run();
    }
}