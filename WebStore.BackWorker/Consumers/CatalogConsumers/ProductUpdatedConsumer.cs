using Hangfire;
using MassTransit;
using WebStore.CartApp.Interfaces.Services;
using WebStore.Contracts.Catalog.Product;

namespace WebStore.BackWorker.Consumers.CatalogConsumers;

public class ProductUpdatedConsumer : IConsumer<ProductUpdated>
{
    private readonly IBackgroundJobClient _hangfire;
    private readonly ILogger<ProductUpdatedConsumer> _logger;
    private readonly ICartProductService _cartProductService;

    public ProductUpdatedConsumer(IBackgroundJobClient hangfire, ILogger<ProductUpdatedConsumer> logger, ICartProductService cartProductService)
    {
        _hangfire = hangfire;
        _logger = logger;
        _cartProductService = cartProductService;
    }

    public async Task Consume(ConsumeContext<ProductUpdated> context)
    {
        var message = context.Message;
        _logger.LogInformation("Message received for Product: {Id}. Scheduling background job...", message.Id);

        _hangfire.Schedule<ProductUpdatedConsumer>(
            x => x.ProcessProductAsync(message),
            TimeSpan.FromSeconds(5));

        await Task.CompletedTask;
    }

    [Queue("default")]
    public async Task ProcessProductAsync(ProductUpdated message)
    {
        _logger.LogInformation("[Hangfire Job] Starting database work for Product {Id}", message.Id);

        await Task.Delay(1000);
        await _cartProductService.UpdateProductPriceAsync(message.Id, message.Price, CancellationToken.None);
        await _cartProductService.UpdateProductStockAsync(message.Id, message.Stock, CancellationToken.None);
        _logger.LogInformation("[Hangfire Job] Successfully processed Product {Id}", message.Id);
    }
}