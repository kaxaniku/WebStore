using Hangfire;
using MassTransit;
using WebStore.CartApp.Interfaces.Services;
using WebStore.Contracts.Catalog.Product;

namespace WebStore.BackWorker.Consumers.CatalogConsumers;

public class ProductDeletedConsumer : IConsumer<ProductDeleted>
{
    private readonly IBackgroundJobClient _hangfire;
    private readonly ILogger<ProductDeletedConsumer> _logger;
    private readonly ICartProductService _cartProductService;

    public ProductDeletedConsumer(IBackgroundJobClient hangfire, ILogger<ProductDeletedConsumer> logger, ICartProductService cartProductService)
    {
        _hangfire = hangfire;
        _logger = logger;
        _cartProductService = cartProductService;
    }

    public async Task Consume(ConsumeContext<ProductDeleted> context)
    {
        var message = context.Message;
        _logger.LogInformation("Message received for Product: {Id}. Scheduling background job...", message.Id);

        _hangfire.Schedule<ProductDeletedConsumer>(
            x => x.ProcessProductAsync(message),
            TimeSpan.FromSeconds(5));

        await Task.CompletedTask;
    }

    [Queue("default")]
    public async Task ProcessProductAsync(ProductDeleted message)
    {
        _logger.LogInformation("[Hangfire Job] Starting database work for Product {Id}", message.Id);

        await Task.Delay(1000);
        await _cartProductService.DeleteCartProductAsync(message.Id, CancellationToken.None);
        _logger.LogInformation("[Hangfire Job] Successfully processed Product {Id}", message.Id);
    }
}