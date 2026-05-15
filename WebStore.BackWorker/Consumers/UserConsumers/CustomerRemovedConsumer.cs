using Hangfire;
using MassTransit;
using WebStore.CartApp.Interfaces.Services;
using WebStore.Contracts.User.Customer;
using WebStore.NotificationService.Interfaces.Services;

namespace WebStore.BackWorker.Consumers.UserConsumers;

public class CustomerRemovedConsumer : IConsumer<CustomerRemoved>
{
    private readonly IBackgroundJobClient _hangfire;
    private readonly ILogger<CustomerRemovedConsumer> _logger;
    private readonly ICartCustomerService _customerService;

    public CustomerRemovedConsumer(IBackgroundJobClient hangfire, ILogger<CustomerRemovedConsumer> logger, IEmailService emailService, ICartCustomerService customerService)
    {
        _hangfire = hangfire;
        _logger = logger;
        _customerService = customerService;
    }

    public async Task Consume(ConsumeContext<CustomerRemoved> context)
    {
        var message = context.Message;
        _logger.LogInformation("Message received for Customer: {Id}. Scheduling background job...", message.Id);

        _hangfire.Schedule<CustomerRemovedConsumer>(
            x => x.ProcessCustomerAsync(message),
            TimeSpan.FromSeconds(5));

        await Task.CompletedTask;
    }

    [Queue("default")]
    public async Task ProcessCustomerAsync(CustomerRemoved message)
    {
        await Task.Delay(1000);
        await _customerService.RemoveCartCustomerAsync(message.Id,CancellationToken.None);
        _logger.LogInformation("[Hangfire Job] Successfully removed Customer {Id}", message.Id);
    }
}