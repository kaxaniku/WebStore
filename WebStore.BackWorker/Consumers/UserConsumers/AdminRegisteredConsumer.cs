using Hangfire;
using MassTransit;
using WebStore.Contracts.User.Admin;
using WebStore.NotificationService.Interfaces.Services;
using WebStore.UserInfrastructure.Repositories;

namespace WebStore.BackWorker.Consumers.UserConsumers;

public class AdminRegisteredConsumer : IConsumer<AdminRegistered>
{
    private readonly IBackgroundJobClient _hangfire;
    private readonly ILogger<AdminRegisteredConsumer> _logger;
    private readonly UserDbContext _db;
    private readonly IEmailService _emailService;

    public AdminRegisteredConsumer(IBackgroundJobClient hangfire, ILogger<AdminRegisteredConsumer> logger, UserDbContext db, IEmailService emailService)
    {
        _hangfire = hangfire;
        _logger = logger;
        _db = db;

        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<AdminRegistered> context)
    {
        var message = context.Message;
        _logger.LogInformation("Message received for Admin: {Id}. Scheduling background job...", message.Id);

        _hangfire.Schedule<AdminRegisteredConsumer>(
            x => x.ProcessAdminAsync(message),
            TimeSpan.FromSeconds(5));

        await Task.CompletedTask;
    }

    [Queue("default")]
    public async Task ProcessAdminAsync(AdminRegistered message)
    {
        _logger.LogInformation("[Hangfire Job] Warmly welcoming Admin to industry {Id}", message.Id);

        //await _emailService.SendEmailAsync(message.Email, "Welcome to KN-Industry-WebStore", $"Thank you for registering with us dear admin {message.Username}");
        await Task.Delay(1000);
        _logger.LogInformation("[Hangfire Job] Successfully registered Admin {Id}", message.Id);
    }
}