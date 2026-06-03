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

        //Log.Logger = new LoggerConfiguration()
        //    .ReadFrom.Configuration(builder.Configuration)
        //    .CreateLogger();

        builder.Services.AddLogging(loggingBuilder =>
        {
            //loggingBuilder.ClearProviders();
            //loggingBuilder.AddSerilog();
            loggingBuilder.AddConfiguration(builder.Configuration.GetSection("Logging"));
            loggingBuilder.AddConsole();
            loggingBuilder.AddDebug();
        });

        builder.Services.Configure<HostOptions>(options =>
        {
            options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost;
        });

        var connectionString = builder.Configuration.GetConnectionString("Default");

        builder.Services.AddHostedService<Worker>();

        builder.ConfigureContexts();

        builder.ConfigureCartService();
        builder.ConfigureOrderService();
        builder.ConfigureCatalogService();
        builder.ConfigureEmail();

        builder.ConfigureMassTransit();
        builder.ConfigureHangfire(connectionString!);

        var host = builder.Build();
        host.Run();
    }
}