using Hangfire;
using MassTransit;
using WebStore.Contracts.User.Customer;
using WebStore.NotificationService.Interfaces.Services;
using WebStore.UserInfrastructure.Repositories;

namespace WebStore.BackWorker.Consumers;

public class CustomerRegisteredConsumer : IConsumer<CustomerRegistered>
{
    private readonly IBackgroundJobClient _hangfire;
    private readonly ILogger<CustomerRegisteredConsumer> _logger;
    private readonly UserDbContext _db;
    private readonly IEmailService _emailService;

    public CustomerRegisteredConsumer(IBackgroundJobClient hangfire, ILogger<CustomerRegisteredConsumer> logger, UserDbContext db, IEmailService emailService)
    {
        _hangfire = hangfire;
        _logger = logger;
        _db = db;

        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<CustomerRegistered> context)
    {
        var message = context.Message;
        _logger.LogInformation("Message received for Customer: {Id}. Scheduling background job...", message.Id);

        _hangfire.Schedule<CustomerRegisteredConsumer>(
            x => x.ProcessCustomerAsync(message),
            TimeSpan.FromSeconds(5));

        await Task.CompletedTask;
    }

    [Queue("default")]
    public async Task ProcessCustomerAsync(CustomerRegistered message)
    {
        _logger.LogInformation("[Hangfire Job] Warmly welcoming Customer to industry {Id}", message.Id);

        await _emailService.SendEmailAsync(message.Email, "Welcome to KN-Industry-WebStore", $"Thank you for registering with us dear customer {message.Username}");
        await Task.Delay(1000);
        _logger.LogInformation("[Hangfire Job] Successfully registered Customer {Id}", message.Id);
    }
}