using Hangfire;
using MassTransit;
using WebStore.CartApp.Interfaces.Services;
using WebStore.Contracts.User.Customer;

namespace WebStore.BackWorker.Consumers.UserConsumers;

public class CustomerUpdatedConsumer : IConsumer<CustomerUpdated>
{
    private readonly IBackgroundJobClient _hangfire;
    private readonly ILogger<CustomerUpdatedConsumer> _logger;
    private readonly ICartCustomerService _customerService;

    public CustomerUpdatedConsumer(IBackgroundJobClient hangfire, ILogger<CustomerUpdatedConsumer> logger, ICartCustomerService customerService)
    {
        _hangfire = hangfire;
        _logger = logger;
        _customerService = customerService;
    }

    public async Task Consume(ConsumeContext<CustomerUpdated> context)
    {
        var message = context.Message;
        _logger.LogInformation("Message received for Customer: {Id}. Scheduling background job...", message.Id);

        _hangfire.Schedule<CustomerUpdatedConsumer>(
            x => x.ProcessCustomerAsync(message),
            TimeSpan.FromSeconds(5));

        await Task.CompletedTask;
    }

    [Queue("default")]
    public async Task ProcessCustomerAsync(CustomerUpdated message)
    {
        await Task.Delay(1000);
        await _customerService.UpdateUsernameAsync(message.Id, message.Username, CancellationToken.None);
        _logger.LogInformation("[Hangfire Job] Successfully updated Customer {Id}", message.Id);
    }
}