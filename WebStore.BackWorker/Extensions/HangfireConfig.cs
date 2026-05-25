using Hangfire;

namespace WebStore.BackWorker.Extensions;

internal static class HangfireConfig
{
    public static void ConfigureHangfire(this HostApplicationBuilder builder, string connectionString)
    {
        builder.Services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString));
        builder.Services.AddHangfireServer(options =>
        {
            options.WorkerCount = 5;
        });
    }
}
