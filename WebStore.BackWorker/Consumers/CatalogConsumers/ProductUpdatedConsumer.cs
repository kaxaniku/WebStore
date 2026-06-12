using Hangfire;
using MassTransit;
using WebStore.CartApp.Interfaces.Services;
using WebStore.CatalogApp.Interfaces.Services;
using WebStore.Contracts.Catalog.Product;
using WebStore.OrderApp.Interfaces.Services;

namespace WebStore.BackWorker.Consumers.CatalogConsumers;

public class ProductUpdatedConsumer : IConsumer<ProductUpdated>
{
    private readonly IBackgroundJobClient _hangfire;
    private readonly ILogger<ProductUpdatedConsumer> _logger;
    private readonly ICartProductService _cartProductService;
    private readonly IOrderProductService _orderProductService;
    private readonly IProductService _productService;

    public ProductUpdatedConsumer(IBackgroundJobClient hangfire, ILogger<ProductUpdatedConsumer> logger, ICartProductService cartProductService, IOrderProductService orderProductService, IProductService productService)
    {
        _hangfire = hangfire;
        _logger = logger;
        _cartProductService = cartProductService;
        _orderProductService = orderProductService;
        _productService = productService;
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
        await _productService.UpdateLocalProductStockAsync(message.Id, message.Stock, CancellationToken.None);

        await _cartProductService.UpdateProductNameAsync(message.Id, message.Name, CancellationToken.None);
        await _cartProductService.UpdateProductPriceAsync(message.Id, message.Price, CancellationToken.None);
        await _cartProductService.UpdateProductStockAsync(message.Id, message.Stock, CancellationToken.None);
        await _cartProductService.UpdateProductImagePathAsync(message.Id, message.ImagePath, CancellationToken.None);

        await _orderProductService.UpdateProductNameAsync(message.Id, message.Name, CancellationToken.None);
        await _orderProductService.UpdateProductPriceAsync(message.Id, message.Price, CancellationToken.None);
        await _orderProductService.UpdateProductStockAsync(message.Id, message.Stock, CancellationToken.None);
        _logger.LogInformation("[Hangfire Job] Successfully processed Product {Id}", message.Id);
    }
}