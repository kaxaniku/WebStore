using Microsoft.EntityFrameworkCore;
using Serilog;
using WebStore.BackWorker.Extensions;

namespace WebStore.BackWorker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Configuration.AddEnvironmentVariables();

        builder.Services.Configure<HostOptions>(options =>
        {
            options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost;
        });

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
        builder.ConfigureHangfire(connectionString!);

        builder.ConfigureContexts();

        builder.ConfigureCartService();
        builder.ConfigureOrderService();
        builder.ConfigureCatalogService();
        builder.ConfigureEmail();

        builder.ConfigureMassTransit();

        var host = builder.Build();
        host.Run();
    }
}