using Hangfire;
using MassTransit;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Webstore.CatalogInfrastructure.Repositories;
using WebStore.BackWorker.Consumers;
using WebStore.NotificationService.Interfaces.Services;
using WebStore.NotificationService.Services;
using WebStore.UserInfrastructure.Repositories;

namespace WebStore.BackWorker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("Default");

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog();
        });

        builder.Services.AddHostedService<Worker>();
        builder.Services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString));
        builder.Services.AddHangfireServer(options =>
        {
            options.WorkerCount = 1;
        });

        builder.Services.AddDbContext<CatalogDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogDb")));
        builder.Services.AddDbContext<UserDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("UserDb")));

        var settings = new EmailSettings
        {
            SmtpServer = builder.Configuration["Email:SmtpServer"],
            SmtpPort = int.Parse(builder.Configuration["Email:SmtpPort"]!),
            FromAddress = builder.Configuration["Email:FromAddress"]!,
            Password = builder.Configuration["Email:Password"]!,
        };
        var options = Options.Create(settings);
        var emailService = new EmailService(options);
        builder.Services.AddScoped<IEmailService>(_ => emailService);


        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumer<CategoryCreatedConsumer>();
            x.AddConsumer<CustomerRegisteredConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        var host = builder.Build();
        host.Run();
    }
}