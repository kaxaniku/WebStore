using Hangfire;
using MassTransit;
using WebStore.CartApp.Interfaces.Services;
using WebStore.Contracts.Cart;

namespace WebStore.BackWorker.Consumers.CatalogConsumers;

public class GetCartRequestConsumer : IConsumer<GetCartRequest>
{
    private readonly ILogger<GetCartRequestConsumer> _logger;
    private readonly ICartService _cartService;

    public GetCartRequestConsumer(ILogger<GetCartRequestConsumer> logger, ICartService cartService)
    {
        _logger = logger;
        _cartService = cartService;
    }

    public async Task Consume(ConsumeContext<GetCartRequest> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing cart request immediately for Customer: {Id}", message.CustomerId);

        var cart = await _cartService.GetCartAsync(message.CustomerId, context.CancellationToken);

        var items = cart.Items.Select(i => new CartItemMessage(
            i.Product.Id,
            i.Product.Name,
            i.Product.Price,
            i.Quantity
        )).ToList();

        await context.RespondAsync(new GetCartResponse(message.CustomerId, items));
    }
}