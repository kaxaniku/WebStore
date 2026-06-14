using Hangfire;
using MassTransit;
using WebStore.CatalogApp.Interfaces.Services;
using WebStore.Contracts.User.Admin;
using WebStore.NotificationService.Interfaces.Services;
using WebStore.UserInfrastructure.Repositories;

namespace WebStore.BackWorker.Consumers.UserConsumers;

public class AdminRemovedConsumer : IConsumer<AdminRemoved>
{
    private readonly IBackgroundJobClient _hangfire;
    private readonly ILogger<AdminRemovedConsumer> _logger;
    private readonly UserDbContext _db;
    private readonly IEmailService _emailService;
    private readonly IAuthService _authService;

    public AdminRemovedConsumer(IBackgroundJobClient hangfire, ILogger<AdminRemovedConsumer> logger, UserDbContext db, IEmailService emailService, IAuthService authService)
    {
        _hangfire = hangfire;
        _logger = logger;
        _db = db;
        _authService = authService;
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<AdminRemoved> context)
    {
        var message = context.Message;
        _logger.LogInformation("Message received for Admin: {Id}. Scheduling background job...", message.Id);

        _hangfire.Schedule<AdminRemovedConsumer>(
            x => x.ProcessAdminAsync(message),
            TimeSpan.FromSeconds(5));

        await Task.CompletedTask;
    }

    [Queue("default")]
    public async Task ProcessAdminAsync(AdminRemoved message)
    {
        _logger.LogInformation("[Hangfire Job] Warmly welcoming Admin to industry {Id}", message.Id);

        await _authService.RemoveAdminAsync(message.Id, CancellationToken.None);
        await Task.Delay(1000);
        _logger.LogInformation("[Hangfire Job] Successfully registered Admin {Id}", message.Id);
    }
}