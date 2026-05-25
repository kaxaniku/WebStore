using Hangfire;
using MassTransit;
using WebStore.CartApp.Interfaces.Services;
using WebStore.Contracts.Cart;

namespace WebStore.BackWorker.Consumers.CartConsumers;

public class ClearCartRequestConsumer : IConsumer<ClearCartRequest>
{
    private readonly IBackgroundJobClient _hangfire;
    private readonly ILogger<ClearCartRequestConsumer> _logger;
    private readonly ICartService _cartService;

    public ClearCartRequestConsumer(IBackgroundJobClient hangfire, ILogger<ClearCartRequestConsumer> logger, ICartService cartService)
    {
        _hangfire = hangfire;
        _logger = logger;
        _cartService = cartService;
    }

    public async Task Consume(ConsumeContext<ClearCartRequest> context)
    {
        var message = context.Message;

        _hangfire.Schedule<ClearCartRequestConsumer>(
            x => x.ProcessCartAsync(message.CustomerId),
            TimeSpan.FromSeconds(5));

        await Task.CompletedTask;
    }

    [Queue("default")]
    public async Task ProcessCartAsync(int id)
    {
        _logger.LogInformation("[Hangfire Job] Starting database work for Clearing cart {Id}", id);

        await Task.Delay(1000);
        await _cartService.ClearCartAsync(id, CancellationToken.None);
        _logger.LogInformation("[Hangfire Job] Successfully processed Clearing cart {Id}", id);
    }
}