using MassTransit;
using WebStore.BackWorker.Consumers.CartConsumers;
using WebStore.BackWorker.Consumers.CatalogConsumers;
using WebStore.BackWorker.Consumers.UserConsumers;

namespace WebStore.BackWorker.Extensions;

internal static class MassTransitConfig
{
    public static void ConfigureMassTransit(this HostApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumer<CategoryCreatedConsumer>();
            x.AddConsumer<CustomerRegisteredConsumer>();
            x.AddConsumer<CustomerUpdatedConsumer>();
            x.AddConsumer<CustomerRemovedConsumer>();
            x.AddConsumer<ProductCreatedConsumer>();
            x.AddConsumer<ProductDeletedConsumer>();
            x.AddConsumer<ProductUpdatedConsumer>();
            x.AddConsumer<AdminRegisteredConsumer>();
            x.AddConsumer<GetCartRequestConsumer>();
            x.AddConsumer<ClearCartRequestConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";

                cfg.Host(rabbitHost, "/", h =>
                {
                    h.Username("webstore_admin");
                    h.Password("RabbitSecurePass2026!");
                });

                cfg.ConfigureEndpoints(context);
            });
        });
    }
}
