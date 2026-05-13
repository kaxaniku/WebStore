using MassTransit;
using WebStore.BackWorker.Consumers;

namespace WebStore.BackWorker.Extensions;

internal static class MassTransitConfig
{
    public static void ConfigureMassTransit(this HostApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumer<CategoryCreatedConsumer>();
            x.AddConsumer<CustomerRegisteredConsumer>();
            x.AddConsumer<AdminRegisteredConsumer>();

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
