using Hangfire;
using MassTransit;
using WebStore.CatalogApp.Interfaces.Services;
using WebStore.Contracts.User.Admin;
using WebStore.NotificationService.Interfaces.Services;
using WebStore.UserInfrastructure.Repositories;

namespace WebStore.BackWorker.Consumers.UserConsumers;

public class AdminUpdatedConsumer : IConsumer<AdminUpdated>
{
    private readonly IBackgroundJobClient _hangfire;
    private readonly ILogger<AdminUpdatedConsumer> _logger;
    private readonly UserDbContext _db;
    private readonly IEmailService _emailService;
    private readonly IAuthService _authService;

    public AdminUpdatedConsumer(IBackgroundJobClient hangfire, ILogger<AdminUpdatedConsumer> logger, UserDbContext db, IEmailService emailService, IAuthService authService)
    {
        _hangfire = hangfire;
        _logger = logger;
        _db = db;
        _authService = authService;
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<AdminUpdated> context)
    {
        var message = context.Message;
        _logger.LogInformation("Message received for Admin: {Id}. Scheduling background job...", message.Id);

        _hangfire.Schedule<AdminUpdatedConsumer>(
            x => x.ProcessAdminAsync(message),
            TimeSpan.FromSeconds(5));

        await Task.CompletedTask;
    }

    [Queue("default")]
    public async Task ProcessAdminAsync(AdminUpdated message)
    {
        _logger.LogInformation("[Hangfire Job] Warmly welcoming Admin to industry {Id}", message.Id);
        if(!string.IsNullOrEmpty(message.newPw))
        {
            await _authService.ChangePasswordAsync(message.Id, message.oldPw, message.newPw, CancellationToken.None);
            //await _emailService.SendEmailAsync(message.Email, "Password Change Notification", $"Dear admin {message.Username}, your password has been successfully changed.");
        }
        if (!string.IsNullOrEmpty(message.Username)) {
            await _authService.UpdateUsernameAsync(message.Id, message.Username, CancellationToken.None);
            //await _emailService.SendEmailAsync(message.Email, "Username Change Notification", $"Dear admin {message.Username}, your username has been successfully changed.");
        }
        await Task.Delay(1000);
        _logger.LogInformation("[Hangfire Job] Successfully registered Admin {Id}", message.Id);
    }
}