using Microsoft.Extensions.Options;
using WebStore.NotificationService.Interfaces.Services;
using WebStore.NotificationService.Services;

namespace WebStore.NotificationTests;

public class Tests
{
    private IEmailService _emailService;

    [SetUp]
    public void Setup()
    {
        var settings = new EmailSettings
        {
            SmtpServer = "smtp.gmail.com",
            SmtpPort = 587,
            FromAddress = "knindustrybank@gmail.com",
            Password = "mhwg rwoe bkek svhs",
        };

        var options = Options.Create(settings);

        _emailService = new EmailService(options);
    }

    [Test]
    public void LaunchEmail()
    {
        Assert.DoesNotThrowAsync(async () =>
        await _emailService.SendEmailAsync(
            "knindustrybank@gmail.com",
            "KN-Industry-Webstore",
            "Working mail Service")
        );
    }
}