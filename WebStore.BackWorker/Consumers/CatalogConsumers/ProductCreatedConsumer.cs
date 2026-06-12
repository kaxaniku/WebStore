using Hangfire;
using MassTransit;
using WebStore.CartApp.Interfaces.Services;
using WebStore.Contracts.Catalog.Product;
using WebStore.OrderApp.Interfaces.Services;

namespace WebStore.BackWorker.Consumers.CatalogConsumers;

public class ProductCreatedConsumer : IConsumer<ProductCreated>
{
    private readonly IBackgroundJobClient _hangfire;
    private readonly ILogger<ProductCreatedConsumer> _logger;
    private readonly ICartProductService _cartProductService;
    private readonly IOrderProductService _orderProductService;

    public ProductCreatedConsumer(IBackgroundJobClient hangfire, ILogger<ProductCreatedConsumer> logger, ICartProductService cartProductService, IOrderProductService orderProductService)
    {
        _hangfire = hangfire;
        _logger = logger;
        _cartProductService = cartProductService;
        _orderProductService = orderProductService;
    }

    public async Task Consume(ConsumeContext<ProductCreated> context)
    {
        var message = context.Message;
        _logger.LogInformation("Message received for Product: {Id}. Scheduling background job...", message.Id);

        _hangfire.Schedule<ProductCreatedConsumer>(
            x => x.ProcessProductAsync(message),
            TimeSpan.FromSeconds(5));

        await Task.CompletedTask;
    }

    [Queue("default")]
    public async Task ProcessProductAsync(ProductCreated message)
    {
        _logger.LogInformation("[Hangfire Job] Starting database work for Product {Id}", message.Id);

        await Task.Delay(1000);
        await _cartProductService.CreateCartProductAsync(message.Id, message.Name, message.Price, message.Stock, message.ImagePath, CancellationToken.None);
        await _orderProductService.CreateOrderProductAsync(message.Id, message.Name, message.Price, message.Stock, CancellationToken.None);
        _logger.LogInformation("[Hangfire Job] Successfully processed Product {Id}", message.Id);
    }
}