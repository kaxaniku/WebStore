using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using WebStore.NotificationService.Interfaces.Services;
using WebStore.NotificationService.Services;

namespace WebStore.BackWorker.Extensions;

internal static class EmailConfig
{
    public static void ConfigureEmail(this HostApplicationBuilder builder)
    {
        var settings = new EmailSettings
        {
            SmtpServer = builder.Configuration["Email:SmtpServer"],
            SmtpPort = int.Parse(builder.Configuration["Email:SmtpPort"]!),
            FromAddress = builder.Configuration["Email:FromAddress"]!,
            Password = builder.Configuration["Email:Password"]!,
        };
        var options = Options.Create(settings);
        var emailService = new EmailService(options);
        builder.Services.AddTransient<IEmailService>(_ => emailService);
    }
}
