using MassTransit;
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
    }
}
